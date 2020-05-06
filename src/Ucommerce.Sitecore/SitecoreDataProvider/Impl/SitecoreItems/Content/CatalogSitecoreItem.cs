using Sitecore.Data;
using Ucommerce.Tree;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Content
{
	internal class CatalogSitecoreItem : ContentNodeSitecoreItem
	{
		public CatalogSitecoreItem(ITreeNodeContent node, ID id, ITemplateFieldValueProvider fieldValueProvider) : base(node, id, fieldValueProvider)
		{
		}

		public CatalogSitecoreItem(ITreeNodeContent node, ID id, ID parentId, ITemplateFieldValueProvider fieldValueProvider)
			: base(node, id, parentId, fieldValueProvider)
		{
		}
	}
}
