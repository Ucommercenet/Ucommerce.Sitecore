using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Entities.Carts;

namespace UCommerce.Sitecore.CommerceConnect.Mapping
{
	public class SetOrderLineNumbersService : ISetOrderLineNumbersService
	{
		public void SetOrderLineNumbersForCartLines(IList<CartLine> target)
		{
			for (var i = 0; i < target.Count; i++)
				target[i].LineNumber = Convert.ToUInt32(i+1);
		}
	}
}
