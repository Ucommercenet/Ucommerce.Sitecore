﻿using System;
using System.Collections.Specialized;
using Sitecore.Install.Framework;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.Steps
{
    public class UpdateUCommerceAssemblyVersionInDatabase : IPostStep
    {
        private readonly UpdateService _updateService;
        private readonly RuntimeVersionChecker _runtimeVersion;
        private readonly IInstallerLoggingService _installerLoggingService;

        public UpdateUCommerceAssemblyVersionInDatabase(UpdateService updateService, RuntimeVersionChecker runtimeVersion, IInstallerLoggingService installerLoggingService)
        {
            _updateService = updateService;
            _runtimeVersion = runtimeVersion;
            _installerLoggingService = installerLoggingService;
        }

        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            var assemblyVersion = _runtimeVersion.GetUCommerceRuntimeAssemblyVersion().ToString();

            _installerLoggingService.Log<UpdateUCommerceAssemblyVersionInDatabase>("New uCommerce version: " + assemblyVersion);
            _updateService.UpdateAssemblyVersion(assemblyVersion);
        }
    }
}
