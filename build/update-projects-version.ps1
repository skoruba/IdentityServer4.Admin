$oldVersion = "<Version>1.0.0-rc2</Version>"
$newVersion = "<Version>1.0.0-rc2-update1</Version>"

$templateFiles = Get-ChildItem .\..\src -include *.csproj -Recurse
foreach ($file in $templateFiles) {
    Write-Host $file.PSPath

    (Get-Content $file.PSPath -raw -Encoding UTF8) |
    Foreach-Object { $_ -replace $oldVersion, $newVersion } |
    Set-Content $file.PSPath -Encoding UTF8
}