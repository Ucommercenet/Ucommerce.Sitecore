using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;

namespace UCommerce.Sitecore.Installer.Steps
{
	public class MoveFileIfDoesntExist : IPostStep
	{
		private readonly UCommerce.Installer.FileMover _command;
		
		public MoveFileIfDoesntExist(string sourceVirtualPath, string targetVirtualPath)
		{
			FileInfo source = new FileInfo(HostingEnvironment.MapPath(sourceVirtualPath)),
				target = new FileInfo(HostingEnvironment.MapPath(targetVirtualPath));

			_command = new UCommerce.Installer.FileMover(source, target);
		}

		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			_command.MoveIfDoesntExist(ex => new SitecoreInstallerLoggingService().Log<int>(ex));
		}
	}
}