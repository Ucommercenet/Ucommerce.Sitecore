using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
	public class MoveFileIfTargetExist : IStep
	{
		private readonly bool _backupTarget;
		private readonly FileMoverIfTargetExist _command;

		public MoveFileIfTargetExist(FileInfo sourceFile, FileInfo targetFile, bool backupTarget)
		{
			_backupTarget = backupTarget; 
			_command = new FileMoverIfTargetExist(sourceFile, targetFile);
		}

		public async Task Run()
		{
			_command.MoveIfTargetExist(_backupTarget, ex => new SitecoreInstallerLoggingService().Error<int>(ex));
		}
	}
}
