using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.InstallationSteps
{
    public class SeperateConfigSectionInNewFile : IInstallationStep
    {
        private readonly string _source;
        private readonly string _target;
        private readonly string _section;
        private readonly IInstallerLoggingService _installerLoggingService;

        public SeperateConfigSectionInNewFile(string source, string target, string section, IInstallerLoggingService installerLoggingService)
        {
            _source = source;
            _target = target;
            _section = section;
            _installerLoggingService = installerLoggingService;
        }
        public void Execute()
        {
            var source = new FileInfo(HostingEnvironment.MapPath(_source));
            var target = new FileInfo(HostingEnvironment.MapPath(_target));

            var command = new UCommerce.Installer.ExtractSection(_section, source, target);
            command.Move(ex => _installerLoggingService.Log<SeperateConfigSectionInNewFile>(ex));

        }
    }
}
