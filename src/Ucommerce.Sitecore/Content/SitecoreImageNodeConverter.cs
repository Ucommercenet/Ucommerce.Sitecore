using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;
using Ucommerce.Content;
using Ucommerce.Tree;
using Ucommerce.Web;
using Ucommerce.Web.Models;

namespace Ucommerce.Sitecore.Content
{
	internal class SitecoreImageNodeConverter : ITreeNodeContentToTreeNodeConverter
	{
		private readonly IImageService _imageService;
		private readonly ISitecoreContext _sitecoreContext;

		public SitecoreImageNodeConverter(IImageService imageService, ISitecoreContext sitecoreContext)
		{
			_imageService = imageService;
			_sitecoreContext = sitecoreContext;
		}

		public virtual NodeItem Convert(ITreeNodeContent node)
		{
			return CreateNodeFromItem(_sitecoreContext.DatabaseForContent.GetItem(ID.Parse(node.ItemId)), node.AutoLoad, node.NodeType);
		}

		private NodeItem CreateNodeFromItem(Item item, bool autoload, string nodeType)
		{
			var mediaitem = MediaManager.GetMedia(item);

			return new NodeItem()
			{
				Id = item.ID.Guid.ToString(),
				Name = item.DisplayName,
				NodeType = nodeType,
				Icon = "/~/icon/" + item.Appearance.Icon,
				HasChildren = item.HasChildren,
				ChildrenCount = item.Children.Count,
				AutoLoad = autoload,
				DimNode = false,
				Url = mediaitem.MediaData.HasContent ? _imageService.GetImage(item.ID.ToString()).Url : "",
				Guid = item.ID.Guid
			};
		}
	}
}
