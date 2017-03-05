////////////////////////////
// VARIABLES
////////////////////////////

Information("========================================");
Information("Parameters");
Information("========================================");

var Target                    = Argument<string>("target");
var Configuration             = Argument<string>("configuration");
var IsLocalBuild              = BuildSystem.IsLocalBuild;
var Version                   = GitVersion(new GitVersionSettings { UpdateAssemblyInfo = !IsLocalBuild });
var BranchName                = Version.BranchName;
var FullSemVer                = Version.FullSemVer;
var Sha                       = Version.Sha;
var IsMasterBranch            = BranchName == "master";

Information("Target                     = " + Target);
Information("Configuration              = " + Configuration);
Information("IsLocalBuild               = " + IsLocalBuild);
Information("BranchName                 = " + BranchName);
Information("FullSemVer                 = " + FullSemVer);
Information("Sha                        = " + Sha);

Information("----------------------------------------");

var SolutionDirectory         = Context.Environment.WorkingDirectory;
var SolutionFile              = SolutionDirectory.CombineWithFilePath("TestLinker.sln");
var OutputDirectory           = SolutionDirectory.Combine("output");
var SourceDirectory           = SolutionDirectory.Combine("src");
var BuildLogFile              = OutputDirectory.CombineWithFilePath("build.log");
var InspectCodeResultFile     = OutputDirectory.CombineWithFilePath("inspectcode.xml");
var InspectCodeCacheHome      = SolutionDirectory.Combine("_ReSharper.InspectionCache");
var CodeAnalysisResultGlob    = SourceDirectory + "/**/bin/" + Configuration + "/*.CodeAnalysisLog.xml";
var ArtifactFile              = OutputDirectory.CombineWithFilePath("artifacts.zip");
var ArtifactGlob              = SourceDirectory + "/**/bin/" + Configuration + "/*";
var InspectCodeDirectory      = SolutionDirectory.Combine("tools/JetBrains.ReSharper.CommandLineTools/tools");
var InspectCodePlugins        = new []
                                {
                                  "ReSharper.ImplicitNullability",
                                  "ReSharper.XmlDocInspections",
                                  "ReSharper.SerializationInspections",
                                  "PowerToys.CyclomaticComplexity",
                                  "EtherealCode.ReSpeller"
                                };

Information("SolutionDirectory          = " + SolutionDirectory);
Information("SolutionFile               = " + SolutionFile);
Information("SourceDirectory            = " + SourceDirectory);
Information("OutputDirectory            = " + OutputDirectory);
Information("BuildLogFile               = " + BuildLogFile);
Information("InspectCodeResultFile      = " + InspectCodeResultFile);
Information("InspectCodeCacheHome       = " + InspectCodeCacheHome);
Information("InspectCodeDirectory       = " + InspectCodeDirectory);
for (var i = 0; i < InspectCodePlugins.Length; i++)
  Information("InspectCodePlugins[" + i + "]      = " + InspectCodePlugins[i]);
Information("CodeAnalysisResultGlob     = " + CodeAnalysisResultGlob);
Information("ArtifactFile               = " + ArtifactFile);
Information("ArtifactGlob               = " + ArtifactGlob);

Information("----------------------------------------");

var NuGetApiKey               = EnvironmentVariable("NuGetApiKey");
var NuGetSourceUrl            = "https://www.nuget.org/api/v2/package";
var MyGetApiKey               = EnvironmentVariable("MyGetApiKey");
var MyGetSourceUrl            = "https://www.myget.org/F/matkoch/api/v2/package";
var ReSharperApiKey           = EnvironmentVariable("ReSharperApiKey");
var ReSharperSourceUrl        = "https://resharper-plugins.jetbrains.com/api/v2/package";

Information("NuGetApiKey                = " + (NuGetApiKey != null ? "<set>" : "<not set>"));
Information("NuGetSourceUrl             = " + NuGetSourceUrl);
Information("MyGetApiKey                = " + (MyGetApiKey != null ? "<set>" : "<not set>"));
Information("MyGetSourceUrl             = " + MyGetSourceUrl);
Information("ReSharperApiKey            = " + (ReSharperApiKey != null ? "<set>" : "<not set>"));
Information("ReSharperSourceUrl         = " + ReSharperSourceUrl);

Information("----------------------------------------");

var NuGetVersionV2            = Version.NuGetVersionV2 + (IsLocalBuild ? "-local" : "");
var NupkgGlob                 = OutputDirectory + "/*.nupkg";
var WaveVersion               = XmlPeek(SourceDirectory.CombineWithFilePath("./TestLinker/packages.config"),
                                        "/packages/package[@id='Wave']/@version");
var PackageSettings = new[] {
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

Information("NuGetVersionV2             = " + NuGetVersionV2);
for (var i = 0; i < PackageSettings.Length; i++)
{
  Information("Package[" + i + "].Id              = " + PackageSettings[i].Id);
}
Information("NupkgGlob                  = " + NupkgGlob);
Information("WaveVersion                = " + WaveVersion);

////////////////////////////
// TOOLS
////////////////////////////

#tool "nuget:?package=JetBrains.ReSharper.CommandLineTools"
#tool "nuget:?package=GitVersion.CommandLine"
//#tool "nuget:?package=NUnit.Runners&version=2.6.4"
//#tool "nuget:?package=JetBrains.dotCover.CommandLineTools"
//#tool "nuget:?package=OpenCover"

////////////////////////////
// TASKS
////////////////////////////

#load .\tasks.cake

Task("Default")
  .Does(() =>
{
});


RunTarget(Target);