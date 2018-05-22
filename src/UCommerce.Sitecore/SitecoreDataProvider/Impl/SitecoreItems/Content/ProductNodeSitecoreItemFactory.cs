using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using UCommerce.EntitiesV2.Queries.Catalog;
using UCommerce.Tree.Impl;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Content
{
	internal class ProductNodeSitecoreItemFactory
	{
		private readonly IList<ITemplateFieldValueProvider> _fieldValueProviders;

		public ProductNodeSitecoreItemFactory(IList<ITemplateFieldValueProvider> fieldValueProviders)
		{
			_fieldValueProviders = fieldValueProviders;
		}

		public ContentNodeSitecoreItem Create(ProductTreeView productView, ID parentId)
		{
			var node = new TreeNodeContent(productView.ParentId == 0 ? "product" : "productVariant", productView.ProductId);
			node.Name = productView.Name;
			node.ItemGuid = productView.Guid;

			var fieldValueProvider = _fieldValueProviders.SingleOrDefault(x => x.Supports(node));

			if (fieldValueProvider != null)
			{
				return new ContentNodeSitecoreItem(node, new ID(productView.Guid), parentId, fieldValueProvider);
			}

			throw new ArgumentException("Could not create a sitecore item from this node type: " + node.NodeType);
		}
	}
}
