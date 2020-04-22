using System;
using System.Collections.Generic;
using Sitecore;
using Sitecore.Caching;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates
{
	internal class SectionItem : SitecoreItemWithValidName, ISitecoreItem
	{
		private string SectionName { get; set; }
		private readonly IDList _children = new IDList();
		private readonly List<FieldItem> _fields = new List<FieldItem>();
		private int SortOrder { get; set; }
		private Guid Revision { get; set; }

		public SectionItem(ID id, string name, int sortOrder)
		{
			Id = id;
			SectionName = name;
			SortOrder = sortOrder;

            SetItemDefinition(Id, SectionName, TemplateIDs.TemplateSection, ID.Null);
            ((ICacheable)ItemDefinition).Cacheable = SitecoreConstants.EnableCacheable;
        }

        public ID Id { get; private set; }

		public FieldList GetFieldList(VersionUri version)
		{
			return new FieldList
					{
						{ FieldIDs.Sortorder, SortOrder.ToString() },
						{ FieldIDs.Revision, Revision.ToString()},
                        { FieldIDs.DisplayName, DisplayName }
					};
		}

		public bool SaveItem(ItemChanges changes)
		{
			throw new NotSupportedException();
		}

		public ID ParentId { get; set; }
		public IDList ChildIds()
		{
			return _children;
		}

		public bool HasChildren()
		{
			return _children.Count > 0;
		}

		public bool IsTemplate()
		{
			return false;
		}

		public IEnumerable<ISitecoreItem> Children
		{
			get { return _fields; }
		}

		public void SetRevision(Guid revision)
		{
			Revision = revision;

			foreach (var child in Children)
			{
				child.SetRevision(revision);
			}
		}

		public void AddField(FieldItem field)
		{
			field.ParentId = Id;
			_children.Add(field.Id);
			_fields.Add(field);
		}

		public IList<FieldItem> Fields
		{
			get { return _fields; }
		}

		public TemplateSection.Builder Build(Template.Builder builder)
		{
			var sectionBuilder = builder.AddSection(SectionName, Id);
			sectionBuilder.SetSortorder(SortOrder);

			return sectionBuilder;
		}
	}
}
