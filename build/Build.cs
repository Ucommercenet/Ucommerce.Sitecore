using System;
using System.IO;
using System.Linq;
using Helpers;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

partial class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Compile);

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
    readonly AbsolutePath WorkDir = TemporaryDirectory / "sitecore";

    // ReSharper disable once UnusedMember.Local
    // ReSharper disable once MissingLinebreak
    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory
                .GlobDirectories("Ucommerce*/bin", "Ucommerce*/obj")
                .ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
            EnsureCleanDirectory(WorkDir);
        });

    // ReSharper disable once MissingLinebreak
    Target Restore => _ => _
        .Executes(() =>
        {
            NuGetTasks.NuGetRestore(settings => settings.SetTargetPath(Solution));
        });

    // ReSharper disable once UnusedMember.Local
    // ReSharper disable once MissingLinebreak
    // ReSharper disable once IdentifierTypo
    Target UpdateUcommerceNugets => _ => _
        .Description("Updates the Ucommerce packages to the newest version from a directory on the filesystem")
        .After(Restore)
        .Before(Compile)
        .Executes(() =>
        {
            UcommerceNugetSource
                .GlobFiles("*")
                .ForEach(path => Log.Information(path));
            NuGetTasks.NuGet($"update {Solution.Path} -Id Ucommerce.Core -Id Ucommerce -Id Ucommerce.Client.WebForms -source {UcommerceNugetSource} -source nuget.org -Prerelease");
        });

    // ReSharper disable once UnusedMember.Local
    // ReSharper disable once MissingLinebreak
    Target DownloadSitecoreLibs => _ => _
        .Description("Internal target used by pipelines, see README.md file for how to get sitecore DLLs into libs")
        .Before(RestoreSitecoreLibs)
        .Executes(() =>
        {
            DeleteFile(SitecoreLibsZip);
            HttpTasks.HttpDownloadFile(SitecoreLibsUrl, SitecoreLibsZip);
        });

    // ReSharper disable once UnusedMember.Local
    // ReSharper disable once MissingLinebreak
    Target RestoreSitecoreLibs => _ => _
        .Description("Internal target used by pipelines, see README.md file for how to get sitecore DLLs into libs")
        .Before(Compile)
        .Executes(() =>
        {
            var sitecoreLibsDir = RootDirectory / "lib" / "Sitecore";
            EnsureCleanDirectory(sitecoreLibsDir);
            CompressionTasks.UncompressZip(SitecoreLibsZip, sitecoreLibsDir);
        });

    // ReSharper disable once MissingLinebreak
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

    Target CreatePostInstallPackage =>
        _ => _
            .DependsOn(Clean)
            .Executes(() =>
            {
                var rootDir = WorkDir / "PostInstallPackage";
                var installerDir = rootDir / "installer";
                var metadataDir = rootDir / "metadata";
                var installerProject = Solution.GetProject("Ucommerce.Sitecore.Installer");
                // ReSharper disable once PossibleNullReferenceException
                var installerProjectDir = installerProject.Directory;
                var installerPackageDir = installerProjectDir / "package";

                CopyDirectoryRecursively(installerPackageDir / "installer", installerDir);
                CopyDirectoryRecursively(installerPackageDir / "metadata", metadataDir);
                //Update the sitecore package info with the version we are building.
                TextTasks.WriteAllText(metadataDir / "sc_version.txt", FullVersion);
                TextTasks.WriteAllText(metadataDir / "sc_name.txt", $"Ucommerce {FullVersion}");

                // The internal package.zip file
                CompressionTasks.CompressZip(rootDir, WorkDir / "package.zip", fileMode: FileMode.Create);

                // The versioned PostInstall Sitecore package.
                });

    Target CreateVersionedPostInstallPackage =>
        _ => _
            .DependsOn(CreatePostInstallPackage)
            .Executes(() =>
            {
                var rootDir = WorkDir / "VersionedPostInstallPackage";

                CopyFileToDirectory(WorkDir / "package.zip", rootDir);
                CompressionTasks.CompressZip(
                    rootDir,
                    WorkDir / $"Ucommerce-PostInstall-{FullVersion}.zip",
                    fileMode: FileMode.Create);
            });

    // ReSharper disable once MissingLinebreak
    // ReSharper disable once UnusedMember.Local
    Target CreateUcommerceInstaller => _ => _
        .DependsOn(Compile)
        .DependsOn(CreateVersionedPostInstallPackage)
        .Description("Packages the files as a Sitecore package")
        .Executes(() =>
        {
            var rootDir = WorkDir / "Installer";
            var filesDir = rootDir / "files";

            var shellDir = filesDir / "sitecore modules" / "Shell";
            var ucommerceDir = shellDir / "Ucommerce";
            var binDir = filesDir / "bin";

            var installerProject = Solution.GetProject("Ucommerce.Sitecore.Installer");
            // ReSharper disable once PossibleNullReferenceException
            var installerProjectDir = installerProject.Directory;
            var installerPackageDir = installerProjectDir / "package";

            CopyDirectoryRecursively(installerPackageDir / "Files", filesDir);

            //////////
            // Bins //
            //////////

            installerProject.GetOutputDir(Configuration)
                .GlobFiles("Ucommerce.Installer.dll", "Ucommerce.Sitecore.Installer.dll")
                .Append(RootDirectory / "lib" / "XmlTransform" / "Microsoft.Web.XmlTransform.dll")
                .ForEach(path => CopyFileToDirectory(path, binDir));

            // Fake Assemblies for update
            new[] { "Ucommerce.Admin.dll", "Ucommerce.systemWeb.dll", "Ucommerce.Pipelines.dll" }
                .ForEach(fileName => File.Create(binDir / fileName));

            ////////////
            // Shell  //
            ////////////

            var packagesDir = SourceDirectory / "packages";
            // We get the last client package since updating the package will leave the old version still in the folder
            var clientPackagePath = packagesDir
                                        .GlobDirectories("Ucommerce.Client.WebForms*")
                                        .Last();
            CopyDirectoryRecursively(
                clientPackagePath / "UcommerceFiles",
                shellDir, DirectoryExistsPolicy.Merge,
                excludeDirectory: directoryInfo =>
                    new[] { "umbraco5", "umbraco6", "umbraco7", "umbraco8", "sitefinity" }.Contains(directoryInfo.Name, StringComparer.CurrentCultureIgnoreCase),
                excludeFile: fileInfo =>
                {
                    return new[]
                               {
                                   // ReSharper disable StringLiteralTypo
                                   "UCommerce6.js", "baskets.addaddress.sitecore.config.default", "settingsmanager.aspx"
                                   // ReSharper restore StringLiteralTypo
                               }
                               .Contains(fileInfo.Name, StringComparer.InvariantCultureIgnoreCase) ||
                           fileInfo.Name.Contains("umbraco", StringComparison.InvariantCultureIgnoreCase) ||
                           fileInfo.Name.Contains("sitefinity", StringComparison.InvariantCultureIgnoreCase) ||
                           fileInfo.Name.Contains("kentico", StringComparison.InvariantCultureIgnoreCase);
                });

            var webProject = Solution.GetProject("Ucommerce.Sitecore.Web");
            // ReSharper disable once PossibleNullReferenceException
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
                .ForEach(path => CopyDirectoryRecursively(path, ucommerceDir / "Apps" / webProjectsAppsDir.GetRelativePathTo(path)));

            CopyFileToDirectory(webProject.Directory / "Configuration" / "Shell.config.default", ucommerceDir / "Configuration");

            CopyDirectoryRecursively(
                webProject.Directory / "Css",
                ucommerceDir / "Css",
                DirectoryExistsPolicy.Merge,
                excludeFile: info => info.Name.Contains(".less"));

            ///////////////////////
            // Ucommerce Install //
            ///////////////////////
            var ucommerceInstallDir = ucommerceDir / "install";

            CopyDirectoryRecursively(installerProjectDir / "SpeakSerialization", ucommerceInstallDir / "SpeakSerialization");

            CopyDirectoryRecursively(installerProjectDir / "ConfigurationTransformations", ucommerceInstallDir, DirectoryExistsPolicy.Merge);
            // ReSharper disable once StringLiteralTypo
            RenameDirectory(ucommerceInstallDir / "ConfigIncludes", "configinclude");

            CopyDirectoryRecursively(RootDirectory / "database", ucommerceInstallDir, DirectoryExistsPolicy.Merge);

            Solution
                .GetProject("Ucommerce.Sitecore")
                .GetOutputDir(Configuration)
                // ReSharper disable StringLiteralTypo
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
                // ReSharper restore StringLiteralTypo
                .Where(path =>
                {
                    if (Configuration == Configuration.Debug)
                    {
                        return true;
                    }

                    var extension = Path.GetExtension(path.ToString());
                    return extension != ".pdb" && extension != ".xml";
                })
                .Where(path => !path.ToString()
                    .Contains("Ucommerce.Installer.dll"))
                // ReSharper disable once PossibleNullReferenceException
                .Append(webProject.Directory / "bin" / "Ucommerce.Sitecore.Web.dll")
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
                Solution
                            .GetProject("Ucommerce.Sitecore92")
                            .GetOutputDir(Configuration) / "Ucommerce.Sitecore92.dll",
                ucommerceDir / "apps" / "Sitecore92compatibility.disabled" / "bin");
            CopyFileToDirectory(
                Solution.GetProject("Ucommerce.Sitecore93")
                            .GetOutputDir(Configuration) / "Ucommerce.Sitecore93.dll",
                ucommerceDir / "apps" / "Sitecore93compatibility.disabled" / "bin");

            // The download zip with cli and package
            CopyFileToDirectory(
                WorkDir / $"Ucommerce-PostInstall-{FullVersion}.zip",
                rootDir / "files" / "App_Data" / "packages");

            var cliBinDir = Solution
                                        .GetProject("Ucommerce.Sitecore.Cli")
                                        .GetOutputDir(Configuration);

            CopyDirectoryRecursively(
                cliBinDir,
                rootDir,
                DirectoryExistsPolicy.Merge,
                excludeFile: info =>
                    info.Name.Contains(".pdb") |
                    info.Name.Contains(".xml")
            );

            CompressionTasks.CompressZip(
                rootDir,
                ArtifactsDirectory / $"Ucommerce-Sitecore-{FullVersion}.zip",
                fileMode: FileMode.Create);
        });
}
