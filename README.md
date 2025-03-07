# WinRMSharp

[![Github Release](https://img.shields.io/github/release/adstep/WinRMSharp)](https://github.com/adstep/WinRMSharp/releases/latest)
[![NuGet](https://img.shields.io/nuget/vpre/WinRMSharp)](https://www.nuget.org/packages/WinRMSharp)
[![License](https://img.shields.io/github/license/adstep/WinRMSharp)](https://github.com/adstep/WinRMSharp/blob/main/LICENSE)

[![Build Status](https://dev.azure.com/adstep/OSS/_apis/build/status/WinRMSharp%20CI)](https://dev.azure.com/adstep/OSS/_build/latest?definitionId=5)
[![Coverage](https://img.shields.io/azure-devops/coverage/adstep/OSS/5)](https://dev.azure.com/adstep/OSS/_build/latest?definitionId=5)

WinRMSharp is a .NET Client for Windows Remote Management (WinRM) service. It allows you to invoke commands on target Windows machines from any machine that can run .NET.

WinRM allows you to perform various management tasks remotely. These include, but are not limited to: running batch scripts, powershell scripts and fetching WMI variables.

For more information on WinRM, please visit [Microsoft's WinRM site](https://learn.microsoft.com/en-us/windows/win32/winrm/portal?redirectedfrom=MSDN).

## Example usage

### Run a process on a remote host

```csharp
using System.Net;
using WinRMSharp;

internal class Program
{
    static async Task Main(string[] args)
    {
        string baseUrl = "https://windows-host.example.com:5986";
        string userName = "john.smith";
        string password = "secret";

        ICredentials credentials = new NetworkCredential()
        {
            UserName = userName,
            Password = password
        };

        WinRMClient client = new WinRMClient(baseUrl, credentials);

        CommandState commandState = await client.RunCommand("ipconfig", new string[] { "/all" });

        Console.WriteLine($"StatusCode: {commandState.StatusCode}");
        Console.WriteLine($"Stdout: \r\n{commandState.Stdout}");
        Console.WriteLine($"Stderr: \r\n{commandState.Stderr}");
    }
}
````

### Run process with low-level API

```csharp
using System.Net;
using System.Xml.Linq;
using WinRMSharp;

internal class Program
{
    static async Task Main(string[] args)
    {
        string baseUrl = "https://windows-host.example.com:5986";
        string userName = "john.smith";
        string password = "secret";

        ICredentials credentials = new NetworkCredential()
        {
            UserName = userName,
            Password = password
        };

        ITransport transport = new Transport(baseUrl, credentials, transportOptions);
        Protocol protocol = new Protocol(transport);

        string shellId = await protocol.OpenShell();
        Console.WriteLine($"Opened shell '{shellId}'");

        string commandId = await protocol.RunCommand(shellId, "ipconfig", new string[] { "/all" });
        Console.WriteLine($"Started command '{commandId}'");

        CommandState commandState = await protocol.GetCommandState(shellId, commandId);

        Console.WriteLine($"StatusCode: {commandState.StatusCode}");
        Console.WriteLine($"Stdout: \r\n{commandState.Stdout}");
        Console.WriteLine($"Stderr: \r\n{commandState.Stderr}");
    }
}
```

## Enabling WinRM on remote host

### Setup Azure VM

To simplify the process of getting a machine ready, run [Create-VM.ps1](scripts/Create-VM.ps1). The script is setup to provision a VM in Azure w/ the appropriate configuration to access it using WinRM over HTTPS.

Ths script will:
1. Locally, create a self-signed certificate
2. Create a Azure Resource Group
3. Create a Azure Key Vault
4. Upload the self-signed certificate to the Key Vault
5. Create a VM w/ WinRM over HTTPs enabled using the self-signed certificate
6. Output the BaseUrl (```https://{ipAddress}:5986```) for the provisioned VM

Example Usage:
```powershell
Create-VM -SubscriptionId af90f260-8304-49a9-84bf-1784ddb6a845 -ResourceGroupName winrm-rg-demo -Location westus2 -VaultName winrm-kv-demo -VmName winrm-vm-001 -UserName user1 -Password password1
```

To validate your machine is accessible, run [Connect-VM.ps1](scripts/Connect-VM.ps1). The script will attempt to open a Powershell with the remote machine.

Example Usage:
```powershell
Connect-VM -UserName user1 -Password password1 -BaseUrl https://10.10.10.10:5986
```

### Setup machine

Enable WinRM over HTTP and HTTPS with self-signed certificate (includes firewall rules):

```
iex "& { $(irm https://raw.githubusercontent.com/ansible/ansible-documentation/refs/heads/devel/examples/scripts/ConfigureRemotingForAnsible.ps1) }"
```

Enable WinRM over HTTP for test usage (includes firewall rules):

```
winrm quickconfig
```
