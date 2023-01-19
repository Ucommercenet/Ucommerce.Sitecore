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

		public MoveFile(FileInfo source, FileInfo target, bool backupTarget)
		{
			_backupTarget = backupTarget;
	
			_command = new Ucommerce.Installer.FileMover(source, target);
		}

        public async Task Run()
        {
            _command.Move(_backupTarget, ex => new SitecoreInstallerLoggingService().Error<int>(ex));
        }
		
    }
}