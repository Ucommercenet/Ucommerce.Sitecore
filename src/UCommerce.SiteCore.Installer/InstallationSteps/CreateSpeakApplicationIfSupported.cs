using System;
using System.Collections.Generic;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.InstallationSteps
{
    public class CreateSpeakApplicationIfSupported : IInstallationStep
    {
        private readonly SitecoreVersionChecker _sitecoreVersionChecker;

        public CreateSpeakApplicationIfSupported(SitecoreVersionChecker sitecoreVersionChecker)
        {
            _sitecoreVersionChecker = sitecoreVersionChecker;
        }
        public void Execute()
        {
            IList<IInstallationStep> installationSteps = new List<IInstallationStep>();
            if (_sitecoreVersionChecker.SupportsSpeakApps())
            {
                installationSteps.Add(new CreateSpeakApplications());
                installationSteps.Add(new CreateApplicationLaunchButton());

                installationSteps.Add(new RemoveUCommerceApplicationLaunchButton());

            }

            if (_sitecoreVersionChecker.IsEqualOrGreaterThan(new Version(8, 2)))
            {
                installationSteps.Add(new AddTitleToCommerceSpeakAppsSection());
            }

            foreach (var installationStep in installationSteps)
            {
                installationStep.Execute();
            }
        }
    }
}