﻿using Sitecore.Data;
using Ucommerce.Tree;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Content
{
	internal class CategorySitecoreItem : ContentNodeSitecoreItem
	{
		public CategorySitecoreItem(ITreeNodeContent node, ID id, ITemplateFieldValueProvider fieldValueProvider) : base(node, id, fieldValueProvider)
		{
		}

		public CategorySitecoreItem(ITreeNodeContent node, ID id, ID parentId, ITemplateFieldValueProvider fieldValueProvider) : base(node, id, parentId, fieldValueProvider)
		{
		}
	}
}
