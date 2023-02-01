using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Installer.FileExtensions;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class UpgradeAppIfEnabled : IStep
    {
        private readonly string _appName;
        private readonly DirectoryInfo _disabledDirectory;
        private readonly DirectoryInfo _enabledDirectory;
        private readonly IInstallerLoggingService _logging;

        public UpgradeAppIfEnabled(string appName, DirectoryInfo sitecoreDirectory, IInstallerLoggingService logging)
        {
            _appName = appName;
            _logging = logging;
            var appsDirectory = sitecoreDirectory.CombineDirectory("sitecore modules", "shell", "ucommerce", "apps");
            _disabledDirectory = appsDirectory.CombineDirectory($"{appName}.disabled");
            _enabledDirectory = appsDirectory.CombineDirectory(appName);
        }

        public async Task Run()
        {
            _logging.Information<EnableApp>($"Upgrading the '{_appName}' app if it is enabled");

            if (!_disabledDirectory.Exists)
            {
                var exception = new DirectoryNotFoundException(
                    $"The directory {_disabledDirectory.FullName} could not be found. This should never happen, did something go wrong when files were copied over?");
                _logging.Error<EnableApp>(exception, $"The app {_appName} could not be found");
                throw exception;
            }

            if (!_enabledDirectory.Exists)
            {
                _logging.Information<EnableApp>($"{_appName} is not enabled, leaving it disabled...");
                return;
            }

            await new MoveDirectory(_disabledDirectory, _enabledDirectory, true, _logging)
                .Run();
        }
    }
}
