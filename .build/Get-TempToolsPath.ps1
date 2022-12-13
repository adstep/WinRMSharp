$path = "$PSScriptRoot\..\.tools"

if (!(Test-Path $path)) {
    New-Item -ItemType Directory -Path $Path | Out-Null
}

(Resolve-Path $path).Path