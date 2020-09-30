using Sitecore.Data.Items;
using Sitecore.Links.UrlBuilders;
using Sitecore.Resources.Media;
using Ucommerce.Infrastructure.Logging;
using Ucommerce.Sitecore;

namespace Ucommerce.Sitecore93.Content
{
    public class SitecoreImageService : Ucommerce.Sitecore.Content.SitecoreImageService
    	{
	        protected override string GetMediaUrl(Item item)
            {
	            return MediaManager.GetMediaUrl(item, new MediaUrlBuilderOptions { AlwaysIncludeServerUrl = false });
            }

	        public SitecoreImageService(ILoggingService loggingService, ISitecoreContext sitecoreContext) : base(loggingService, sitecoreContext)
	        {
	        }
        }
}