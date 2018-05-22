using Sitecore.Data.Items;
using Sitecore.Resources.Media;
using UCommerce.Content;
using UCommerce.Infrastructure.Logging;

namespace UCommerce.Sitecore.Content
{
	public class SitecoreImageService : IImageService
	{
		private readonly ILoggingService _loggingService;
		private readonly ISitecoreContext _sitecoreContext;

		public SitecoreImageService(ILoggingService loggingService, ISitecoreContext sitecoreContext)
		{
			_loggingService = loggingService;
			_sitecoreContext = sitecoreContext;
		}

		/// <summary>
		/// Resolves an imageUrl from an id.
		/// </summary>
		/// <param name="contentId">Id of a sitecore item.</param>
		/// <returns>Link to an .ashx which resolves to an image.</returns>
		/// <remarks>Url must be absolute and return an .ashx page which is the way that SiteCore stores and resolves images.</remarks>
		public virtual UCommerce.Content.Content GetImage(string contentId)
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
				_loggingService.Log<SitecoreContentService>(string.Format("Item with id: {0} was not found. Check that content exists in database", contentId));
				return content;
			}

			content.Name = item.Name;
			content.Url = MediaManager.GetMediaUrl(item, new MediaUrlOptions { AlwaysIncludeServerUrl = true });
		    content.Icon = item.Appearance.Icon;

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
		private Item GetItemFromId(string id)
		{
			return _sitecoreContext.DatabaseForContent.GetItem(id);
		}
	}
}
