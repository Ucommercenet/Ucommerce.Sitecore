using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Caching;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using Sitecore.Globalization;
using Ucommerce.Sitecore.Extensions;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates
{
	internal class FieldItem : SitecoreItemWithValidName, ISitecoreItem
	{
		private string FieldName { get; set; }
		private string Icon { get; set; }
		private string Type { get; set; }
		private string Style { get; set; }
		private string Source { get; set; }

		private int SortOrder { get; set; }

		private bool Shared { get; set; }
		private bool Unversioned { get; set; }

		private Guid Revision { get; set; }

		private List<KeyValuePair<string, string>> DisplayNamesByCultureCodes { get; set; }

		public FieldItem(ID id, string name, string icon, int sortOrder, string style, string type, bool shared, bool unversioned, string source, List<KeyValuePair<string, string>> displayNamesByCultureCodes)
		{
			Id = id;
			FieldName = name;
			Icon = icon;
			SortOrder = sortOrder;
			Style = style;
			Type = type;
			Shared = shared;
			Source = source;
			Unversioned = unversioned;
			DisplayNamesByCultureCodes = displayNamesByCultureCodes;

            SetItemDefinition(Id, FieldName, TemplateIDs.TemplateField, ID.Null);
            ((ICacheable)ItemDefinition).Cacheable = SitecoreConstants.EnableCacheable;
        }

        public ID Id { get; private set; }

		public string Validation { get; set; }

		public string ValidationText { get; set; }

		public FieldList GetFieldList(VersionUri version)
		{
			var fields = new FieldList();
			fields.SafeAdd(FieldIDs.Icon, Icon);
			fields.SafeAdd(FieldIDs.Style, Style);
			fields.SafeAdd(FieldIDs.Sortorder, SortOrder.ToString());
			fields.SafeAdd(TemplateFieldIDs.Type, Type);
			fields.SafeAdd(TemplateFieldIDs.Shared, Shared.ToSitecoreFormat());
			fields.SafeAdd(TemplateFieldIDs.Unversioned, Unversioned.ToSitecoreFormat());

			if (DisplayNamesByCultureCodes.Any(x => x.Key == version.Language.CultureInfo.Name))
			{
				fields.SafeAdd(TemplateFieldIDs.Title, DisplayNamesByCultureCodes.First(x => x.Key == version.Language.CultureInfo.Name).Value);
			}

			if (!string.IsNullOrEmpty(Source))
			{
				fields.SafeAdd(TemplateFieldIDs.Source, Source);
			}

			if (!string.IsNullOrWhiteSpace(Validation))
			{
				fields.SafeAdd(TemplateFieldIDs.Validation, Validation);
			}

			if (!string.IsNullOrWhiteSpace(ValidationText))
			{
				fields.SafeAdd(TemplateFieldIDs.ValidationText, ValidationText);
			}

			fields.SafeAdd(FieldIDs.Revision, Revision.ToString());

            fields.SafeAdd(FieldIDs.DisplayName, DisplayName);

			return fields;
		}

		public bool SaveItem(ItemChanges changes)
		{
			throw new NotSupportedException();
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

		public IEnumerable<ISitecoreItem> Children { get { return new List<ISitecoreItem>(); } }
		public void SetRevision(Guid revision)
		{
			Revision = revision;
		}

		public void Build(TemplateSection.Builder sectionBuilder)
		{
			var fieldBuilder = sectionBuilder.AddField(FieldName, Id);

			fieldBuilder.SetIcon(Icon);
			fieldBuilder.SetShared(Shared);
			fieldBuilder.SetSortorder(SortOrder);
			fieldBuilder.SetStyle(Style);
			fieldBuilder.SetType(Type);
			fieldBuilder.SetUnversioned(Unversioned);

			foreach (var displayNameAndCultureCode in DisplayNamesByCultureCodes)
			{
				var cultureCode = displayNameAndCultureCode.Key;
				var displayName = displayNameAndCultureCode.Value;

				try
				{
					fieldBuilder.SetTitle(displayName, Language.Parse(cultureCode));
				}
				catch (ArgumentException)
				{
					// Can happen, if the culture code is a custom code, not installed on the machine.
					fieldBuilder.SetTitle(displayName, Language.Parse("en"));
				}
			}
		}
	}
}
