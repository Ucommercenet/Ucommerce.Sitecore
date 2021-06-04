using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;

namespace Ucommerce.Sitecore.Installer.Steps
{
	/// <summary>
	/// Extract section by a giving section path from file to an seperated file.
	/// </summary>
	class SeperateConfigSectionInNewFile : IPostStep
	{
		private readonly string _section;
		private readonly Ucommerce.Installer.ExtractSection _command;

		public SeperateConfigSectionInNewFile(string sectionPath, string sourceVirtualPath, string targetVirtualPath)
		{
			var source = new FileInfo(HostingEnvironment.MapPath(sourceVirtualPath));
			var target = new FileInfo(HostingEnvironment.MapPath(targetVirtualPath));

			_command = new Ucommerce.Installer.ExtractSection(sectionPath, source, target);
		}


		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			_command.Move(ex => new SitecoreInstallerLoggingService().Error<int>(ex));
		}
	}
}
