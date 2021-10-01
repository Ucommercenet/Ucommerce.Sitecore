using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
	/// <summary>
	/// Moves a folder.
	/// </summary>
	public class MoveDirectory : IPostStep
	{
		private readonly string _sourceDirectory;
		private readonly string _targetDirectory;
		private readonly bool _overwriteTarget;
        private readonly IInstallerLoggingService _loggingService;

        public MoveDirectory(string sourceDirectory, string targetDirectory, bool overwriteTarget, IInstallerLoggingService loggingService)
		{
			_sourceDirectory = sourceDirectory;
			_targetDirectory = targetDirectory;
			_overwriteTarget = overwriteTarget;
            _loggingService = loggingService;
        }

		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			new DirectoryMover(
				new DirectoryInfo(HostingEnvironment.MapPath(_sourceDirectory)),
				new DirectoryInfo(HostingEnvironment.MapPath(_targetDirectory)),
				_overwriteTarget).Move(
				ex => _loggingService.Error<MoveDirectory>(ex));
		}
	}
}