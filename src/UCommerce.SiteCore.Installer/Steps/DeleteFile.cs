using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
	public class DeleteFile : IPostStep
	{
        private readonly IInstallerLoggingService _logging;
        private readonly FileDeleter _command;

		public DeleteFile(string filePath, IInstallerLoggingService logging)
		{
            _logging = logging;
            var filePathInfo = new FileInfo(HostingEnvironment.MapPath(filePath));
			_command = new FileDeleter(filePathInfo);
		}

		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			_command.Delete(ex => _logging.Error<DeleteFile>(ex));
		}
	}
}
