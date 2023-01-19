using AuthorizeNet.Api.Contracts.V1;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
	public class MoveDirectoryIfTargetExist : IStep
	{
		private readonly DirectoryInfo _sourceDirectory;
		private readonly DirectoryInfo _targetDirectory;

		public MoveDirectoryIfTargetExist(DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory)
		{
			_sourceDirectory = sourceDirectory;
			_targetDirectory = targetDirectory;
		}

		public async Task Run()
		{
			new DirectoryMoverIfTargetExist( _sourceDirectory, _targetDirectory)
				.Move(ex => new SitecoreInstallerLoggingService().Error<int>(ex));
           

        }
		 
       
    }
}
