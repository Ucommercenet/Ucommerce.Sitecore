using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Orders;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using OrderLine = UCommerce.EntitiesV2.OrderLine;

namespace UCommerce.Sitecore.CommerceConnect.Mapping
{
	/// <summary>
	/// Everything needed for your mappings to CommerceConect.
	/// </summary>
	public static class MappingLibrary
	{
		private static MappingLibraryInternal MappingLibraryInternal
		{
			get
			{
				return ObjectFactory.Instance.Resolve<MappingLibraryInternal>();
			}
		}

		public static Cart MapPurchaseOrderToCart(PurchaseOrder purchaseOrder)
		{
			return MappingLibraryInternal.MapPurchaseOrderToCart(purchaseOrder);
		}

		public static CartLine MapOrderLineToCartLine(OrderLine orderLine)
		{
			return MappingLibraryInternal.MapOrderLineToCartLine(orderLine);
		}

		public static OrderLine MapCartLineToOrderLine(CartLine cartLine)
		{
			return MappingLibraryInternal.MapCartLineToOrderLine(cartLine);
		}

		public static Order MapPurchaseOrderToOrder(PurchaseOrder purchaseOrder)
		{
			return MappingLibraryInternal.MapPurchaseOrderToOrder(purchaseOrder);
		}

		public static Product MapCartProductToProduct(CartProduct cartProduct)
		{
			return MappingLibraryInternal.MapCartProductToProduct(cartProduct);
		}
	}
}
