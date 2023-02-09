using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Install.Framework;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Installer.Steps;

namespace Ucommerce.Sitecore.Installer
{
    public class PostInstallationStep : IPostStep
    {
        private readonly IList<IPostStep> _postInstallationSteps;

        /// <summary>
        ///     The uCommerce post installation step.
        /// </summary>
        /// <remarks>
        ///     There is a race condition between upgrading the database and upgrading the binaries. :-(
        ///     Upgrade the database first, and the old binaries might not work with the new database.
        ///     Upgrade the binaries first, and the new binaries might not work with the old database.
        ///     We have one observation indicating a failed installation because the new binaries was
        ///     activated before the database scripts were done, resulting in a broken system.
        ///     The problem is probably going to grow, as more database migrations are added. 
        ///     We have chosen to upgrade the database first.
        ///     This is because the database upgrade takes a long time in the clean scenario, but is
        ///     relatively faster in upgrade scenarios.
        ///     So for clean installs there are no old binaries, so the race condition is void.
        ///     - Jesper
        /// </remarks>
        public PostInstallationStep()
        {
            var sitecoreVersionChecker = new SitecoreVersionCheckerOnline();

            _postInstallationSteps = new List<IPostStep>();

            _postInstallationSteps.Add(new CreateApplicationShortcuts());
            _postInstallationSteps.Add(new CreateSpeakApplicationIfSupported(sitecoreVersionChecker));
            _postInstallationSteps.Add(new MoveSitecoreConfigIncludes());
        }

        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            foreach (var step in _postInstallationSteps)
            {
                IInstallerLoggingService logging = new SitecoreInstallerLoggingService();

                try
                {
                    step.Run(output, metaData);
                    logging.Information<PostInstallationStep>($"Executed: {step.GetType().FullName}");
                }
                catch (Exception ex)
                {
                    logging.Error<PostInstallationStep>(ex,
                        step.GetType()
                            .FullName);

                    throw;
                }
            }
        }
    }
}
