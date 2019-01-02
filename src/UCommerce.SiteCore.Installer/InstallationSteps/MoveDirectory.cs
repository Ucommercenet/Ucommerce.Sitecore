using System;
using System.IO;
using System.Web.Hosting;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.InstallationSteps
{
    public class MoveDirectory : IInstallationStep
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

        public void Execute()
        {
            new DirectoryMover(
                new DirectoryInfo(HostingEnvironment.MapPath(_sourceDirectory)),
                new DirectoryInfo(HostingEnvironment.MapPath(_targetDirectory)),
                _overwriteTarget).Move(
                ex => new SitecoreInstallerLoggingService().Log<int>(ex));
        }
    }
}