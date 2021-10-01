using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
	public class MoveFile : IPostStep
	{
		private readonly bool _backupTarget;
        private readonly IInstallerLoggingService _loggingService;
        private readonly Ucommerce.Installer.FileMover _command;

		public MoveFile(string sourceVirtualPath, string targetVirtualPath, bool backupTarget, IInstallerLoggingService loggingService)
		{
			_backupTarget = backupTarget;
            _loggingService = loggingService;

            FileInfo source = new FileInfo(HostingEnvironment.MapPath(sourceVirtualPath)),
                     target = new FileInfo(HostingEnvironment.MapPath(targetVirtualPath));

			_command = new Ucommerce.Installer.FileMover(source, target);
		}

		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			_command.Move(_backupTarget, ex => _loggingService.Error<MoveFile>(ex));
		}
	}
}