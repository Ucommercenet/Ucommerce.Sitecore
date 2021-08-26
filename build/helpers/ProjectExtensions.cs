using Nuke.Common.IO;
using Nuke.Common.ProjectModel;

namespace Helpers
{
    public static class ProjectExtensions
    {
        public static AbsolutePath GetOutputDir(this Project project, Configuration configuration)
        {
            return project.Directory / "bin" / configuration;
        }
        
        public static void CopyProjectBinDir(this Project project, AbsolutePath targetDir, Configuration configuration)
        {
            FileSystemTasks.CopyDirectoryRecursively(project.GetOutputDir(configuration), targetDir);
        }
    }
}