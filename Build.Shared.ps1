$SolutionFile     = "TestLinker.sln"
$SourceDir        = "src"
$NuSpecDir        = "nuspec"
$OutputDir        = "output"

$CoverageFile     = Join-Path $SourceDir "coverage.xml"
$AssemblyInfoFile = Join-Path $SourceDir "AssemblyInfoShared.cs"

[array] `
$TestAssemblies   = @() | %{ Join-Path $SourceDir "$_\bin\$Configuration\$_.dll" }

[array] `
$NuSpecFiles      = @("TestFx.TestLinker.nuspec") | %{ Join-Path $NuSpecDir $_ }