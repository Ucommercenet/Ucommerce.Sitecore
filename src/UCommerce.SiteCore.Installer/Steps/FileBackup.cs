using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;

namespace Ucommerce.Sitecore.Installer.Steps
{
	public class FileBackup : IPostStep
	{
		private Ucommerce.Installer.FileBackup _command;

		public FileBackup(string sourceVirtualPath)
		{
			var source = new FileInfo(HostingEnvironment.MapPath(sourceVirtualPath));

			_command = new Ucommerce.Installer.FileBackup(source);
		}

		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			_command.Backup(ex => new SitecoreInstallerLoggingService().Log<int>(ex));
		}
	}
}
