using System;
using System.IO;
using System.Xml.XPath;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Install;

namespace Ucommerce.Sitecore.Cli
{
    public class SitecoreVersionCheckerOffline : ISitecoreVersionChecker
    {
        private readonly DirectoryInfo _sitecorePath;
        private readonly IInstallerLoggingService _logging;
        private Version _version;

        public SitecoreVersionCheckerOffline(DirectoryInfo sitecorePath, IInstallerLoggingService logging)
        {
            _sitecorePath = sitecorePath ?? throw new ArgumentNullException(nameof(sitecorePath));
            if (!sitecorePath.Exists)
            {
                throw new ArgumentException($"Sitecore version file does not exist: {sitecorePath.FullName}");
            }

            _logging = logging ?? throw new ArgumentNullException(nameof(logging));
        }

        public bool IsEqualOrGreaterThan(Version version)
        {
            return GetVersion() >= version;
        }

        public bool IsLowerThan(Version version)
        {
            return GetVersion() < version;
        }

        public bool SupportsSpeakApps()
        {
            return GetVersion()
                .Major >= 8;
        }

        private FileInfo GetSitecoreConfigFilePath()
        {
            var webConfigFilePath = new FileInfo(Path.Combine(_sitecorePath.FullName, "web.config"));
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

            return new FileInfo(Path.Combine(_sitecorePath.FullName, sitecoreConfig));
        }

        private Version GetVersion()
        {
            if (_version != null)
            {
                return _version;
            }

            var versionData = GetVersionData();
            var buildNumber = GetBuildNumber(versionData);
            var versionNumber = GetMajorVersion(versionData)
                + "." + GetMinorVersion(versionData)
                + (string.IsNullOrEmpty(buildNumber) ? string.Empty : "." + buildNumber)
                + "." + GetRevisionNumber(versionData);
            _version = new Version(versionNumber);
            return _version;
        }

        private XPathDocument GetVersionData()
        {
            try
            {
                var sitecoreConfigPath = GetSitecoreConfigFilePath();
                var versionFile = GetVersionFile(sitecoreConfigPath);

                return new XPathDocument(versionFile.FullName);
            }
            catch (Exception ex)
            {
                _logging.Error<SitecoreVersionCheckerOffline>(ex);
                throw;
            }
        }

        private FileInfo GetVersionFile(FileInfo sitecoreConfigFilePath)
        {
            if (!sitecoreConfigFilePath.Exists)
            {
                throw new FileNotFoundException($"Could not find sitecore config file: {sitecoreConfigFilePath.FullName}", sitecoreConfigFilePath.FullName);
            }

            var sitecoreConfig = new XPathDocument(sitecoreConfigFilePath.FullName);
            var versionFile = sitecoreConfig.CreateNavigator()
                .SelectSingleNode("/sitecore/settings/setting[@name='VersionFilePath']")
                ?.GetAttribute("value", "");
            if (string.IsNullOrEmpty(versionFile))
            {
                throw new Exception($"Could not locate a setting named VersionFilePath in '{sitecoreConfigFilePath.FullName}'");
            }

            return new FileInfo(Path.Combine(_sitecorePath.FullName, versionFile.TrimStart('/').Replace('/', '\\')));
        }

        private static string GetBuildNumber(XPathDocument document) => GetNodeValue("/*/version/build", document);
        private static string GetMajorVersion(XPathDocument document) => GetNodeValue("/*/version/major", document);
        private static string GetMinorVersion(XPathDocument document) => GetNodeValue("/*/version/minor", document);

        private static string GetNodeValue(string xpath, XPathDocument document)
        {
            var xpathNavigator = document.CreateNavigator()
                .SelectSingleNode(xpath);
            return xpathNavigator?.Value;
        }

        private static string GetRevisionNumber(XPathDocument document) => GetNodeValue("/*/version/revision", document);
    }
}
