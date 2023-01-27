using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class UpgradeSearchProviders : IStep
    {
        private readonly DirectoryInfo _sitecoreDirectory;
        private readonly IInstallerLoggingService _logging;
        private readonly DirectoryInfo _appsDirectory;

        public UpgradeSearchProviders(DirectoryInfo sitecoreDirectory, IInstallerLoggingService logging)
        {
            _sitecoreDirectory = sitecoreDirectory;
            _logging = logging;
            _appsDirectory = new DirectoryInfo(Path.Combine(sitecoreDirectory.FullName, "sitecore modules", "shell", "ucommerce", "apps"));
        }

        public async Task Run()
        {
            _logging.Information<UpgradeSearchProviders>("Detecting enabled search provider...");
            var enabledLuceneAppDirectory = new DirectoryInfo(Path.Combine(_appsDirectory.FullName, "Ucommerce.Search.Lucene"));
            var disabledLuceneAppDirectory = new DirectoryInfo(Path.Combine(_appsDirectory.FullName, "Ucommerce.Search.Lucene.disabled"));
            var enabledElasticAppDirectory = new DirectoryInfo(Path.Combine(_appsDirectory.FullName, "Ucommerce.Search.ElasticSearch"));

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
