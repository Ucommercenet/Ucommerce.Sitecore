using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Entities.Prices;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetTaxTotalValuesFromOrderLine : IMapValues<OrderLine, TaxTotal>
	{
		public void MapValues(OrderLine source, TaxTotal target)
		{
			target.Amount = source.VAT;
		}
	}
}
