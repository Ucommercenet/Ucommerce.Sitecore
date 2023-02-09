using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class MoveFileWeb : IPostStep
    {
        private readonly bool _backupTarget;
        private readonly FileMover _command;

        public MoveFileWeb(string sourceVirtualPath, string targetVirtualPath, bool backupTarget)
        {
            _backupTarget = backupTarget;

            FileInfo source = new FileInfo(HostingEnvironment.MapPath(sourceVirtualPath)),
                target = new FileInfo(HostingEnvironment.MapPath(targetVirtualPath));

            _command = new FileMover(source, target);
        }

        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            _command.Move(_backupTarget, ex => new SitecoreInstallerLoggingService().Error<int>(ex));
        }
    }
}
