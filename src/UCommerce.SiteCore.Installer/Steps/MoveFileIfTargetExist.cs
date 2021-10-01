using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
	public class MoveFileIfTargetExist : IPostStep
	{
		private readonly bool _backupTarget;
        private readonly IInstallerLoggingService _loggingService;
        private readonly FileMoverIfTargetExist _command;

		public MoveFileIfTargetExist(string sourceVirtualPath, string targetVirtualPath, bool backupTarget, IInstallerLoggingService loggingService)
		{
			_backupTarget = backupTarget;
            _loggingService = loggingService;
            FileInfo source = new FileInfo(HostingEnvironment.MapPath(sourceVirtualPath)),
                     target = new FileInfo(HostingEnvironment.MapPath(targetVirtualPath));

			_command = new FileMoverIfTargetExist(source, target);
		}

		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			_command.MoveIfTargetExist(_backupTarget, ex => _loggingService.Error<MoveFileIfTargetExist>(ex));
		}
	}
}
