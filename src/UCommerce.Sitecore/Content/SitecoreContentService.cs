using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;
using Sitecore.Sites;
using UCommerce.Content;
using UCommerce.Infrastructure.Logging;
using Links = Sitecore.Links;

namespace UCommerce.Sitecore.Content
{
	public class SitecoreContentService : IContentService
	{
		private readonly ILoggingService _loggingService;
		private readonly ISitecoreContext _sitecoreContext;

		public SitecoreContentService(ILoggingService loggingService, ISitecoreContext sitecoreContext)
		{
			_loggingService = loggingService;
			_sitecoreContext = sitecoreContext;
		}

		/// <summary>
		/// Resolves a ContentUrl from an id.
		/// </summary>
		/// <param name="contentId">Id of a sitecore item.</param>
		/// <returns>Url to a given item from sitecore.</returns>
		public virtual UCommerce.Content.Content GetContent(string contentId)
		{
			var content = new UCommerce.Content.Content
				{
					Id = contentId,
					Name = "",
					Url = ""
				};

			if (string.IsNullOrEmpty(contentId))
				return content;

			var item = GetItemFromId(contentId);

			if (item == null)
			{
				_loggingService.Log<SitecoreContentService>(string.Format("Item with id: {0} was not found. Check that content exists in database",contentId));
				return content;
			}

		    var urlOptions = new Links.UrlOptions()
		    {
		        AlwaysIncludeServerUrl = true,
                Language = Context.Language,
                Site = SiteContext.Current
		    };
            
            content.Url = Links.LinkManager.LanguageEmbedding.ToString().ToLower() == "never"
                ? Links.LinkManager.GetDynamicUrl(item, new Links.LinkUrlOptions() { Language = Context.Language })
                : Links.LinkManager.GetItemUrl(item, urlOptions);

		    content.Icon = "/~/icon/" + item.Appearance.Icon;
            content.Name = item.Name;

			return content;
		}

		/// <summary>
		/// Returns a sitecore item based on id.
		/// </summary>
		/// <param name="id">id of the sitecore item.</param>
		/// <returns>Sitecore item</returns>
		/// <remarks>
		/// Runs on Context object, initialized on each Http request, using configured 
		/// database from the site detected by SiteCore, which is configured in web.config under sites node.
		/// </remarks>
		protected virtual Item GetItemFromId(string id)
		{
			return _sitecoreContext.DatabaseForContent.GetItem(id);
		}
	}
}
