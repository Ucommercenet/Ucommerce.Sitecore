using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    /// <summary>
    /// Extract section by a giving section path from file to an seperated file.
    /// </summary>
    internal class SeperateConfigSectionInNewFile : IPostStep
    {
        private readonly ExtractSection _command;
        private readonly IInstallerLoggingService _logging;

        public SeperateConfigSectionInNewFile(string sectionPath, string sourceVirtualPath, string targetVirtualPath, IInstallerLoggingService logging)
        {
            _logging = logging;
            var source = new FileInfo(HostingEnvironment.MapPath(sourceVirtualPath));
            var target = new FileInfo(HostingEnvironment.MapPath(targetVirtualPath));

            _command = new ExtractSection(sectionPath, source, target);
        }


        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            _command.Move(ex => _logging.Error<SeperateConfigSectionInNewFile>(ex));
        }
    }
}
