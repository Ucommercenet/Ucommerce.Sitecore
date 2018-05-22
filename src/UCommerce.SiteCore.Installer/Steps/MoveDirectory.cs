using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.Steps
{
	/// <summary>
	/// Moves a folder.
	/// </summary>
	public class MoveDirectory : IPostStep
	{
		private readonly string _sourceDirectory;
		private readonly string _targetDirectory;
		private readonly bool _overwriteTarget;

		public MoveDirectory(string sourceDirectory, string targetDirectory, bool overwriteTarget)
		{
			_sourceDirectory = sourceDirectory;
			_targetDirectory = targetDirectory;
			_overwriteTarget = overwriteTarget;
		}

		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			new DirectoryMover(
				new DirectoryInfo(HostingEnvironment.MapPath(_sourceDirectory)), 
				new DirectoryInfo(HostingEnvironment.MapPath(_targetDirectory)),
				_overwriteTarget).Move(
				ex => new SitecoreInstallerLoggingService().Log<int>(ex));
		}
	}
}