using System.Collections.Specialized;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Install.Framework;

namespace Ucommerce.Sitecore.Installer.Steps
{
	public class CreateApplicationShortcuts : IPostStep
	{
		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			CreateApplication();
			CreateApplicationShortcut();
		}

		private static readonly ID ApplicationTemplateId = ID.Parse("{EB06CEC0-5E2D-4DC4-875B-01ADCC577D13}");
		private const string ApplicationPath = "/sitecore/content/Applications/uCommerce";

		private static readonly ID ApplicationShortcutTemplateId = ID.Parse("{72450C9C-98C4-4117-88B7-573110C7E0C0}");
		private const string ApplicationShortcutPath = "/sitecore/content/Documents and settings/All users/Start menu/Left/uCommerce";
		private ID ApplicationId;

		private void SetField(Item item, string fieldName, string value)
		{
			item.Fields[fieldName].Value = value;
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

		private void CreateApplication()
		{
			var supportedLanguages = new Language[]
			{
				LanguageManager.GetLanguage("en"),
				LanguageManager.GetLanguage("en-US"),
				LanguageManager.GetLanguage("en-GB"),
				LanguageManager.GetLanguage("da-DK"),
				LanguageManager.GetLanguage("da"),
				LanguageManager.GetLanguage("sv-SE"),
				LanguageManager.GetLanguage("de-DE")
			};

			foreach (var supportedLanguage in supportedLanguages)
			{
				var item = CreateItem(ApplicationPath, ApplicationTemplateId, supportedLanguage);
				item.Editing.BeginEdit();
				SetField(item, "tool tip", GetLanguageToolTipVersion(supportedLanguage));
				SetField(item, "__sortorder", "0");
				SetField(item, "icon", "/sitecore/shell/themes/standard/uCommerceLogo/16x16/uCommerceLogo.png");
				SetField(item, "__icon", "/sitecore/shell/themes/standard/uCommerceLogo/16x16/uCommerceLogo.png");
				SetField(item, "display name", "uCommerce");
				SetField(item, "window mode", "Maximized");
				SetField(item, "application", @"<link url=""/sitecore%20modules/Shell/ucommerce/shell/index.html"" linktype=""internal"" id="""" />");
				item.Appearance.Icon = "/sitecore/shell/themes/standard/uCommerceLogo/16x16/uCommerceLogo.png";
				item.Editing.EndEdit();

				//save Ucommerce application Id for further use when creating the shortcut in Start Menu in Sitecore
				ApplicationId = item.ID;
			}
		}

		private string GetLanguageToolTipVersion(Language supportedLanguage)
		{
			switch (supportedLanguage.CultureInfo.Name)
			{
				case "da":
					return "Håndter dine online butikker";
				case "da-DK":
					return "Håndter dine online butikker";
				case "sv-SE":
					return "Hantera dina nätbutiker";
				case "de-DE":
					return "Verwalten Sie Ihre Online-Shops";
			}
			return "Manage your online stores";
		}

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
				//use the ApplicationId for the shortcut
				var applicationFieldValueForShortcut =
					string.Format(
						@"<link linktype=""internal"" url=""/Applications/uCommerce"" querystring="""" target="""" id=""{0}"" />",
						ApplicationId);
				var item = CreateItem(ApplicationShortcutPath, ApplicationShortcutTemplateId, supportedLanguage);

				item.Editing.BeginEdit();
				SetField(item, "display name", "uCommerce");
				SetField(item, "icon", "uCommerceLogo/16x16/uCommerceLogo.png");
				SetField(item, "__icon", "uCommerceLogo/16x16/uCommerceLogo.png");
				SetField(item, "application", applicationFieldValueForShortcut);
				SetField(item, "__sortorder", "0");
				item.Appearance.Icon = "uCommerceLogo/16x16/uCommerceLogo.png";
				item.Editing.EndEdit();
			}
		}
	}
}