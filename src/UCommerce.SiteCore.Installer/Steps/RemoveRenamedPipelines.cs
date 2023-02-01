using System.IO;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Installer.FileExtensions;

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
                new DeleteFileWithBackup(pipelinesPath.CombineFile("Basket.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("Checkout.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("DeleteCampaignItem.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("DeleteCategory.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("DeleteDataType.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("DeleteDefinition.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("DeleteLanguage.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("DeleteProduct.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("DeleteProductCatalog.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("DeleteProductCatalogGroup.config"),
                    _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("DeleteProductDefinitionField.config"),
                    _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("Processing.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("ProductReview.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("ProductReview.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("ProductReviewComment.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("SaveCategory.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("SaveDataType.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("SaveDefinition.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("SaveDefinitionField.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("SaveLanguage.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("SaveOrder.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("SaveProduct.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("SaveProductCatalog.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("SaveProductCatalogGroup.config"),
                    _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("SaveProductDefinitionField.config"),
                    _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("ToCancelled.config"), _loggingService),
                new DeleteFileWithBackup(pipelinesPath.CombineFile("ToCompletedOrder.config"), _loggingService)
            });
        }

        protected override void LogStart()
        {
            _loggingService.Information<DeleteRavenDB>("Removing old pipelines...");
        }
    }
}
