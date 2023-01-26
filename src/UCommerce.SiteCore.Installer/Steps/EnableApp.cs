using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class EnableApp : IStep
    {
        private readonly string _appName;
        private readonly DirectoryInfo _disabledDirectory;
        private readonly DirectoryInfo _enabledDirectory;
        private readonly IInstallerLoggingService _logging;

        public EnableApp(string appName, DirectoryInfo sitecoreDirectory, IInstallerLoggingService logging)
        {
            _appName = appName;
            _logging = logging;
            var appsDirectory = new DirectoryInfo(Path.Combine(sitecoreDirectory.FullName, "sitecore modules", "shell", "ucommerce", "apps"));
            _disabledDirectory = new DirectoryInfo(Path.Combine(appsDirectory.FullName, $"{appName}.disabled"));
            _enabledDirectory = new DirectoryInfo(Path.Combine(appsDirectory.FullName, appName));
        }

        public async Task Run()
        {
            _logging.Information<EnableApp>($"Enabling the '{_appName}' app");

            if (!_disabledDirectory.Exists)
            {
                var exception = new DirectoryNotFoundException(
                    $"The directory {_disabledDirectory.FullName} could not be found. This should never happen, did something go wrong when files were copied over?");
                _logging.Error<EnableApp>(exception, $"The app {_appName} could not be found");
                throw exception;
            }

            if (_enabledDirectory.Exists)
            {
                _logging.Information<EnableApp>($"{_appName} is already installed, overwriting with new version...");
            }

            await new MoveDirectory(_disabledDirectory, _enabledDirectory, true, _logging)
                .Run();
        }
    }
}
