using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
	public class MoveDirectoryIfTargetExist : IPostStep
	{
		private readonly string _sourceDirectory;
		private readonly string _targetDirectory;

		public MoveDirectoryIfTargetExist(string sourceDirectory, string targetDirectory)
		{
			_sourceDirectory = sourceDirectory;
			_targetDirectory = targetDirectory;
		}

		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			new DirectoryMoverIfTargetExist(
				new DirectoryInfo(HostingEnvironment.MapPath(_sourceDirectory)),
				new DirectoryInfo(HostingEnvironment.MapPath(_targetDirectory)))
				.Move(ex => new SitecoreInstallerLoggingService().Log<int>(ex));
		}
	}
}
