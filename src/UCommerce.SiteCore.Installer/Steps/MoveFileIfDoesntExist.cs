using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;

namespace Ucommerce.Sitecore.Installer.Steps
{
	public class MoveFileIfDoesntExist : IPostStep
	{
		private readonly Ucommerce.Installer.FileMover _command;

		public MoveFileIfDoesntExist(string sourceVirtualPath, string targetVirtualPath)
		{
			FileInfo source = new FileInfo(HostingEnvironment.MapPath(sourceVirtualPath)),
				target = new FileInfo(HostingEnvironment.MapPath(targetVirtualPath));

			_command = new Ucommerce.Installer.FileMover(source, target);
		}

		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			_command.MoveIfDoesntExist(ex => new SitecoreInstallerLoggingService().Log<int>(ex));
		}
	}
}