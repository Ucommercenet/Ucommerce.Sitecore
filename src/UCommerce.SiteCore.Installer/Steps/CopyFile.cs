using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class CopyFile : IStep
    {
        private readonly FileCopier _command;
        private readonly IInstallerLoggingService _logging;

        public CopyFile(FileInfo source, FileInfo target, IInstallerLoggingService logging)
        {
            _logging = logging;
            _command = new FileCopier(source, target);
        }

        public async Task Run()
        {
            _command.Copy(ex => _logging.Error<int>(ex));
        }
    }
}
