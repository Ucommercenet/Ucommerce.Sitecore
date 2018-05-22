using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Services.Carts;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Carts
{
	public interface ICartService
	{
		IList<CartBase> GetAll(GetCartsRequest getCartsRequest);
		Cart GetByCartId(string cartId, string shopName);
		IQueryable<CartBase> GetByUserName(string userName, string shopName);
	}
}
