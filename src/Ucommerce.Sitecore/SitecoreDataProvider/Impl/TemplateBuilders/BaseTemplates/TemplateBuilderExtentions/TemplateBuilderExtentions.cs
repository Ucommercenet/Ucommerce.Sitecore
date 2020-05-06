using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Ucommerce.EntitiesV2;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders.BaseTemplates.TemplateBuilderExtentions
{
	internal static class TemplateBuilderExtentions
	{
		/// <summary>
		/// Tells if a fieldchange has actual changes.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public static bool ValueDidNotChangeFor(this ITemplateBuilder templateBuilder, FieldChange fieldChange)
		{
			return fieldChange.Value == fieldChange.OriginalValue;
		}

		/// <summary>
		/// Finds out weather the field type belongs to the statistics section in Sitecore.
		/// </summary>
		/// <remarks>
		/// As we do NOT want to handle statistics part inside the dataprovider (Underlying repositories takes care of updating those)
		/// We need to know weather field type is one of those kinds so we can ignore them when saving entities.
		/// </remarks>
		public static bool FieldBelongsToStatistics(this ITemplateBuilder templateBuilder, FieldChange fieldChange)
		{
			return fieldChange.Definition.Template.ID == ID.Parse(SitecoreConstants.SitecoreStatisticsSectionId);
		}

		/// <summary>
		/// Returns the correct field value based on the field type.
		/// </summary>
		public static string GetFieldSpecificValue(this ITemplateBuilder templateBuilder, FieldChange fieldChange, Item item)
		{
			switch (fieldChange.Definition.Type)
			{
				case SitecoreConstants.FieldTypeDropdown:
					return GetFieldSpecificValueForDropDownList(fieldChange);
				case SitecoreConstants.FieldTypeCheckboxList:
					return GetFieldSpecificValueForCheckboxList(fieldChange);
				case SitecoreConstants.FieldTypeImage:
					return GetImageId(fieldChange, item);
				case SitecoreConstants.FieldTypeNumber:
			        return ConvertLocalizedStringToDecimal(fieldChange.Value, Context.Language.CultureInfo).ToString();
				case SitecoreConstants.FieldTypeBoolean:
					return fieldChange.Value == "1" ? "True" : "False";
				case SitecoreConstants.FieldTypeDatetime:
					return
						DateUtil.IsoDateToDateTime(fieldChange.Value).ToUniversalTime().ToString("u");
				default:
					return fieldChange.Value;
			}
		}

		public static void UpdateProductPricing(this ITemplateBuilder templateBuilder, FieldChange fieldChange, Item item, Product product, int priceGroupId)
		{
			var productPrice = product.ProductPrices.SingleOrDefault(x => x.Price.PriceGroup.PriceGroupId == priceGroupId && x.MinimumQuantity == 1);

			if (productPrice == null)
			{
			    productPrice = new ProductPrice()
				{
                    MinimumQuantity = 1,
                    Guid = Guid.NewGuid(),
                    Product = product,
                    Price = new Price()
                    {
                        Amount = 0m,
                        Guid = Guid.NewGuid(),
                        PriceGroup = PriceGroup.Get(priceGroupId)
                    }
				};

                product.ProductPrices.Add(productPrice);
			}

		    productPrice.Price.Amount = ConvertLocalizedStringToDecimal(fieldChange.Value, Context.Language.CultureInfo);
		}

		private static string GetFieldSpecificValueForDropDownList(FieldChange fieldChange)
		{
			if (string.IsNullOrEmpty(fieldChange.Value)) return fieldChange.Value;

			return Database.GetDatabase(SitecoreConstants.SitecoreMasterDatabaseName).GetItem(ID.Parse(fieldChange.Value)).Name;
		}
		private static string GetFieldSpecificValueForCheckboxList(FieldChange fieldChange)
		{
			var val = fieldChange.Value;
			var parts = val.Split('|');
			var names = new List<string>();
			foreach (var part in parts)
			{
				Item item = Database.GetDatabase(SitecoreConstants.SitecoreMasterDatabaseName).GetItem(ID.Parse(part));
				if (item != null)
					names.Add(item.Name);
			}
			return string.Join("|", names);
		}

		private static string GetImageId(FieldChange fieldChange, Item item)
		{
			ImageField imageField = new Field(fieldChange.FieldID, item);

			return imageField.MediaID.ToString() == "{00000000-0000-0000-0000-000000000000}"
				? null
				: imageField.MediaID.ToString();
		}

	    private static decimal ConvertLocalizedStringToDecimal(string value, CultureInfo culture)
	    {
	        if (string.IsNullOrEmpty(value))
	            value = "0";

            // Sitecore formats all number values into english, unless you edit the raw values directly.
            var cultureForConvert = (value.Contains(culture.NumberFormat.CurrencyDecimalSeparator))
                ? culture
                : CultureInfo.InvariantCulture;

            return System.Convert.ToDecimal(value, cultureForConvert.NumberFormat);
	    }
	}
}
