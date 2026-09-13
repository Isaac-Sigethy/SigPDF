$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
dotnet build "$root\PdfEditor.sln"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet test "$root\PdfEditor.sln" --no-build
exit $LASTEXITCODE
