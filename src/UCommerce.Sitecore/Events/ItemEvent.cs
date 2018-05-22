using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using UCommerce.Infrastructure.Globalization;
using UCommerce.Pipelines;
using SitecoreExt = global::Sitecore;

namespace UCommerce.Sitecore.Events
{
	public class ItemEvent
	{
		public void OnItemCreated(object sender, EventArgs args)
		{
			var eventArgs = args as SitecoreExt.Events.SitecoreEventArgs;

			if (eventArgs == null) return;

			var languageInfo = eventArgs.Parameters[0] as SitecoreExt.Data.Events.ItemCreatedEventArgs;

			Item item = null;

			if (languageInfo == null)
			{
				if (eventArgs.EventName == "item:deleting")
				{
					item = eventArgs.Parameters[0] as Item;
				}
			}
			else
			{
				item = languageInfo.Item;
			}

			if (item == null) return;

			if (!ItemIsLanguageFromMasterDatabase(item)) return;

			var language = LanguageManager.GetLanguage(item.Name);

			if (language == null) return;

			PipelineFactory.Create<Language>("SaveLanguage").Execute(new Language(language.CultureInfo.DisplayName, language.CultureInfo.Name));
		}

		private bool ItemIsLanguageFromMasterDatabase(Item item)
		{
			return (item.Template != null && item.Template.Key == "language" && item.Database.Name == "master");
		}
	}
}
