using System.IO;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
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
