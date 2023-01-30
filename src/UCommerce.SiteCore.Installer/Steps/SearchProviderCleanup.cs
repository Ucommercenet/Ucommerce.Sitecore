using System.IO;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class SearchProviderCleanup : AggregateStep
    {
        private readonly IInstallerLoggingService _loggingService;

        public SearchProviderCleanup(string appsPath, IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
            var luceneIndexesFolderPath = new DirectoryInfo(Path.Combine(appsPath, "Ucommerce.Search.Lucene", "Configuration", "Indexes"));
            var luceneIndexesFolderPathDisabled = new DirectoryInfo(Path.Combine(appsPath, "Ucommerce.Search.Lucene.disabled", "Configuration", "Indexes"));
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
