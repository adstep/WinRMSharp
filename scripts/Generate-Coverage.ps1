Push-Location

dotnet test "$PSScriptRoot/../src/WinRMSharp.Tests/" --framework net7.0 --filter "Category!=Integration" --collect:"XPlat Code Coverage"

$coverageFiles = Get-ChildItem -Path "$PSScriptRoot/../**/coverage.cobertura.xml" -Recurse
$coverageFiles = $coverageFiles | Sort-Object -Property LastWriteTime -Descending

$latestCoverageFile = $coverageFiles[0]

reportgenerator.exe -reports:$latestCoverageFile -targetdir:"$PSScriptRoot/../coveragereport" -reporttypes:Html

Invoke-Expression "$PSScriptRoot/../coveragereport\index.html"
