TeamCity.SetBuildNumber(FullSemVer);

Task("Clean")
  .Does(() => CleanDirectory(OutputDirectory));

Task("Restore")
  .Does(() => NuGetRestore(SolutionFile));

Task("Compile")
  .IsDependentOn("Clean")
  .IsDependentOn("Restore")
  .Does(() =>
{

  var settings = new MSBuildSettings {
      ToolVersion = MSBuildToolVersion.VS2015,
      MaxCpuCount = 0,
      NodeReuse = false,
      Configuration = Configuration,
      PlatformTarget = PlatformTarget.MSIL
    };

  if (!IsLocalBuild)
    settings.SetVerbosity(Verbosity.Normal);
  else
    settings
      .SetVerbosity(Verbosity.Quiet)
      .AddFileLogger(
        new MSBuildFileLogger {
            Verbosity = Verbosity.Normal,
            LogFile = BuildLogFile.FullPath
          });

  MSBuild(SolutionFile, settings);
  //Zip(SolutionDirectory, ArtifactFile, ArtifactGlob);
});

Task("EnsureBinaries")
  .Does(() =>
{
  var binaries = GetFiles(ArtifactGlob);
  if (binaries.Any())
  {
    Information("Existing binaries:");
    foreach (var binary in binaries)
      Information(" - " + binary.FullPath);
  }
  else
  {
    Information("No binaries found.");
    RunTarget("Compile");
  }
});

//Task("UnzipBinaries")
//  .ContinueOnError()
//  .Does(() => Unzip(ArtifactFile, SolutionDirectory));


Task("InspectCode")
  .Does(() =>
{
  InspectCode(SolutionFile, new InspectCodeSettings {
    SolutionWideAnalysis = true,
    OutputFile = InspectCodeResultFile
  });

  TeamCity.ImportData("ReSharperInspectCode", InspectCodeResultFile);
});

Task("FxCop")
  .IsDependentOn("Restore")
  .Does(() =>
{
  MSBuild(SolutionFile, new MSBuildSettings {
      ToolVersion = MSBuildToolVersion.VS2015,
      MaxCpuCount = 0,
      NodeReuse = false,
      Configuration = Configuration,
      PlatformTarget = PlatformTarget.MSIL,
    }.WithProperty("RunCodeAnalysis", new[]{ "true" })); // PR: params + copy

  var codeAnalysisFiles = GetFiles(CodeAnalysisResultGlob);
  //CopyFiles(codeAnalysisFiles, OutputDirectory);
  foreach (var codeAnalysisFile in codeAnalysisFiles)
    TeamCity.ImportData("FxCop", codeAnalysisFile);
});

Task("Pack")
  .IsDependentOn("Clean")
  .IsDependentOn("EnsureBinaries")
  .Does(() =>
{
  foreach (var packageSetting in PackageSettings)
    NuGetPack(packageSetting);
});

Task("Publish")
  .IsDependentOn("Pack")
  .Does(() =>
{
  var pushSettings = new NuGetPushSettings { Verbosity = NuGetVerbosity.Detailed };
  foreach (var nupkgFile in GetFiles(NupkgGlob))
  {
    if (!IsMasterBranch)
    {
      pushSettings.ApiKey = MyGetApiKey;
      pushSettings.Source = MyGetSourceUrl;
    }
    else if (nupkgFile.Segments.Last().StartsWith("ReSharper"))
    {
      pushSettings.ApiKey = ReSharperApiKey;
      pushSettings.Source = ReSharperSourceUrl;
    }
    else
    {
      pushSettings.ApiKey = NuGetApiKey;
      pushSettings.Source = NuGetSourceUrl;
    }

	  NuGetPush (nupkgFile, pushSettings);
  }
});

Task("Dry");
