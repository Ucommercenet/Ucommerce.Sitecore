using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Entities.Carts;

namespace UCommerce.Sitecore.CommerceConnect.Mapping
{
	public interface ISetOrderLineNumbersService
	{
		void SetOrderLineNumbersForCartLines(IList<CartLine> target);
	}
}
