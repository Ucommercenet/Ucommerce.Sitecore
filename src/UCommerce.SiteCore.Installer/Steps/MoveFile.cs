using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;
using AuthorizeNet.Api.Contracts.V1;

namespace Ucommerce.Sitecore.Installer.Steps
{
	public class MoveFile : IStep
	{
		private readonly bool _backupTarget;
		private readonly Ucommerce.Installer.FileMover _command;

		public MoveFile(string sourceVirtualPath, string targetVirtualPath, bool backupTarget)
		{
			_backupTarget = backupTarget;

			FileInfo source = new FileInfo(HostingEnvironment.MapPath(sourceVirtualPath)),
				target = new FileInfo(HostingEnvironment.MapPath(targetVirtualPath));

			_command = new Ucommerce.Installer.FileMover(source, target);
		}

        public Task Run()
        {
            _command.Move(_backupTarget, ex => new SitecoreInstallerLoggingService().Error<int>(ex));
			return Task.CompletedTask;
        }
		
    }
}