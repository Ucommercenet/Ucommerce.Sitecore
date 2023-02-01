using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Installer.FileExtensions;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class AddHeadlessToIgnoreUrlPrefixes : IStep
    {
        private readonly IInstallerLoggingService _loggingService;
        private readonly DirectoryInfo _sitecoreDirectory;

        public AddHeadlessToIgnoreUrlPrefixes(DirectoryInfo sitecoreDirectory, IInstallerLoggingService loggingService)
        {
            _sitecoreDirectory = sitecoreDirectory;
            _loggingService = loggingService;
        }

        public Task Run()
        {
            _loggingService.Information<AddHeadlessToIgnoreUrlPrefixes>("Adding client dependencies to sitecore.config");

            var sitecoreConfig = new XmlDocument();

            var sitecoreConfigPath = GetSitecoreConfigFilePath();

            sitecoreConfig.Load(sitecoreConfigPath.FullName);

            var ignoreUrlPrefixesNode =
                sitecoreConfig.SelectSingleNode("sitecore/settings//setting[@name='IgnoreUrlPrefixes']");

            if (ignoreUrlPrefixesNode != null && ignoreUrlPrefixesNode.Attributes != null)
            {
                var ignoreUrlPrefixesValue = ignoreUrlPrefixesNode.Attributes["value"]
                    .Value;
                if (!ignoreUrlPrefixesValue.Contains("|/api/v1"))
                {
                    ignoreUrlPrefixesNode.Attributes["value"]
                        .Value += "|/api/v1";
                }
            }

            sitecoreConfig.Save(sitecoreConfigPath.FullName);

            return Task.CompletedTask;
        }

        private FileInfo GetSitecoreConfigFilePath()
        {
            var webConfigFilePath = _sitecoreDirectory.CombineFile("web.config");
            if (!webConfigFilePath.Exists)
            {
                throw new FileNotFoundException($"Could not find web.config: {webConfigFilePath.FullName}", webConfigFilePath.FullName);
            }

            var webConfig = new XPathDocument(webConfigFilePath.FullName);
            var sitecoreConfig = webConfig.CreateNavigator()
                .SelectSingleNode("/configuration/sitecore")
                ?.GetAttribute("configSource", "");
            if (string.IsNullOrEmpty(sitecoreConfig))
            {
                throw new Exception("Could not locate 'configuration/sitecore' in web.config");
            }

            return _sitecoreDirectory.CombineFile(sitecoreConfig);
        }
    }
}
