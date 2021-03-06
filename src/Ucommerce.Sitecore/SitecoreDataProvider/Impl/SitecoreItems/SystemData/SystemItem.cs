﻿using System;
using System.Collections.Generic;
using Sitecore;
using Sitecore.Caching;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Ucommerce.Sitecore.Extensions;
using Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.SystemData
{
	internal class SystemItem : SitecoreItemWithValidName, ISitecoreItem
	{
		private readonly IDList _childrensIds;
		private readonly FieldList _fieldList;
		private readonly List<ISitecoreItem> _children;

		private string ItemName { get; set; }

		public SystemItem(ID id, string name)
		{
			Id = id;
			ItemName = name;

			_childrensIds = new IDList();
			_children = new List<ISitecoreItem>();
			_fieldList = new FieldList();

            SetItemDefinition(Id, ItemName, TemplateIDs.StandardTemplate, ID.Null);
            ((ICacheable)ItemDefinition).Cacheable = SitecoreConstants.EnableCacheable;

            AddToFieldList(FieldIDs.DisplayName, DisplayName);
        }

        public ID Id { get; private set; }

		public FieldList GetFieldList(VersionUri version)
		{
			return _fieldList.Clone();
		}

		public void AddToFieldList(ID fieldId, string value)
		{
			_fieldList.SafeAdd(fieldId,value);
		}

		public bool SaveItem(ItemChanges changes)
		{
			return false;
		}

		public ID ParentId { get; set; }

		public IDList ChildIds()
		{
			return _childrensIds;
		}

		public bool HasChildren()
		{
			return _childrensIds.Count > 0;
		}

		public bool IsTemplate()
		{
			return false;
		}

		public IEnumerable<ISitecoreItem> Children
		{
			get { return _children; }
		}

		public void SetRevision(Guid revision)
		{
			AddToFieldList(FieldIDs.Revision, revision.ToString());
			foreach (var child in Children)
			{
				child.SetRevision(revision);
			}
		}

		public void AddItem(ISitecoreItem item)
		{
			item.ParentId = Id;
			_childrensIds.Add(item.Id);
			_children.Add(item);
		}
	}
}
