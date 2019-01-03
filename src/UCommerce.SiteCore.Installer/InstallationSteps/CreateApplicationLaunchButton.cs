using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Install.Framework;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.InstallationSteps
{
    public class MoveFileIfTargetExist : IInstallationStep
    {
        private readonly bool _backupTarget;
        private readonly FileMoverIfTargetExist _command;

        public MoveFileIfTargetExist(string sourceVirtualPath, string targetVirtualPath, bool backupTarget)
        {
            _backupTarget = backupTarget;
            FileInfo source = new FileInfo(HostingEnvironment.MapPath(sourceVirtualPath)),
                target = new FileInfo(HostingEnvironment.MapPath(targetVirtualPath));

            _command = new FileMoverIfTargetExist(source, target);
        }

        public void Execute()
        {
            _command.MoveIfTargetExist(_backupTarget, ex => new SitecoreInstallerLoggingService().Log<int>(ex));
        }
    }

    public class CreateApplicationLaunchButton : IInstallationStep
    {
        public void Execute()
        {
            CreateApplicationShortcut();
        }

        private static readonly ID ApplicationLaunchButtonTemplateId = ID.Parse("{D4C7F5F9-8977-4E0C-B6CE-28AE8E0753E2}");
        private const string ApplicationLaunchButtonPath = "/sitecore/client/Applications/Launchpad/PageSettings/Buttons/Commerce/uCommerce";

        private void CreateApplicationShortcut()
        {
            var supportedLanguages = new Language[]
            {
                LanguageManager.GetLanguage("en"),
                LanguageManager.GetLanguage("en-US"),
                LanguageManager.GetLanguage("en-GB"),
                LanguageManager.GetLanguage("da-DK"),
                LanguageManager.GetLanguage("sv-SE"),
                LanguageManager.GetLanguage("de-DE")
            };

            foreach (var supportedLanguage in supportedLanguages)
            {
                var item = CreateItem(ApplicationLaunchButtonPath, ApplicationLaunchButtonTemplateId, supportedLanguage);

                item.Editing.BeginEdit();
                SetField(item, "text", "Commerce Settings");
                SetField(item, "icon", "/sitecore%20modules/Shell/ucommerce/css/sitecore8/images/speak/commerce-settings.png");
                SetField(item, "__icon", "/sitecore%20modules/Shell/ucommerce/images/commerce-settings.png");
                SetField(item, "__sortorder", "400");
                SetField(item, "Link", "/sitecore%20modules/Shell/ucommerce/Resources/Sitecore8/uCommerce.aspx");
                item.Appearance.Icon = "uCommerceLogo/16x16/uCommerceLogo.png";
                item.Editing.EndEdit();
            }

        }

        private Item CreateItem(string path, ID templateId, Language language)
        {
            Item item = Factory.GetDatabase("core").GetItem(path, language);

            if (item == null)
            {
                item = Factory.GetDatabase("core").CreateItemPath(path);
                item.Editing.BeginEdit();
                item.TemplateID = templateId;
                item.Editing.EndEdit();
            }

            return item;
        }

        private void SetField(Item item, string fieldName, string value)
        {
            item.Fields[fieldName].Value = value;
        }
    }
}