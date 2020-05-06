using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Ucommerce.Sitecore.Extensions;
using Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Content
{
	internal abstract class SimpleSitecoreItem : SitecoreItemWithValidName, ISitecoreItem
	{
		private IDList _childIds = new IDList();
		private List<ISitecoreItem>  _children = new List<ISitecoreItem>();

		public ID Id { get; protected set; }
		public virtual FieldList GetFieldList(VersionUri version)
		{
			var result = new FieldList();
            result.SafeAdd(FieldIDs.DisplayName, DisplayName);
		    return result;
		}

		public virtual bool SaveItem(ItemChanges changes)
		{
			return false;
		}

		public ID ParentId { get; set; }
		public virtual IDList ChildIds()
		{
			return _childIds;
		}

		public virtual bool HasChildren()
		{
			return _children.Any();
		}

		public virtual bool IsTemplate()
		{
			return false;
		}

		public virtual IEnumerable<ISitecoreItem> Children
		{
			get { return _children; }
		}

		public void SetRevision(Guid revision) {}

		public virtual void AddItem(ISitecoreItem item)
		{
			item.ParentId = Id;
			_children.Add(item);
			_childIds.Add(item.Id);
		}
	}
}
