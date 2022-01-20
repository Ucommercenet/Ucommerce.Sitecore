using System;
using System.IO;
using System.Linq;
using Helpers;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

[CheckBuildProjectConfigurations]
partial class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    [Parameter("The directory where the Ucommerce Nuget packages can be found")]
    AbsolutePath UcommerceNugetSource
    {
        get => UcommerceNugetSourceField ?? ArtifactsDirectory;
        set => UcommerceNugetSourceField = value;
    }
    AbsolutePath UcommerceNugetSourceField;

    readonly AbsolutePath SitecoreLibsZip = TemporaryDirectory / "sitecore.lib.zip";

    [Parameter] string SitecoreLibsUrl;
    
    // ReSharper disable once UnusedMember.Local
    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });
    
    Target Restore => _ => _
        .Executes(() =>
        {
            NuGetTasks.NuGetRestore(settings => settings.SetTargetPath(Solution));
        });

    // ReSharper disable once UnusedMember.Local
    Target UpdateUcommerceNugets => _ => _
        .Description("Updates the Ucommerce packages to the newest version from a directory on the filesystem")
        .After(Restore)
        .Before(Compile)
        .Executes(() =>
        {
            UcommerceNugetSource.GlobFiles("*").ForEach(path => Logger.Info(path));
            NuGetTasks.NuGet($"update {Solution.Path} -Id Ucommerce.Core -Id Ucommerce -Id Ucommerce.Client.WebForms -source {UcommerceNugetSource} -source nuget.org -Prerelease");
        });

    // ReSharper disable once UnusedMember.Local
    Target DownloadSitecoreLibs => _ => _
        .Description("Internal target used by pipelines, see README.md file for how to get sitecore DLLs into libs")
        .Before(RestoreSitecoreLibs)
        .Executes(() =>
        {
            DeleteFile(SitecoreLibsZip);
            HttpTasks.HttpDownloadFile(SitecoreLibsUrl, SitecoreLibsZip);
        });
    
    // ReSharper disable once UnusedMember.Local
    Target RestoreSitecoreLibs => _ => _
        .Description("Internal target used by pipelines, see README.md file for how to get sitecore DLLs into libs")
        .Before(Compile)
        .Executes(() =>
        {
            var sitecoreLibsDir = RootDirectory / "lib" / "Sitecore";
            EnsureCleanDirectory(sitecoreLibsDir);
            CompressionTasks.UncompressZip(SitecoreLibsZip, sitecoreLibsDir);
        });
    
    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            MSBuild(s => s
                .SetTargetPath(Solution)
                .SetTargets("Rebuild")
                .SetConfiguration(Configuration)
                .SetProcessArgumentConfigurator(a => a
                    .Add("/p:PostBuildEvent=")
                )
                .SetNodeReuse(IsLocalBuild));
        });

    Target Package => _ => _
        .DependsOn(Compile)
        .Description("Packages the files as a Sitecore package")
        .Executes(() =>
        {
            var workDir = TemporaryDirectory / "sitecore";
            EnsureCleanDirectory(workDir);
            var filesDir = workDir / "files";
            var installerDir = workDir / "installer";
            var metadataDir = workDir / "metadata";
            
            var shellDir = filesDir / "sitecore modules" / "Shell";
            var ucommerceDir = shellDir / "Ucommerce";
            var binDir = filesDir / "bin";
            

            // ReSharper disable once PossibleNullReferenceException
            var installerProject = Solution.GetProject("Ucommerce.Sitecore.Installer");
            var installerProjectDir = installerProject.Directory;
            var installerPackageDir = installerProjectDir / "package";

            var webProject = Solution.GetProject("Ucommerce.Sitecore.Web");
           
            CopyDirectoryRecursively(installerPackageDir / "installer", installerDir);
            CopyDirectoryRecursively(installerPackageDir / "metadata", metadataDir);
            //Update the sitecore package info with the version we are building.
            TextTasks.WriteAllText(metadataDir / "sc_version.txt", FullVersion);
            TextTasks.WriteAllText(metadataDir / "sc_name.txt", $"Ucommerce {FullVersion}");
            
            CopyDirectoryRecursively(installerPackageDir / "Files", filesDir );

            //////////
            // Bins //
            //////////
            
            installerProject.GetOutputDir(Configuration)
                .GlobFiles("Ucommerce.Installer.dll", "Ucommerce.Sitecore.Installer.dll")
                .Append(RootDirectory / "lib" / "XmlTransform"/ "Microsoft.Web.XmlTransform.dll")
                .ForEach(path => CopyFileToDirectory(path, binDir));
            
            // Fake Assemblies for update
            new [] {"Ucommerce.Admin.dll", "Ucommerce.systemWeb.dll", "Ucommerce.Pipelines.dll"}
                .ForEach(fileName => File.Create(binDir / fileName));
            
            ////////////
            // Shell  //
            ////////////

            var packagesDir = SourceDirectory / "packages";
            // We get the last client package since updating the package will leave the old version still in the folder 
            var clientPackagePath = packagesDir.GlobDirectories("Ucommerce.Client.WebForms*").Last();
            CopyDirectoryRecursively(
                clientPackagePath / "UcommerceFiles", 
                shellDir, DirectoryExistsPolicy.Merge,
                excludeDirectory: directoryInfo => 
                    new [] {"umbraco5", "umbraco6", "umbraco7", "umbraco8", "sitefinity"}.Contains(directoryInfo.Name, StringComparer.CurrentCultureIgnoreCase),
                excludeFile: fileInfo =>
                {
                    return new []
                               {
                                   "UCommerce6.js", "baskets.addaddress.sitecore.config.default", "settingsmanager.aspx"
                               }
                               .Contains(fileInfo.Name, StringComparer.InvariantCultureIgnoreCase) ||
                           fileInfo.Name.Contains("umbraco", StringComparison.InvariantCultureIgnoreCase) ||
                           fileInfo.Name.Contains("sitefinity", StringComparison.InvariantCultureIgnoreCase) ||
                           fileInfo.Name.Contains("kentico", StringComparison.InvariantCultureIgnoreCase);
                });

            var webShellDir = webProject.Directory / "Shell"; 
            webShellDir
                .GlobFiles(
                "OrderManager.aspx",
                "PromotionManager.aspx"
                )
                .ForEach(path => CopyFileToDirectory(path, ucommerceDir / "shell"));
            
            CopyFile(webShellDir / "Scripts" / "Sitecore.js", ucommerceDir / "shell" / "app" / "constants.js");

            var webProjectsAppsDir = webProject.Directory / "Apps"; 
            webProjectsAppsDir
                .GlobDirectories("*")
                .ForEach(path => CopyDirectoryRecursively(path,  ucommerceDir / "Apps" / webProjectsAppsDir.GetRelativePathTo(path)));
            
            CopyFileToDirectory(webProject.Directory / "Configuration" / "Shell.config.default", ucommerceDir / "Configuration");

            CopyDirectoryRecursively(
                webProject.Directory / "Css", 
                ucommerceDir / "Css", 
                DirectoryExistsPolicy.Merge,
                excludeFile:info => info.Name.Contains(".less"));

            ///////////////////////
            // Ucommerce Install //
            ///////////////////////
            var ucommerceInstallDir = ucommerceDir / "install";
            
            CopyDirectoryRecursively(installerProjectDir / "SpeakSerialization", ucommerceInstallDir / "SpeakSerialization");
            
            CopyDirectoryRecursively(installerProjectDir / "ConfigurationTransformations", ucommerceInstallDir, DirectoryExistsPolicy.Merge);
            RenameDirectory(ucommerceInstallDir / "ConfigIncludes", "configinclude");
            
            CopyDirectoryRecursively(RootDirectory / "database", ucommerceInstallDir, DirectoryExistsPolicy.Merge);
            
            Solution.GetProject("Ucommerce.Sitecore").GetOutputDir(Configuration)
                .GlobFiles(
                    "Ucommerce*.dll",
                    "castle.core.dll",
                    "castle.windsor.dll",
                    "clientdependency.core.dll",
                    "Castle.Facilities.AspNet.SystemWeb.dll",
                    "csvhelper.dll", 
                    "epplus.dll", 
                    "fluentnhibernate.dll", 
                    "iesi.collections.dll", 
                    "infralution.licensing.dll", 
                    "log4net.dll", 
                    "lucene.net.dll", 
                    "microsoft.web.xmltransform.dll",
                    "newtonsoft.json.dll", 
                    "nhibernate.caches.syscache.dll", 
                    "nhibernate.dll", 
                    "Antlr3.Runtime.dll", 
                    "Remotion.Linq.dll",
                    "Remotion.Linq.EagerFetching.dll", 
                    "FluentValidation.dll", 
                    "Slugify.Core.dll", 
                    "Dapper.dll", 
                    "J2N.dll", 
                    "System.Linq.Dynamic.dll")
                .Where(path =>
                {
                    if (Configuration == Configuration.Debug)
                    {
                        return true;
                    }
                    var extension = Path.GetExtension(path.ToString()); 
                    return extension != ".pdb" && extension != ".xml";
                })
                .Where(path => !path.ToString().Contains("Ucommerce.Installer.dll"))
                // ReSharper disable once PossibleNullReferenceException
                .Append(webProject.Directory / "bin"  / "Ucommerce.Sitecore.Web.dll")
                .Append(RootDirectory / "lib" / "Lucene.net" / "Lucene.net.dll")
                .ForEach(path => CopyFileToDirectory(path, ucommerceInstallDir / "binaries", FileExistsPolicy.Overwrite));
            
            //////////////////////////
            // Commerce Connect APP //
            //////////////////////////
            CopyDirectoryRecursively(SourceDirectory / "Ucommerce.Sitecore.CommerceConnect" / "Configuration", 
                ucommerceDir / "Apps" / "sitecore commerce connect.disabled");
            
            RenameFile(ucommerceDir / "configuration" / "settings" / "settings.sitecore.config.default", "settings.config.default");
            
            CopyDirectoryRecursively(webProject.Directory / "Pipelines", ucommerceDir / "Pipelines", DirectoryExistsPolicy.Merge, FileExistsPolicy.Overwrite);


            ////////////////////////
            // Compatibility Apps //
            ////////////////////////
            CopyFileToDirectory(
                Solution.GetProject("Ucommerce.Sitecore92").GetOutputDir(Configuration) / "Ucommerce.Sitecore92.dll", 
                ucommerceDir / "apps" / "Sitecore92compatibility.disabled" / "bin");
            CopyFileToDirectory(
                Solution.GetProject("Ucommerce.Sitecore93").GetOutputDir(Configuration) / "Ucommerce.Sitecore93.dll", 
                ucommerceDir / "apps" / "Sitecore93compatibility.disabled" / "bin");
            
            // The internal package.zip file
            CompressionTasks.CompressZip(workDir, TemporaryDirectory / "package" / "package.zip", fileMode: FileMode.Create);
            // The versioned Sitecore package.
            CompressionTasks.CompressZip(TemporaryDirectory / "package",ArtifactsDirectory / $"Ucommerce-for-Sitecore-{FullVersion}.zip", fileMode: FileMode.Create);
        });

}
