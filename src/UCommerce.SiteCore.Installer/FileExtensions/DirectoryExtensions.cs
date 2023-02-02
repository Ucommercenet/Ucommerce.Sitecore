using System.IO;

namespace Ucommerce.Sitecore.Installer.FileExtensions
{
    /// <summary>
    /// Extension methods used for the DirectoryInfo object
    /// </summary>
    internal static class DirectoryExtensions
    {
        /// <summary>
        /// Combines the path of the directory and the given strings
        /// </summary>
        /// <param name="directory">The directory that defines the first part of the path</param>
        /// <param name="paths">The strings that make up the rest of the path</param>
        /// <returns>A new DirectoryInfo based on the combined path</returns>
        internal static DirectoryInfo CombineDirectory(this DirectoryInfo directory, params string[] paths)
        {
            var path = Path.Combine(directory.FullName, Path.Combine(paths));
            return new DirectoryInfo(path);
        }

        /// <summary>
        /// Combines the path of the directory and the given strings
        /// </summary>
        /// <param name="directory">The directory that defines the first part of the path</param>
        /// <param name="paths">The strings that make up the rest of the path</param>
        /// <returns>A new FileInfo based on the combined path</returns>
        internal static FileInfo CombineFile(this DirectoryInfo directory, params string[] paths)
        {
            var path = Path.Combine(directory.FullName, Path.Combine(paths));
            return new FileInfo(path);
        }
    }
}
