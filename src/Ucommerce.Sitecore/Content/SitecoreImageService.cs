using System.IO;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;
using Ucommerce.Content;
using Ucommerce.Infrastructure.Components.Windsor;
using Ucommerce.Infrastructure.Logging;
using Ucommerce.Infrastructure.Runtime;
using Ucommerce.Web;

namespace Ucommerce.Sitecore.Content
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
        public virtual Ucommerce.Content.Content GetImage(string contentId)
        {
            var content = new Ucommerce.Content.Content();
            if (string.IsNullOrEmpty(contentId))
            {
                return content;
            }

            var item = GetItemFromId(contentId);

            if (item == null)
            {
                _loggingService.Information<SitecoreContentService>(
                    string.Format("Item with id: {0} was not found. Check that content exists in database", contentId));
                return ImageNotFound(contentId);
            }

            return MapItemToContent(item);
        }

        protected virtual Ucommerce.Content.Content MapItemToContent(Item item)
        {
            return new Ucommerce.Content.Content()
            {
                Name = item.Name,
                Icon = item.Appearance.Icon,
                Url = GetMediaUrl(item),
                Id = item.ID.ToString(),
                NodeType = GetNodeType(item)
            };
        }

        protected virtual string GetNodeType(Item item)
        {
            var folderTemplateKeys = new[] {"media folder", "node"};
            var imageTemplateKey = "image";

            if (folderTemplateKeys.Contains(item.Template.Key))
            {
                return Constants.ImagePicker.Folder;
            }

            if (item.Template.Key == imageTemplateKey ||
                item.Template.BaseTemplates.Any(x => x.Key == imageTemplateKey))
            {
                return Constants.ImagePicker.Image;
            }

            return Constants.ImagePicker.File;
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

        protected virtual string GetMediaUrl(Item item)
        {
            return MediaManager.GetMediaUrl(item, new MediaUrlOptions {AlwaysIncludeServerUrl = false});
        }

        private Ucommerce.Content.Content ImageNotFound(string id)
        {
            return new Ucommerce.Content.Content
            {
                Id = id,
                Name = "image-not-found.png",
                Url = "/sitecore modules/Shell/Ucommerce/images/ui/image-not-found.png",
                NodeType = Constants.ImagePicker.Image
            };
        }
    }
}