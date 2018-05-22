using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Caching;
using Sitecore.Data;
using Sitecore.Reflection;
using UCommerce.Content;
using UCommerce.EntitiesV2;
using UCommerce.EntitiesV2.Definitions;
using UCommerce.Extensions;
using UCommerce.Infrastructure;
using UCommerce.Sitecore.SitecoreDataProvider;
using UCommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders.BaseTemplates;

namespace UCommerce.Sitecore.Extensions
{
	public static class IdExtensions
	{
	    public static void EvictFromSitecoresItemCache(this Guid @this)
	    {
	        new ID(@this).EvictFromSitecoresItemCache();
	    }

	    public static void EvictFromSitecoresItemCache(this ID @this)
	    {
            var sitecoreContext = ObjectFactory.Instance.Resolve<ISitecoreContext>();

            var item = sitecoreContext.MasterDatabase.GetItem(@this);
            if (item != null)
            {
                ReflectionUtil.CallMethod(
                    typeof(ItemCache), CacheManager.GetItemCache(sitecoreContext.MasterDatabase),
                    "RemoveItem", true, true, new object[] { @this });
            }
        }

        // DataTypeEnum
        public static ID SitecoreId(this DataTypeEnum @this)
		{
			return new ID(@this.Guid);
		}

		// DataType
		public static ID SitecoreIdForEnum(this DataType @this)
		{
			return new ID(@this.Guid);
		}

		// Price group
		public static ID SitecoreId(this PriceGroup @this)
		{
			return new ID(@this.Guid);
		}

		// Currency
		public static ID SitecoreId(this Currency @this)
		{
			return new ID(@this.Guid);
		}

		// Email profiles
		public static ID SitecoreId(this EmailProfile @this)
		{
			return new ID(@this.Guid);
		}

		// Member groups
		public static ID SitecoreId(this MemberGroup @this)
		{
			return new ID(SitecoreConstants.MemberGroupItemBaseId.Derived(@this.MemberGroupId));
		}

		// Member types
		public static ID SitecoreId(this MemberType @this)
		{
			return new ID(SitecoreConstants.MemberTypeItemBaseId.Derived(@this.MemberTypeId));
		}

		// Order number series
		public static ID SitecoreId(this OrderNumberSerie @this)
		{
			return new ID(@this.Guid);
		}

		// Domain
		public static ID SitecoreId(this Domain @this)
		{
			return new ID(SitecoreConstants.DomainItemBaseId.Derived(@this.DomainId));
		}

		// Product Definition Templates.
		public static ID SitecoreTemplateId(this ProductDefinition @this)
		{
			return new ID(@this.Guid);
		}

		public static IEnumerable<ID> GetSitecoreBaseTemplateIds(this ProductDefinition @this)
		{
			var result = new List<ID>();

			if (@this.GetParentDefinitions().Any())
			{
				foreach (var parent in @this.GetParentDefinitions().Cast<ProductDefinition>())
				{
					result.Add(parent.SitecoreTemplateId());
				}
			}
			else
			{
				result.Add(FieldIds.Product.ProductBaseTemplateId);
			}

			return result;
		}

		// Product Definition Templates Variants.
		public static ID SitecoreVariantTemplateId(this ProductDefinition @this)
		{
			return new ID(@this.Guid.Derived("VariantTemplate"));
		}

		public static IEnumerable<ID> GetSitecoreBaseVariantTemplateIds(this ProductDefinition @this)
		{
			var result = new List<ID>();

			if (@this.GetParentDefinitions().Any())
			{
				foreach (var parent in @this.GetParentDefinitions().Cast<ProductDefinition>())
				{
					result.Add(parent.SitecoreVariantTemplateId());
				}
			}
			else
			{
				result.Add(BaseVariantTemplate.VariantBaseTemplateId);
			}

			return result;
		}

		// Definition
		public static ID SitecoreTemplateId(this IDefinition @this)
		{
			return new ID(@this.Guid);
		}

		public static IEnumerable<ID> GetBaseTemplateIds(this IDefinition @this)
		{
			var result = new List<ID>();

			if (@this.GetParentDefinitions().Any())
			{
				foreach (var parent in @this.GetParentDefinitions())
				{
					result.Add(parent.SitecoreTemplateId());
				}
			}
			else
			{
				result.Add(FieldIds.Category.ProductCategoryTemplateId);
			}

			return result;
		}

		#region ID's for Template parts
		// ID values used for Template parts. I.e. Sections and Fields.
		// Price group field
		public static ID SitecoreTemplateFieldId(this PriceGroup @this, ID parentId)
		{
			return new ID(@this.Guid.Derived("TemplateField"));
		}

		// Price group field
		public static ID SitecoreTemplateFieldIdForVariant(this PriceGroup @this, ID parentId)
		{
			return new ID(@this.Guid.Derived("VariantField"));
		}

		public static ID SitecoreTemplateSectionDynamicDefinitions(this ProductDefinition @this, ID parentId)
		{
			return new ID(@this.Guid.Derived("TemplateSectionDynamicDefinitions"));
		}

		public static ID SitecoreTemplateField(this ProductDefinitionField @this, ID parentId)
		{
			var id = new ID(@this.Guid);
			DynamicFieldIds.Add(id);
			return id;
		}

		public static ID SitecoreTemplateSectionDynamicDefinitionsForVariant(this ProductDefinition @this, ID parentId)
		{
			return new ID(@this.Guid.Derived("TemplateSectionDynamicDefinitionsForVariant"));
		}

		public static ID SitecoreTemplateFieldForVariant(this ProductDefinitionField @this, ID parentId)
		{
			// A field cannot be both a variant field and not be a variant field. Therefore there is no clash when using the guid as is.
			var id = new ID(@this.Guid);
			DynamicFieldIds.Add(id);
			return id;
		}

		// Definition
		public static ID SitecoreDynamicFieldsSectionId(this IDefinition @this, ID parentId)
		{
			return new ID(@this.Guid.Derived("DynamicFieldsSection"));
		}

		// Definition Field
		public static ID GetOrCreateSitecoreDynamicFieldFieldId(this DefinitionField @this, ID parentId)
		{
			var id = new ID(@this.Guid);
			DynamicFieldIds.Add(id);
			return id;
		}

		public static ID GetOrCreateAllowedPriceGroupFieldId(this PriceGroup @this, ID parentId)
		{
			return new ID(@this.Guid.Derived("AllowedPriceGroupField"));
		}
		#endregion

		#region Helpers
		private static readonly HashSet<ID> DynamicFieldIds = new HashSet<ID>();
		
		public static bool IsUCommerceDynamicField(this ID @this)
		{
			return DynamicFieldIds.Contains(@this);
		}

		#endregion Helpers
	}
}
