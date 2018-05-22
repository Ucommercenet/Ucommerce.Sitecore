using System.Collections.Generic;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using UCommerce.Extensions;
using UCommerce.Sitecore.Extensions;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Content
{
	internal class ClonedProductNodeSitecoreItem : SimpleSitecoreItem
	{
		private readonly ContentNodeSitecoreItem _original;
		private bool _childrenLoaded;
		private readonly object _lock = new object();

		private const string SourcePrefix = "sitecore://master/{0}?lang={1}&ver={2}";

		// Example of working source value.
		// sitecore://master/{FF566796-F489-4A67-AD8B-4F431C65092A}?lang=en&ver=1

		public ClonedProductNodeSitecoreItem(ID parentId, ContentNodeSitecoreItem original)
		{
			_original = original;
			ParentId = parentId;
			Id = new ID(parentId.Guid.Derived(original.Id.Guid.ToString()));
			SetItemDefinition(Id, _original.ItemDefinition.Name, _original.ItemDefinition.TemplateID, _original.ItemDefinition.BranchId);
		}

		private void LoadChildrenIfNeeded()
		{
			if (!_childrenLoaded)
			{
				lock (_lock)
				{
					if (!_childrenLoaded)
					{
						foreach (var child in _original.Children)
						{
							var asProductNodeSitecoreItem = child as ContentNodeSitecoreItem;
							if (asProductNodeSitecoreItem != null)
							{
								var clonedChild = new ClonedProductNodeSitecoreItem(Id, asProductNodeSitecoreItem);
								AddItem(clonedChild);
							}
						}

						_childrenLoaded = true;
					}
				}
			}
		}

		public override bool SaveItem(ItemChanges changes)
		{
			return _original.SaveItem(changes);
		}

		public override IDList ChildIds()
		{
			LoadChildrenIfNeeded();
			return base.ChildIds();
		}

		public override IEnumerable<ISitecoreItem> Children
		{
			get
			{
				LoadChildrenIfNeeded();
				return base.Children;
			}
		}

		public override bool HasChildren()
		{
			LoadChildrenIfNeeded();
			return base.HasChildren();
		}

		public override FieldList GetFieldList(VersionUri version)
		{
			var fields = new FieldList();
			fields.SafeAdd(FieldIDs.Source, string.Format(SourcePrefix, _original.Id, version.Language.Name, version.Version.Number));
			fields.SafeAdd(FieldIDs.NeverPublish, true.ToSitecoreFormat());

			return fields;
		}
	}
}
