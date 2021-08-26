using System;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.Git;
using Nuke.Common.Utilities;

// These targets take care of patching the assemblyinfo.cs files, and cleaning up again, in the project
partial class Build
{
    [Parameter("Patch all assemblyinfo.cs files in the solution before compile", Name = "PatchAssemblyInfo")]
    bool ShouldPatchAssemblyInfo;
    
    [Parameter("The version in semver format")] 
    string Version = "0.0.0";
    
    string BuildNumber => $"{DateTime.Today:yy}{DateTime.Today.DayOfYear:000}";

    string FullVersion => $"{Version}.{BuildNumber}";
    Target PatchAssemblyInfo => _ => _
         .OnlyWhenStatic(() => ShouldPatchAssemblyInfo)
         .DependentFor(Compile)
         .Unlisted()
         .Executes(() =>
         {
             var commitSha = GitTasks.GitCurrentCommit();
             var assemblyInfoFiles = SourceDirectory.GlobFiles("**/AssemblyInfo.cs");
             foreach (var assemblyInfoFile in assemblyInfoFiles)
             {
                 var contents = TextTasks.ReadAllText(assemblyInfoFile);

                 var newContents = contents
                     .ReplaceRegex("AssemblyVersion\\(.*\\)", _ => $"AssemblyVersion(\"{FullVersion}\")")
                     .ReplaceRegex("AssemblyFileVersion\\(.*\\)", _ => $"AssemblyFileVersion(\"{FullVersion}\")")
                     .ReplaceRegex("AssemblyInformationalVersion\\(.*\\)", _ => $"AssemblyInformationalVersion(\"{FullVersion} {commitSha}\")");
                TextTasks.WriteAllText(assemblyInfoFile, newContents);                 
             }
         });

    // ReSharper disable once UnusedMember.Local
    Target CleanupPatchAssemblyInfo => _ => _
        .TriggeredBy(PatchAssemblyInfo)
        .After(Compile)
        .Unlisted()
        .Executes(() =>
        {
            GitTasks.Git("checkout -- \"**\\AssemblyInfo.cs\"", SourceDirectory);
        });
}
