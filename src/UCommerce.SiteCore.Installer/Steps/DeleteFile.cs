using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.Steps
{
	public class DeleteFile : IPostStep
	{
		private readonly FileDeleter _command;

		public DeleteFile(string filePath)
		{
			var filePathInfo = new FileInfo(HostingEnvironment.MapPath(filePath));

			_command = new FileDeleter(filePathInfo);
		}

		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			_command.Delete(ex => new SitecoreInstallerLoggingService().Log<int>(ex));
		}
	}
}
