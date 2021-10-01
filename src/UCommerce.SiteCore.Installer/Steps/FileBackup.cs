using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
	public class FileBackup : IPostStep
	{
        private readonly IInstallerLoggingService _logging;
        private Ucommerce.Installer.FileBackup _command;

		public FileBackup(string sourceVirtualPath, IInstallerLoggingService logging)
		{
            _logging = logging;
            var source = new FileInfo(HostingEnvironment.MapPath(sourceVirtualPath));

			_command = new Ucommerce.Installer.FileBackup(source);
		}

		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			_command.Backup(ex => _logging.Error<FileBackup>(ex));
		}
	}
}
