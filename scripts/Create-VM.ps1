<#
    .SYNOPSIS
        Utility for creating a VM in Azure with WinRM over HTTPS enabled.

        Outputs the BaseUrl for accessing the VM.

    .PARAMETER SubscriptionId
        The Azure subscriptionId to generate resources in
    .PARAMETER ResourceGroupName
        The Azure resourceGroupName to generate resources in
    .PARAMETER Location
        The Azure location to generate resources in (westus, westus2, etc.)
    .PARAMETER VaultName
        The Azure KeyVault to store the HTTPS certificate
    .PARAMETER VmName
        The Azure VM name to generate
    .PARAMETER UserName
        The userName of the user
    .PARAMETER Password
        The password of the user
    .PARAMETER NsgId
        Resource identifier for NSG to be applied to VNET
    .EXAMPLE
        Create-VM -SubscriptionId af90f260-8304-49a9-84bf-1784ddb6a845 -ResourceGroupName winrm-rg-demo -Location westus2 -VaultName winrm-kv-demo -VmName winrm-vm-001 -UserName user1 -Password password1
#>
[CmdletBinding()]
param (
    [Parameter(Mandatory=$true)]
    [string]$SubscriptionId,

    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName,
    
    [Parameter(Mandatory=$true)]
    [string]$Location,
    
    [Parameter(Mandatory=$true)]
    [string]$VaultName,

    [Parameter(Mandatory=$true)]
    [string]$VmName,

    [Parameter(Mandatory=$true)]
    [string]$UserName,

    [Parameter(Mandatory=$true)]
    [securestring]$Password,

    [Parameter(Mandatory=$false)]
    [string]$NsgId
)

$secretName = "certificate"

function Write-Success($message)
{
    Write-Host "  $([char]0x221A) " -ForegroundColor DarkGreen -NoNewline
    Write-Host $message
}

function Write-Error($message)
{
    Write-Host "  ✗ " -ForegroundColor DarkRed -NoNewline
    Write-Host $message
}

function Write-Warning($message)
{
    Write-Host "  - " -NoNewline
    Write-Host $message
}

$context = Get-AzContext

if ($null -eq $context) 
{
    Connect-AzAccount | Out-Null    
}

Set-AzContext -Tenant "72f988bf-86f1-41af-91ab-2d7cd011db47" -Subscription $SubscriptionId | Out-Null

Write-Host "Resource Group"
$resourceGroup = Get-AzResourceGroup -ResourceGroupName $ResourceGroupName -ErrorAction SilentlyContinue

if ($null -eq $resourceGroup) 
{
    $resourceGroup = New-AzResourceGroup -ResourceGroupName $ResourceGroupName -Location $Location
    Write-Success "Created resource group '$($resourceGroup.ResourceId)'"
} 
else 
{
    Write-Warning "Resource group '$($resourceGroup.ResourceId)' already exists"
}

Write-Host "Key Vault"
$keyVault = Get-AzKeyVault -ResourceGroupName $ResourceGroupName -VaultName $VaultName 3> $null

if ($null -eq $keyVault) 
{
    $keyVault = New-AzKeyVault -VaultName $VaultName -ResourceGroupName $ResourceGroupName -Location $Location -EnabledForDeployment -EnabledForTemplateDeployment
    Write-Success "Created key vault '$($keyVault.ResourceId)'"
}
else
{
    Write-Warning "Key vault '$($keyVault.ResourceId)' already exists"
}

Write-Host "Local certificate"
$certificateName = $VaultName
$cerficatePath = "$PSScriptRoot\..\config\$certificateName.pfx"

if (!(Test-Path $cerficatePath))
{
    $configRoot = Split-Path $cerficatePath -Parent

    if (!(Test-Path $configRoot))
    {
        New-Item -Path $configRoot -ItemType Directory -Force | Out-Null
    }

    $thumbprint = (New-SelfSignedCertificate -DnsName $certificateName -CertStoreLocation Cert:\CurrentUser\My -KeySpec KeyExchange).Thumbprint
    $cert = (Get-ChildItem -Path cert:\CurrentUser\My\$thumbprint)
    $certPassword = Read-Host -Prompt "Please enter the certificate password" -AsSecureString

    Export-PfxCertificate -Cert $cert -FilePath $cerficatePath -Password $certPassword | Out-Null

    Write-Success "Created local certificate '$([System.IO.Path]::GetFullPath($cerficatePath))'"
} 
else 
{
    Write-Warning "Local certificate '$([System.IO.Path]::GetFullPath($cerficatePath))' already exists"
}

Write-Host "Key Vault Certificate Secret"
$secret = Get-AzKeyVaultSecret -VaultName $VaultName -Name $secretName

if ($null -eq $secret) 
{
    $certificateContentBytes = Get-Content $cerficatePath -AsByteStream
    $certificateContentEncoded = [System.Convert]::ToBase64String($certificateContentBytes)
    
    if ($null -eq $certPassword)
    {
        $certPassword = Read-Host -Prompt "Please enter the certificate password" -AsSecureString
    }

    [System.Collections.HashTable]$TableForJSON = @{
        "data"     = $certificateContentEncoded;
        "dataType" = "pfx";
        "password" = (ConvertFrom-SecureString $certPassword -AsPlainText);
    }
    
    [System.String]$jsonObject = $TableForJSON | ConvertTo-Json
    $encoding = [System.Text.Encoding]::UTF8
    $jsonEncoded = [System.Convert]::ToBase64String($encoding.GetBytes($jsonObject))
    $secret = ConvertTo-SecureString -String $jsonEncoded -AsPlainText –Force
    $secret = Set-AzKeyVaultSecret -VaultName $VaultName -Name $secretName -SecretValue $secret
    
    Write-Success "Created key vault certificate secret '$($secret.Id)'"
}
else
{
    Write-Warning "Key vault certificate secret '$($secret.Id)' already exists"
}

Write-Host "Virtual Machine"

$vm = Get-AzVM -ResourceGroupName $ResourceGroupName -Name $VmName -ErrorAction SilentlyContinue

if ($null -eq $vm)
{
    $vmSize = "Standard_B4ms"

    $deployment = New-AzResourceGroupDeployment `
        -ResourceGroupName $ResourceGroupName `
        -TemplateFile "$PSScriptRoot\..\config\deployment-template.json" `
        -adminPassword $Password `
        -adminUsername $UserName `
        -vmName $VmName `
        -vmSize $vmSize `
        -nsgId $nsgId `
        -vaultId ($keyVault.ResourceId) `
        -certificateUrl ($secret.id)

    if ($deployment.ProvisioningState -ne 'Succeeded')
    {
        Write-Error "Failure deploying template"
        exit
    }

    $vm = Get-AzVM -ResourceGroupName $ResourceGroupName -Name $VmName -ErrorAction SilentlyContinue

    Write-Success "Created virtual machine '$($vm.Id)'"
}
else
{
    Write-Warning "Virtual machine '$($vm.Id)' already exists"
}

if ($null -ne $vm)
{
    $publicIpAddress = Get-AzPublicIpAddress -ResourceGroupName $ResourceGroupName | Where-Object {$_.name -like "*$VmName*" }

    $baseUrl = "https://$($publicIpAddress.IpAddress):5986"
    Write-Success "Retrieved baseUrl '$baseUrl'"
}