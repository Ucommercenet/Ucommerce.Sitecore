using System.IO;

namespace Ucommerce.Sitecore.Installer.FileExtensions
{
    internal static class DirectoryExtensions
    {
        internal static DirectoryInfo CombineDirectory(this DirectoryInfo directory, params string[] paths)
        {
            var path = Path.Combine(directory.FullName, Path.Combine(paths));
            return new DirectoryInfo(path);
        }

        internal static FileInfo CombineFile(this DirectoryInfo directory, params string[] paths)
        {
            var path = Path.Combine(directory.FullName, Path.Combine(paths));
            return new FileInfo(path);
        }
    }
}
