using System;
using System.IO;
using Sitecore.Data.Serialization;
using Sitecore.Data.Serialization.ObjectModel;
using Sitecore.IO;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.InstallationSteps
{
    public class AddTitleToCommerceSpeakAppsSection : IInstallationStep
    {
        public void Execute()
        {
            IInstallerLoggingService logging = new SitecoreInstallerLoggingService();
            logging.Log<Steps.CreateSpeakApplications>("AddTitleToCommerceSpeakAppsSection started.");

            Parse(new DirectoryInfo(GetRootFolder()));

            logging.Log<Steps.CreateSpeakApplications>("AddTitleToCommerceSpeakAppsSection finished.");
        }

        private void Parse(DirectoryInfo directoryInfo)
        {
            var fileInfos = directoryInfo.GetFiles("*.item");
            foreach (var fileInfo in fileInfos)
            {
                var streamReader = new StreamReader(fileInfo.FullName);

                var syncItem = SyncItem.ReadItem(new Tokenizer(streamReader), true);
                var options = new LoadOptions { DisableEvents = true, ForceUpdate = true, UseNewID = false };

                ItemSynchronization.PasteSyncItem(syncItem, options, true);
            }

            foreach (var info in directoryInfo.GetDirectories())
            {
                Parse(info);
            }
        }
        private string GetRootFolder()
        {
            string serializationFolder = "/Sitecore Modules/Shell/ucommerce/install/SpeakSerialization/sitecore82";
            string str = Path.GetFullPath(SafeMapPath(serializationFolder));
            if (str[str.Length - 1] != Path.DirectorySeparatorChar)
                str = str + Path.DirectorySeparatorChar;
            return str;
        }

        private static string SafeMapPath(string path)
        {
            while (!string.IsNullOrEmpty(path) && path[0] == '/')
                path = path.Substring(1);
            return Path.Combine(GetSafeAppRoot(), path ?? string.Empty);
        }

        private static string GetSafeAppRoot()
        {
            try
            {
                return FileUtil.MapPath("/");
            }
            catch (Exception)
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }
    }
}