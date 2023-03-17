using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Install.Steps.FileExtensions;

namespace Ucommerce.Sitecore.Install.Steps
{
    /// <summary>
    /// Installation step that enables ElasticSearch and disables Lucene, if possible
    /// </summary>
    public class UpgradeSearchProviders : IStep
    {
        private readonly DirectoryInfo _appsDirectory;
        private readonly IInstallerLoggingService _logging;
        private readonly DirectoryInfo _sitecoreDirectory;

        public UpgradeSearchProviders(DirectoryInfo sitecoreDirectory, IInstallerLoggingService logging)
        {
            _sitecoreDirectory = sitecoreDirectory;
            _logging = logging;
            _appsDirectory = sitecoreDirectory.CombineDirectory("sitecore modules", "shell", "ucommerce", "apps");
        }

        public async Task Run()
        {
            _logging.Information<UpgradeSearchProviders>("Detecting enabled search provider...");
            var enabledLuceneAppDirectory = _appsDirectory.CombineDirectory("Ucommerce.Search.Lucene");
            var disabledLuceneAppDirectory = _appsDirectory.CombineDirectory("Ucommerce.Search.Lucene.disabled");
            var enabledElasticAppDirectory = _appsDirectory.CombineDirectory("Ucommerce.Search.ElasticSearch");

            if (enabledElasticAppDirectory.Exists)
            {
                _logging.Information<UpgradeSearchProviders>("ElasticSearch detected, upgrading...");
                await new UpgradeAppIfEnabled("Ucommerce.Search.ElasticSearch", _sitecoreDirectory, _logging).Run();
                _logging.Information<UpgradeSearchProviders>("Lucene app enabled by default, disabling...");
                await new MoveDirectoryIfTargetExist(enabledLuceneAppDirectory, disabledLuceneAppDirectory, _logging).Run();
            }
            else
            {
                _logging.Information<UpgradeSearchProviders>("Lucene detected, upgrading...");
            }
        }
    }
}
