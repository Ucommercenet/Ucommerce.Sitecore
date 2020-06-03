using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Conventions;
using Sitecore.Data;
using Ucommerce.EntitiesV2.Definitions;
using Ucommerce.Extensions;

namespace Ucommerce.Sitecore.Extensions
{
	public static class IPropertyExtensions
	{
		public static string ToSitecoreFormat(this IProperty @this)
		{
			var dataType = @this.GetDefinitionField().DataType;
			switch (dataType.DefinitionName)
			{
				case "Number":
					var numValue = @this.GetValue() == null || string.IsNullOrEmpty(@this.GetValue().ToString())
						? "0"
						: @this.GetValue<decimal>().ToString();
					return numValue;
				case "ShortText":
				case "LongText":
				case "RichText":
				case "Content": // TODO: How should Content be displayed?
					return @this.GetValue() as string;
				case "Media":
					return string.Format("<image mediaid=\"{0}\" />", @this.GetValue());
				case "DateTime":
				case "Date":
					var valueAsString = (string) @this.GetValue();
					if (string.IsNullOrEmpty(valueAsString))
					{
						return string.Empty;
					}
					var value = @this.GetValue<DateTime>();
					return value.ToSitecoreFormat();
				case "Boolean":
					if (@this.GetValue() == null) return false.ToSitecoreFormat();
					return (@this.GetValue().ToString() == "True").ToSitecoreFormat();
				case "Enum":
					var dataTypeEnum = dataType.DataTypeEnums.FirstOrDefault(x => x.Value == @this.GetValue().ToString());

					if (dataTypeEnum == null)
					{
						return string.Empty;
						// The enum selected is possibly deleted. In that case, we return an empty selection, signified by the empty string.
					}

					return new ID(dataTypeEnum.Guid).ToString();
				case "EnumMultiSelect":
					var val = (string) @this.GetValue();
					var parts = val.Split('|');
					var ids = new List<string>();

					foreach (var part in parts)
					{
						var dtEnum = dataType.DataTypeEnums.FirstOrDefault(x => x.Value == part);
						if (dtEnum != null)
						{
							ids.Add(new ID(dtEnum.Guid).ToString());
						}
					}

					if (ids.IsEmpty())
					{
						return string.Empty;
					}

					return string.Join("|", ids);
				default:
					// Default behaviour is to use the "raw" value for types we do not know.
					return @this.GetValue() as string;
			}
		}

		public static bool RenderForCulture(this IProperty @this, string cultureInfo)
		{
			return (string.IsNullOrEmpty(@this.CultureCode) || @this.CultureCode == cultureInfo)
					&& @this.GetDefinitionField().RenderInEditor;
		}
	}
}
