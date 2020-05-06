using Sitecore;
using Sitecore.Caching;
using Sitecore.Data;
using Sitecore.Data.Items;
using Ucommerce.Sitecore.Extensions;
using Ucommerce.Tree;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Content
{
	internal class ContentNodeSitecoreItem : SimpleSitecoreItem
	{
		public readonly ITreeNodeContent Node;
		protected readonly ITemplateFieldValueProvider FieldValueProvider;

		private bool _hasChildrenIsAlwaysTrue;

		public ContentNodeSitecoreItem(ITreeNodeContent node, ID id, ITemplateFieldValueProvider fieldValueProvider)
		{
			Node = node;
			FieldValueProvider = fieldValueProvider;
			Id = id;

			var templateId = FieldValueProvider.GetTemplateId(node);
			SetItemDefinition(Id, node.Name, templateId, ID.Null);
			((ICacheable)ItemDefinition).Cacheable = SitecoreConstants.EnableCacheable;
		}

		public ContentNodeSitecoreItem(ITreeNodeContent node, ID id, ID parentId, ITemplateFieldValueProvider fieldValueProvider) : this(node, id, fieldValueProvider)
		{
			ParentId = parentId;
		}

		public void HasChildrenAlwaysTrue()
		{
			_hasChildrenIsAlwaysTrue = true;
		}

		public override FieldList GetFieldList(VersionUri version)
		{
			var fieldList = new FieldList();
			fieldList.SafeAdd(FieldIDs.Icon, GetIcon());
			fieldList.SafeAdd(FieldIDs.Sortorder, Node.SortOrder.ToString());
            fieldList.SafeAdd(FieldIDs.DisplayName, DisplayName);

			FieldValueProvider.AddFieldValues(Node, fieldList, version);
			return fieldList;
		}

		public override bool SaveItem(ItemChanges changes)
		{
			return FieldValueProvider.SaveItem(Node, changes);
		}

		public override bool HasChildren()
		{
			if (_hasChildrenIsAlwaysTrue) return true;
			return base.HasChildren();
		}

		public string GetIcon()
		{
			string iconPart = null;
			switch (Node.NodeType)
			{
				case Constants.DataProvider.NodeType.Product:
				case Constants.DataProvider.NodeType.ProductVariant:
					iconPart = "tag_green.png";
					break;
				case Constants.DataProvider.NodeType.ProductCategory:
					iconPart = "report.png";
					break;
				case Constants.DataProvider.NodeType.ProductCatalog:
					iconPart = "layout.png";
					break;
				case Constants.DataProvider.NodeType.ProductCatalogGroup:
					iconPart = "cart.png";
					break;
				case Constants.DataProvider.NodeType.Catalog:
					iconPart = "box.png";
					break;
				case Constants.DataProvider.NodeType.Root:
					iconPart = "ucommerce-logo-icon.png";
					break;
			}

			return "/sitecore modules/Shell/ucommerce/shell/Content/Images/ui/" + iconPart;
		}
	}
}
