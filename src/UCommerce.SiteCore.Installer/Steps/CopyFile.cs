using System;
using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
	public class CopyFile : IPostStep
    {
        private readonly IInstallerLoggingService _logging;
		private readonly Ucommerce.Installer.FileCopier _command;

		public CopyFile(string sourceVirtualPath, string targetVirtualPath, IInstallerLoggingService logging)
        {
            _logging = logging;

            var source = new FileInfo(HostingEnvironment.MapPath(sourceVirtualPath));
			var target = new FileInfo(HostingEnvironment.MapPath(targetVirtualPath));

			_command = new Ucommerce.Installer.FileCopier(source, target);
		}

		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			_command.Copy(ex => _logging.Error<CopyFile>(ex));
		}
	}
}
