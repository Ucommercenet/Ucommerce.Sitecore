using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;

namespace UCommerce.Sitecore.Extensions
{
	public static class FieldListExtensions
	{
		public static void SafeAdd(this FieldList @this, ID id, string value)
		{
			@this.Add(id, value ?? string.Empty);
		}

		public static FieldList Clone(this FieldList @this)
		{
			var clone = new FieldList();

			foreach (ID key in @this.FieldValues.Keys)
			{
				clone.Add(key, @this.FieldValues[key]);
			}

			return clone;
		}
	}
}
