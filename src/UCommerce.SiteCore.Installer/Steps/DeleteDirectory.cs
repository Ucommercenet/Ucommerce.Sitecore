using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class DeleteDirectory : IPostStep
    {
        private readonly DirectoryDeleter _command;

        public DeleteDirectory(string directoryPath)
        {
            var directoryPathInfo = new DirectoryInfo(HostingEnvironment.MapPath(directoryPath));
            _command = new DirectoryDeleter(directoryPathInfo);
        }

        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            _command.Delete(ex => new SitecoreInstallerLoggingService().Log<int>(ex));
        }
    }
}
