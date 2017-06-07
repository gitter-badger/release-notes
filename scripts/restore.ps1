[Parameter()][string]$SolutionPath

if (!$SolutionPath) {
    $SolutionPath = $cwd
}

if (!(Test-Path $SolutionPath -PathType Container)) {
    throw "Path not found"
}

Push-Location $SolutionPath
try {
    Get-ChildItem *.*proj -File -Recurse | ForEach-Object {
        Push-Location $_.Directory.FullName
        try {
            . dotnet restore
        } finally {
            Pop-Location
        }
    } 
} finally {
    Pop-Location
}