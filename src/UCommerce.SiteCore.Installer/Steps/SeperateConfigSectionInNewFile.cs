using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    /// <summary>
    /// Extract section by a given section path from file to an seperated file.
    /// </summary>
    internal class SeperateConfigSectionInNewFile : IStep
    {
        private readonly ExtractSection _command;
        private readonly IInstallerLoggingService _loggingService;
        private readonly string _section;
        private readonly FileInfo _source;
        private readonly FileInfo _target;

        public SeperateConfigSectionInNewFile(string section, FileInfo source, FileInfo target, IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
            _section = section;
            _source = source;
            _target = target;
            _command = new ExtractSection(section, source, target);
        }

        public async Task Run()
        {
            _loggingService.Information<SeperateConfigSectionInNewFile>($"Seperating {_section} of {_source} to new file {_target}");
            _command.Move(ex => _loggingService.Error<int>(ex));
        }
    }
}
