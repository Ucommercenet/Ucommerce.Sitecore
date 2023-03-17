using System.IO;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Install.Steps
{
    /// <summary>
    /// Aggregate step that combines the backup file and delete file steps into one
    /// </summary>
    public class DeleteFileWithBackup : AggregateStep
    {
        public DeleteFileWithBackup(FileInfo file, IInstallerLoggingService loggingService)
        {
            Steps.AddRange(new IStep[]
            {
                new BackupFile(file, loggingService),
                new DeleteFile(file, loggingService)
            });
        }
    }
}
