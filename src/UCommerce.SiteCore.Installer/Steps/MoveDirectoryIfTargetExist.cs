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
        private readonly IInstallerLoggingService _installerLoggingService;

        public MoveDirectoryIfTargetExist(string sourceDirectory, string targetDirectory, IInstallerLoggingService installerLoggingService)
		{
			_sourceDirectory = sourceDirectory;
			_targetDirectory = targetDirectory;
            _installerLoggingService = installerLoggingService;
        }

		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			new DirectoryMoverIfTargetExist(
				new DirectoryInfo(HostingEnvironment.MapPath(_sourceDirectory)),
				new DirectoryInfo(HostingEnvironment.MapPath(_targetDirectory)))
				.Move(ex => _installerLoggingService.Error<int>(ex));
		}
	}
}
