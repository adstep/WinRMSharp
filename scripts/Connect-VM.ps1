<#
    .SYNOPSIS
        Connect-VM userName password baseUrl

    .PARAMETER UserName
        The userName of the user
    .PARAMETER Password
        The password of the user
    .PARAMETER BaseUrl
        Remote host in https://host:port format.
    .EXAMPLE
        Connect-VM -UserName user1 -Password password1 -BaseUrl https://10.10.10.10:5986
#>
param (
    [Parameter(Position = 0, Mandatory=$true)]
    [string]$UserName, 
    
    [Parameter(Position = 1, Mandatory=$true)]
    [securestring]$Password,

    [Parameter(Position = 2, Mandatory=$true)]
    [string]$BaseUrl
)

$cred = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $UserName, $Password

Enter-PSSession -ConnectionUri $BaseUrl -Credential $cred -SessionOption (New-PSSessionOption -SkipCACheck -SkipCNCheck -SkipRevocationCheck) -Authentication Negotiate