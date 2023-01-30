using System.IO;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class RemoveRenamedPipelines : AggregateStep
    {
        private readonly IInstallerLoggingService _loggingService;

        public RemoveRenamedPipelines(DirectoryInfo sitecoreDirectory, IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
            var pipelinesPath = new DirectoryInfo(Path.Combine(sitecoreDirectory.FullName, "sitecore modules", "Shell", "ucommerce", "Pipelines"));
            Steps.AddRange(new IStep[]
            {
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "Basket.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "Checkout.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "DeleteCampaignItem.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "DeleteCategory.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "DeleteDataType.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "DeleteDefinition.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "DeleteLanguage.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "DeleteProduct.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "DeleteProductCatalog.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "DeleteProductCatalogGroup.config")),
                    _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "DeleteProductDefinitionField.config")),
                    _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "Processing.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "ProductReview.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "ProductReview.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "ProductReviewComment.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "SaveCategory.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "SaveDataType.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "SaveDefinition.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "SaveDefinitionField.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "SaveLanguage.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "SaveOrder.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "SaveProduct.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "SaveProductCatalog.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "SaveProductCatalogGroup.config")),
                    _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "SaveProductDefinitionField.config")),
                    _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "ToCancelled.config")), _loggingService),
                new DeleteFileWithBackup(new FileInfo(Path.Combine(pipelinesPath.FullName, "ToCompletedOrder.config")), _loggingService)
            });
        }

        protected override void LogStart()
        {
            _loggingService.Information<DeleteRavenDB>("Removing old pipelines...");
        }
    }
}
