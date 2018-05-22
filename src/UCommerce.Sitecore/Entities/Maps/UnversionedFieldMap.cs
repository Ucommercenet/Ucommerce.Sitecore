using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCommerce.EntitiesV2.Maps;

namespace UCommerce.Sitecore.Entities.Maps
{
	public class UnversionedFieldMap : BaseClassMap<UnversionedField>
	{
		public UnversionedFieldMap()
		{
			Cache.ReadWrite();
			Cache.Region("FieldValues");

			Id(x => x.Id);
			Map(x => x.ItemId).Generated.Never();
			Map(x => x.FieldId).Generated.Never();
			Map(x => x.Language);
			Map(x => x.FieldValue).Length(4001); // Length will override NH default of 4k chars for strings.
		}
	}
}
