﻿using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Install.Steps
{
    /// <summary>
    /// Installation step that initializes an object factory
    /// </summary>
    public class InitializeObjectFactory : IStep
    {
        private readonly IInstallerLoggingService _loggingService;

        public InitializeObjectFactory(IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public Task Run()
        {
            _loggingService.Information<InitializeObjectFactory>("Initializing object factory...");
            var initializer = new ObjectFactoryInitializer();
            initializer.InitializeObjectFactory();
            return Task.CompletedTask;
        }
    }
}
