using System;
using System.Collections.Generic;
using Sitecore;
using Sitecore.Caching;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Ucommerce.Extensions;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates
{
	internal class StandardValuesItem : SitecoreItemWithValidName, ISitecoreItem
	{
		private readonly ID _parentTemplateId;

		public StandardValuesItem(ID parentTemplateId)
		{
			_parentTemplateId = parentTemplateId;
			ParentId = _parentTemplateId;
			Id = new ID(parentTemplateId.Guid.Derived("__Standard Values"));

            SetItemDefinition(Id, "__Standard Values", _parentTemplateId, ID.Null);
            ((ICacheable)ItemDefinition).Cacheable = SitecoreConstants.EnableCacheable;
        }

        public ID Id { get; private set; }

		public FieldList GetFieldList(VersionUri version)
		{
			return new FieldList
			{
				{ FieldIDs.Revision, Guid.NewGuid().ToString() } // New revision ensures republish of this item everytime, when using Smart publish.
            };
		}

		public bool SaveItem(ItemChanges changes)
		{
			return false;
		}

		public ID ParentId { get; set; }

		public IDList ChildIds()
		{
			return new IDList();
		}

		public bool HasChildren()
		{
			return false;
		}

		public bool IsTemplate()
		{
			return false;
		}

		public IEnumerable<ISitecoreItem> Children
		{
			get
			{
				return new List<ISitecoreItem>();
			}
		}

		public void SetRevision(Guid revision) {}
	}
}
