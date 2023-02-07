using System.IO;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Install.Steps.FileExtensions;

namespace Ucommerce.Sitecore.Install.Steps
{
    /// <summary>
    /// Aggregate step that deletes folders for search providers which are no longer used
    /// </summary>
    public class SearchProviderCleanup : AggregateStep
    {
        private readonly IInstallerLoggingService _loggingService;

        public SearchProviderCleanup(DirectoryInfo appsDirectory, IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
            var luceneIndexesFolderPath = appsDirectory.CombineDirectory("Ucommerce.Search.Lucene", "Configuration", "Indexes");
            var luceneIndexesFolderPathDisabled = appsDirectory.CombineDirectory("Ucommerce.Search.Lucene.disabled", "Configuration", "Indexes");
            if (Directory.Exists(luceneIndexesFolderPath.FullName))
            {
                Steps.Add(new DeleteDirectory(luceneIndexesFolderPath, _loggingService));
            }

            if (Directory.Exists(luceneIndexesFolderPathDisabled.FullName))
            {
                Steps.Add(new DeleteDirectory(luceneIndexesFolderPathDisabled, _loggingService));
            }
        }

        protected override void LogStart()
        {
            _loggingService.Information<SearchProviderCleanup>("Cleaning up search providers...");
        }
    }
}
