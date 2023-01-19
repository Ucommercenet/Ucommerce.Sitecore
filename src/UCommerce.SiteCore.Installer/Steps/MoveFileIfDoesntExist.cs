using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Ucommerce.Sitecore.Installer.Steps
{
	public class MoveFileIfDoesntExist : IStep
	{
		private readonly Ucommerce.Installer.FileMover _command;

		public MoveFileIfDoesntExist(FileInfo sourceFile, FileInfo targetFile)
		{
			_command = new Ucommerce.Installer.FileMover(sourceFile, targetFile);
		}

		public async Task Run()
		{
			_command.MoveIfDoesntExist(ex => new SitecoreInstallerLoggingService().Error<int>(ex));
		}
	}
}