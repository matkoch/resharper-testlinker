Setup(context => TeamCity.SetBuildNumber(FullSemVer));


////////////////////////////
// COMPILE
////////////////////////////
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


////////////////////////////
// ANALYSIS
////////////////////////////
Task("InspectCode")
  .IsDependentOn("Restore")
  .Does(() =>
{
  foreach (var inspectCodePlugin in InspectCodePlugins)
  {
    var packageFile = InspectCodeDirectory.CombineWithFilePath(inspectCodePlugin + ".nupkg");
    if (!FileExists(packageFile))
      DownloadFile("https://resharper-plugins.jetbrains.com/api/v2/package/" + inspectCodePlugin, packageFile);
  }

  InspectCode(SolutionFile, new InspectCodeSettings {
    SolutionWideAnalysis = true,
    OutputFile = InspectCodeResultFile,
    CachesHome = InspectCodeCacheHome
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


////////////////////////////
// PACKAGES
////////////////////////////
Task("Packages")
  .IsDependentOn("Clean")
  .IsDependentOn("EnsureBinaries")
  .ContinueOnError()
  .Does(() =>
{
  foreach (var packageSetting in PackageSettings)
    NuGetPack(packageSetting);

  var pushSettings = new NuGetPushSettings { Verbosity = NuGetVerbosity.Detailed };
  foreach (var packageFile in GetFiles(NuGetPackageFileGlob))
  {
    if (packageFile.Segments.Last().StartsWith("ReSharper"))
    {
      pushSettings.ApiKey = ReSharperGalleryApiKey;
      pushSettings.Source = ReSharperGallerySource;
    }
    else if (IsDefaultBranch)
    {
      pushSettings.ApiKey = NuGetApiKey;
      pushSettings.Source = NuGetSource;
    }
    else
    {
      pushSettings.ApiKey = MyGetApiKey;
      pushSettings.Source = MyGetSource;
    }
    
    CatchAll(() => NuGetPush (packageFile, pushSettings));
  }
});


////////////////////////////
// RELEASE
////////////////////////////
//Task("Tag")
//  .ContinueOnError()
//  .Does(() =>
//{
//  CatchAll(() => GitTag(SolutionDirectory, FullSemVer));
//  CatchAll(() => GitPushRef(SolutionDirectory, GitHubUsername, GitHubPassword, "origin", FullSemVer));
//});
//
//Task("ReleaseNotes")
//  .ContinueOnError()
//  .Does(() =>
//{
//  GitReleaseNotes(SolutionDirectory.CombineWithFilePath("CHANGELOG.md"),
//    new GitReleaseNotesSettings {
//      WorkingDirectory = SolutionDirectory,
//      Categories       = "breaking, feature, improvement, bug",
//      Version          = FullSemVer,
//      AllLabels        = false,
//      Verbose          = true
//    });
//
//  GitAddAll(SolutionDirectory);
//  GitCommit(SolutionDirectory, "Botty", "ithrowexceptions@gmail.com", "Update CHANGELOG.md");
//
//  GitReleaseManagerCreate(GitHubUsername, GitHubPassword, "matkoch", "TestLinker2",
//    new GitReleaseManagerCreateSettings {
//        Name            = FullSemVer,
//        InputFilePath   = "CHANGELOG.md",
//        Prerelease      = false,
//        TargetCommitish = BranchName,
//        TargetDirectory = SolutionDirectory
//    });
//GitReleaseManagerPublish(GitHubUsername, GitHubPassword, "matkoch", "TestLinker2", FullSemVer);
//  //GitReleaseManagerExport(GitHubUsername, GitHubPassword, "matkoch", "TestLinker2", "releasemanager.md",
//  //new GitReleaseManagerExportSettings {
//  //    ArgumentCustomization = Log,
//  //    TagName           = FullSemVer,
//  //    TargetDirectory   = SolutionDirectory,
//  //    WorkingDirectory  = SolutionDirectory,
//  //    LogFilePath       = SolutionDirectory.CombineWithFilePath("log.log")
//  //});
//});



////////////////////////////
// BUMP VERSION
////////////////////////////
//Task("BumpVersion")
//  .Does(() =>
//{
//  var bump = EnvironmentVariable("bump");
//  if (bump == null) throw new Exception("Value for 'bump' not set.");
//
//  var git = Context.Tools.Resolve("git.exe") ?? "git";
//
//  var args = String.Format("commit --allow-empty --author \"Robot <ithrowexceptions+build@gmail.com>\" -m \"+semver: {0}\"", bump);
//  StartProcess(git, new ProcessSettings { Arguments = args });
//
//  args = String.Format("remote set-url origin https://github.com/matkoch/TestLinker2");
//  StartProcess(git, new ProcessSettings { Arguments = args });
//  GitPush(SolutionDirectory, GitHubUsername, GitHubPassword);
//});


////////////////////////////
// HELPER
////////////////////////////

public void CatchAll(Action action)
{
  try
  {
    action();
  }
  catch (Exception e)
  {
    Error(e.Message);
  }
}


ProcessArgumentBuilder Log(ProcessArgumentBuilder bla)
{
  Warning(bla.Render());
  return bla;
}


Task("Dry");
