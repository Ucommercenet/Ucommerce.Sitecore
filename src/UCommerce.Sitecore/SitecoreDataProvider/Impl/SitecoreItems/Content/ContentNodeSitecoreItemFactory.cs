using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;

using UCommerce.Tree;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Content
{
	internal class ContentNodeSitecoreItemFactory
	{
		private readonly IList<ITemplateFieldValueProvider> _fieldValueProviders;

		public ContentNodeSitecoreItemFactory(IList<ITemplateFieldValueProvider> fieldValueProviders )
		{
			_fieldValueProviders = fieldValueProviders;
		}

		public ContentNodeSitecoreItem Create(ITreeNodeContent node)
		{
			if (!node.ItemGuid.HasValue)
			{
				throw new InvalidOperationException("Create called with node without Guid.");
			}

			var fieldValueProvider = _fieldValueProviders.SingleOrDefault(x => x.Supports(node));

			if (fieldValueProvider != null)
			{
				if (node.NodeType == Constants.DataProvider.NodeType.ProductCategory)
				{
					return new CategorySitecoreItem(node, new ID(node.ItemGuid.Value), fieldValueProvider);
				}
				
				if (node.NodeType == Constants.DataProvider.NodeType.ProductCatalog)
				{
					return new CatalogSitecoreItem(node, new ID(node.ItemGuid.Value), fieldValueProvider);
				}

				return new ContentNodeSitecoreItem(node, new ID(node.ItemGuid.Value), fieldValueProvider);
			}

			// Default. No support for this node.
			throw new InvalidOperationException("Create could not find a field value provider supporting the nodetype: " + node.NodeType);
		}

		public ContentNodeSitecoreItem Create(ITreeNodeContent node, ID parentId)
		{
			if (!node.ItemGuid.HasValue)
			{
				throw new InvalidOperationException("Create called with node without Guid.");
			}

			var fieldValueProvider = _fieldValueProviders.SingleOrDefault(x => x.Supports(node));

			if (fieldValueProvider != null)
			{
				if (node.NodeType == Constants.DataProvider.NodeType.ProductCategory)
				{
					return new CategorySitecoreItem(node, new ID(node.ItemGuid.Value), parentId, fieldValueProvider);
				}

				if (node.NodeType == Constants.DataProvider.NodeType.ProductCatalog)
				{
					return new CatalogSitecoreItem(node, new ID(node.ItemGuid.Value), parentId, fieldValueProvider);
				}

				return new ContentNodeSitecoreItem(node, new ID(node.ItemGuid.Value), parentId, fieldValueProvider);
			}

			// Default. No support for this node.
			throw new InvalidOperationException("Create could not find a field value provider supporting the nodetype: " + node.NodeType);
		}
	}
}
