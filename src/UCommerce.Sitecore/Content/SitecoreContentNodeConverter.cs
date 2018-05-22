using Sitecore.Data;
using UCommerce.Content;
using UCommerce.Tree;
using UCommerce.Web;
using UCommerce.Web.Models;

namespace UCommerce.Sitecore.Content
{
	public class SitecoreContentNodeConverter : ITreeNodeContentToTreeNodeConverter
	{
		private readonly IContentService _contentService;
		private readonly ISitecoreContext _sitecoreContext;

		public SitecoreContentNodeConverter(IContentService contentService, ISitecoreContext sitecoreContext)
		{
			_contentService = contentService;
			_sitecoreContext = sitecoreContext;
		}

		public virtual NodeItem Convert(ITreeNodeContent node)
		{
			var item = _sitecoreContext.DatabaseForContent.GetItem(ID.Parse(node.ItemId));
			
			return new NodeItem()
			{
				Id = item.ID.Guid.ToString(),
				Name = item.DisplayName,
				NodeType = node.NodeType,
				Icon = "/~/icon/" + item.Appearance.Icon,
				HasChildren = item.HasChildren,
				AutoLoad = node.AutoLoad,
				DimNode = false,
				Url = _contentService.GetContent(item.ID.Guid.ToString()).Url
			};
		}
	}
}
