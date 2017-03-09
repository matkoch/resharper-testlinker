////////////////////////////
// TOOLS
////////////////////////////
#tool "nuget:?package=JetBrains.ReSharper.CommandLineTools"
#tool "nuget:?package=GitVersion.CommandLine&prerelease"
//#tool "nuget:?package=gitreleasemanager"
//#tool "nuget:?package=GitReleaseNotes"
//#tool "nuget:?package=Git"
//#tool "nuget:?package=NUnit.Runners&version=2.6.4"
//#tool "nuget:?package=JetBrains.dotCover.CommandLineTools"
//#tool "nuget:?package=OpenCover"
//#addin "nuget:?package=Cake.Git"


////////////////////////////
// INCLUDES
////////////////////////////
#load .\tasks.cake


////////////////////////////
// PARAMETERS
////////////////////////////
var Target          = Argument<string>("target", "Default");
var Configuration   = Argument<string>("configuration", "Release");
var IsLocalBuild    = BuildSystem.IsLocalBuild;
var Version         = GitVersion(new GitVersionSettings { UpdateAssemblyInfo = !IsLocalBuild });
var BranchName      = Version.BranchName;
var FullSemVer      = Version.FullSemVer;
var Sha             = Version.Sha;
var DefaultBranch   = "master";
var IsDefaultBranch = BranchName == DefaultBranch;
var NuGetVersionV2  = Version.NuGetVersionV2 + (IsLocalBuild ? "-local" : "");


////////////////////////////
// PATHS
////////////////////////////
var SolutionDirectory      = Context.Environment.WorkingDirectory;
var SolutionFile           = SolutionDirectory.CombineWithFilePath("TestLinker.sln");
var OutputDirectory        = SolutionDirectory.Combine("output");
var SourceDirectory        = SolutionDirectory.Combine("src");
var BuildLogFile           = OutputDirectory.CombineWithFilePath("build.log");
var InspectCodeResultFile  = OutputDirectory.CombineWithFilePath("inspectcode.xml");
var InspectCodeCacheHome   = SolutionDirectory.Combine("_ReSharper.InspectionCache");
var CodeAnalysisResultGlob = SourceDirectory + "/**/bin/" + Configuration + "/*.CodeAnalysisLog.xml";
var ArtifactFile           = OutputDirectory.CombineWithFilePath("artifacts.zip");
var ArtifactGlob           = SourceDirectory + "/**/bin/" + Configuration + "/*";
var InspectCodeDirectory   = SolutionDirectory.Combine("tools/JetBrains.ReSharper.CommandLineTools/tools");
var NuGetPackageFileGlob   = OutputDirectory + "/*.nupkg";


////////////////////////////
// DOWNLOADS
////////////////////////////
var InspectCodePlugins = new []
    {
      "ReSharper.ImplicitNullability",
      "ReSharper.XmlDocInspections",
      "ReSharper.SerializationInspections",
      "PowerToys.CyclomaticComplexity",
      "EtherealCode.ReSpeller"
    };


////////////////////////////
// SERVICES
////////////////////////////
var NuGetSource            = "https://www.nuget.org/api/v2/package";
var NuGetApiKey            = EnvironmentVariable("NuGetApiKey");
var MyGetSource            = "https://www.myget.org/F/matkoch/api/v2/package";
var MyGetApiKey            = EnvironmentVariable("MyGetApiKey");
var ReSharperGallerySource = "https://resharper-plugins.jetbrains.com/api/v2/package";
var ReSharperGalleryApiKey = EnvironmentVariable("ReSharperApiKey");
var GitHubUsername         = "matkoch-build";
var GitHubPassword         = EnvironmentVariable("GitHubPassword");

////////////////////////////
// PACKAGES
////////////////////////////
var WaveVersion = XmlPeek(SourceDirectory.CombineWithFilePath("./TestLinker/packages.config"), "/packages/package[@id='Wave']/@version");
var PackageSettings = new[]
    {
      new NuGetPackSettings {
        Version                  = NuGetVersionV2,
        Authors                  = new[] { "Matthias Koch" },
        Owners                   = new[] { "matkoch" },
        Copyright                = "Copyright (c) Matthias Koch, 2014-2017",
        BasePath                 = SourceDirectory,
        OutputDirectory          = OutputDirectory,
        Symbols                  = false,
        NoPackageAnalysis        = true,
        RequireLicenseAcceptance = false,

        Id           = "ReSharper.TestLinker",
        Title        = "TestLinker",
        Description  = "Linking production and test code.",
        Tags         = new[] { "test", "linker", "nunit", "mspec", "xunit", "mstest" },
        ProjectUrl   = new Uri("https://github.com/matkoch/TestLinker/"),
        LicenseUrl   = new Uri("https://raw.github.com/matkoch/TestLinker/master/LICENSE"),
        IconUrl      = new Uri("https://raw.githubusercontent.com/matkoch/TestLinker/master/TestLinker.ico"),
        Files        = new[] { new NuSpecContent { Target = "DotFiles", Source = "TestLinker\\bin\\" + Configuration + "\\TestLinker.dll" } },
        Dependencies = new[] { new NuSpecDependency { Id = "Wave", Version = "[" + WaveVersion + "]" } }
      }
    };


////////////////////////////
// ENTRY TASKS
////////////////////////////
Task("Default")
  .Does(() =>
{
});

RunTarget(Target);