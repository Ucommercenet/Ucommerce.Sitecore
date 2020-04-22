using System.Collections.Generic;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.StandardTemplateFields
{
	public class StandardTemplateFieldValueConfiguration : ISitecoreStandardTemplateFieldValueConfiguration
	{
		public StandardTemplateFieldValueConfiguration(IEnumerable<string> whiteListTemplates, IEnumerable<string> blackListFields)
		{
			var temp = new List<string>();
			temp.AddRange(whiteListTemplates);

			WhiteListTemplateIds = temp;

			temp = new List<string>();
			temp.AddRange(blackListFields);

			BlackListFieldIds = temp;
		}

		public IEnumerable<string> WhiteListTemplateIds { get; private set; }

		public IEnumerable<string> BlackListFieldIds { get; private set; }
	}
}
