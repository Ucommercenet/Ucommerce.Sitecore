using System.IO;
using System.Linq;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Install;
using Ucommerce.Sitecore.Installer.FileExtensions;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class RemoveRenamedPipelines : AggregateStep
    {
        private readonly IInstallerLoggingService _loggingService;

        public RemoveRenamedPipelines(DirectoryInfo sitecoreDirectory, IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
            var renamedPipelineConfigs = new[]
            {
                "Basket.config",
                "Checkout.config",
                "DeleteCampaignItem.config",
                "DeleteCategory.config",
                "DeleteDataType.config",
                "DeleteDefinition.config",
                "DeleteLanguage.config",
                "DeleteProduct.config",
                "DeleteProductCatalog.config",
                "DeleteProductCatalogGroup.config",
                "DeleteProductDefinitionField.config",
                "Processing.config",
                "ProductReview.config",
                "ProductReview.config",
                "ProductReviewComment.config",
                "SaveCategory.config",
                "SaveDataType.config",
                "SaveDefinition.config",
                "SaveDefinitionField.config",
                "SaveLanguage.config",
                "SaveOrder.config",
                "SaveProduct.config",
                "SaveProductCatalog.config",
                "SaveProductCatalogGroup.config",
                "SaveProductDefinitionField.config",
                "ToCancelled.config",
                "ToCompletedOrder.config"
            };
            Steps.AddRange(renamedPipelineConfigs.Select(
                config => new DeleteFileWithBackup(
                    sitecoreDirectory.CombineDirectory("sitecore modules", "Shell", "ucommerce", "Pipelines").CombineFile(config),
                    _loggingService))
            );
        }

        protected override void LogStart()
        {
            _loggingService.Information<DeleteRavenDB>("Removing old pipelines...");
        }
    }
}
