using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;

namespace Ucommerce.Sitecore.Installer.Steps
{
	public class MoveFile : IPostStep
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

		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			_command.Move(_backupTarget, ex => new SitecoreInstallerLoggingService().Error<int>(ex));
		}
	}
}