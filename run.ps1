dotnet test .\src\WinRMSharp.Tests\ --framework net7.0 --collect:"XPlat Code Coverage"

$coverageFiles = Get-ChildItem -Path "$PSScriptRoot/**/coverage.cobertura.xml" -Recurse
$coverageFiles = $coverageFiles | Sort-Object -Property LastWriteTime -Descending

$latestCoverageFile = $coverageFiles[0]

reportgenerator.exe -reports:$latestCoverageFile -targetdir:"coveragereport" -reporttypes:Html