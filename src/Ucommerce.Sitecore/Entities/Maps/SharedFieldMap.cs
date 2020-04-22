using Ucommerce.EntitiesV2.Maps;

namespace Ucommerce.Sitecore.Entities.Maps
{
	public class SharedFieldMap : BaseClassMap<SharedField>
	{
		public SharedFieldMap()
		{
			Cache.ReadWrite();
			Cache.Region("FieldValues");

			Id(x => x.Id);
			Map(x => x.ItemId).Generated.Never();
			Map(x => x.FieldId).Generated.Never();
			Map(x => x.FieldValue).Length(4001); // Length will override NH default of 4k chars for strings.
		}
	}
}
