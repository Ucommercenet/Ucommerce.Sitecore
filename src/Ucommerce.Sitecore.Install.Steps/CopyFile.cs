using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Install.Steps
{
    public class CopyFile : IStep
    {
        private readonly FileCopier _command;
        private readonly IInstallerLoggingService _logging;
        private readonly FileInfo _source;
        private readonly FileInfo _target;

        public CopyFile(FileInfo source, FileInfo target, IInstallerLoggingService logging)
        {
            _logging = logging;
            _source = source;
            _target = target;
            _command = new FileCopier(source, target);
        }

        public Task Run()
        {
            _logging.Information<CopyFile>($"Copying file {_source.FullName} to {_target.FullName}");
            _command.Copy(ex => _logging.Error<int>(ex));
            return Task.CompletedTask;
        }
    }
}
