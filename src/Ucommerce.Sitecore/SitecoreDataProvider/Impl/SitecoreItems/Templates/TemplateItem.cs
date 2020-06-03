using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Caching;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates
{
	internal class TemplateItem : SitecoreItemWithValidName, ISitecoreItem
	{
		public TemplateItem(ID id, string name, ID baseId)
			: this(id, name, new List<ID> { baseId }) { }

		public TemplateItem(ID id, string name, IEnumerable<ID> baseIds)
		{
			Id = id;
			TemplateName = name;
			BaseIds = baseIds;
			ParentId = ID.Parse("{B29EE504-861C-492F-95A3-0D890B6FCA09}");
			_fieldList = new FieldList();

            SetItemDefinition(Id, TemplateName, TemplateIDs.Template, ID.Null);
            ((ICacheable)ItemDefinition).Cacheable = SitecoreConstants.EnableCacheable;

            AddToFieldList(FieldIDs.DisplayName, DisplayName);
        }

        private string TemplateName { get; set; }
		private IEnumerable<ID> BaseIds { get; set; }
		private ID StandardValuesHolderId { get; set; }
		private readonly IDList _childrenIds = new IDList();
		private readonly List<ISitecoreItem> _children = new List<ISitecoreItem>();
		private readonly List<SectionItem> _sections = new List<SectionItem>();
		private FieldList _fieldList;

		public ID Id { get; private set; }

		public void AddToFieldList(ID fieldId, string value)
		{
			_fieldList.Add(fieldId, value);
		}

		public FieldList GetFieldList(VersionUri version)
		{
			EnsureStandardValues();
			return _fieldList;
		}

		private void EnsureStandardValues()
		{
			if (_fieldList[FieldIDs.BaseTemplate] == null)
				_fieldList.Add(FieldIDs.BaseTemplate,  BaseIdsValue);

			if (_fieldList[FieldIDs.StandardValues] == null)
				_fieldList.Add(FieldIDs.StandardValues, StandardValuesHolderId.ToString());

			if (_fieldList[FieldIDs.StandardValueHolderId] == null)
				_fieldList.Add(FieldIDs.StandardValueHolderId, StandardValuesHolderId.ToString());
		}

		private string BaseIdsValue
		{
			get { return string.Join("|", BaseIds.Select(x => x.ToString())); }
		}

		public bool SaveItem(ItemChanges changes)
		{
			throw new NotSupportedException();
		}

		public ID ParentId { get; set; }

		public IDList ChildIds()
		{
			return _childrenIds;
		}

		public bool HasChildren()
		{
			return _childrenIds.Count > 0;
		}

		public bool IsTemplate()
		{
			return true;
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

		public void AddSection(SectionItem section)
		{
			section.ParentId = Id;
			_childrenIds.Add(section.Id);
			_sections.Add(section);
			_children.Add(section);
		}

		public void AddStandardValues(StandardValuesItem standardValues)
		{
			StandardValuesHolderId = standardValues.Id;
			standardValues.ParentId = Id;
			_childrenIds.Add(standardValues.Id);
			_children.Add(standardValues);
		}

		public IList<SectionItem> Sections
		{
			get { return _sections; }
		}

		public Template.Builder BuildTemplate(TemplateCollection owner)
		{
			var builder = new Template.Builder(TemplateName, Id, owner);
			builder.SetFullName(TemplateName);
			builder.SetBaseIDs(BaseIdsValue);

			return builder;
		}
	}
}
