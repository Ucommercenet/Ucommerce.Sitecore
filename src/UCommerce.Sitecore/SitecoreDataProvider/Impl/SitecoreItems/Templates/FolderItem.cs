using System;
using System.Collections.Generic;
using Sitecore;
using Sitecore.Caching;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates
{
	internal class FolderItem : SitecoreItemWithValidName, ISitecoreItem
	{
		protected readonly IDList _childrensIds;
		protected readonly List<ISitecoreItem> _children;
		protected readonly FieldList _fieldList;
		protected string FolderName { get; set; }

		public FolderItem(ID id, string name)
		{
			Id = id;
			FolderName = name;
			_fieldList = new FieldList();
			_childrensIds = new IDList();
			_children = new List<ISitecoreItem>();

            SetItemDefinition(Id, FolderName, TemplateIDs.TemplateFolder, ID.Null);
            ((ICacheable)ItemDefinition).Cacheable = SitecoreConstants.EnableCacheable;

            AddToFieldList(FieldIDs.DisplayName, DisplayName);
        }

        public ID Id { get; private set; }
	
		public virtual FieldList GetFieldList(VersionUri version)
		{
			return _fieldList;
		}

		public virtual void AddToFieldList(ID fieldId, string value)
		{
			_fieldList.Add(fieldId,value);
		}

		public virtual bool SaveItem(ItemChanges changes)
		{
			return false;
		}

		public virtual ID ParentId { get; set; }

		public virtual IDList ChildIds()
		{
			return _childrensIds;
		}

		public virtual bool HasChildren()
		{
			return _childrensIds.Count > 0;
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
			_childrensIds.Add(item.Id);
			_children.Add(item);
		}
	}
}
