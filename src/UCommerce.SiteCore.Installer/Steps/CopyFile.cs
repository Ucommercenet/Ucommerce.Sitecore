using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;

namespace UCommerce.Sitecore.Installer.Steps
{
	public class CopyFile : IPostStep
	{
		private readonly UCommerce.Installer.FileCopier _command;

		public CopyFile(string sourceVirtualPath, string targetVirtualPath)
		{
            var source = new FileInfo(HostingEnvironment.MapPath(sourceVirtualPath));
			var target = new FileInfo(HostingEnvironment.MapPath(targetVirtualPath));

			_command = new UCommerce.Installer.FileCopier(source, target);
		}

		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			_command.Copy(ex => new SitecoreInstallerLoggingService().Log<int>(ex));
		}
	}
}
