using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Orders;
using UCommerce.EntitiesV2;
using OrderLine = UCommerce.EntitiesV2.OrderLine;

namespace UCommerce.Sitecore.CommerceConnect.Mapping
{
	/// <summary>
	/// Implementation of the Mapping API.
	/// </summary>
	public class MappingLibraryInternal
	{
		private readonly IMapping<PurchaseOrder, Cart> _mapPurchaseOrderToCart;
		private readonly IMapping<OrderLine, CartLine> _mapOrderLineToCartLine;
		private readonly IMapping<CartLine, OrderLine> _mapCartLineToOrderLine;
		private readonly IMapping<PurchaseOrder, Order> _mapPurchaseOrderToOrder;
		private readonly IMapping<CartProduct, Product> _mapCartProductToProduct;

		public MappingLibraryInternal(IMapping<PurchaseOrder, Cart> mapPurchaseOrderToCart,
			IMapping<OrderLine, CartLine> mapOrderLineToCartLine,
			IMapping<CartLine, OrderLine> mapCartLineToOrderLine, 
			IMapping<PurchaseOrder, Order> mapPurchaseOrderToOrder, 
			IMapping<CartProduct, Product> mapCartProductToProduct)
		{
			_mapPurchaseOrderToCart = mapPurchaseOrderToCart;
			_mapOrderLineToCartLine = mapOrderLineToCartLine;
			_mapCartLineToOrderLine = mapCartLineToOrderLine;
			_mapPurchaseOrderToOrder = mapPurchaseOrderToOrder;
			_mapCartProductToProduct = mapCartProductToProduct;
		}

		public virtual Cart MapPurchaseOrderToCart(PurchaseOrder purchase)
		{
			return _mapPurchaseOrderToCart.Map(purchase);
		}

		public virtual CartLine MapOrderLineToCartLine(OrderLine orderLine)
		{
			return _mapOrderLineToCartLine.Map(orderLine);
		}

		public virtual OrderLine MapCartLineToOrderLine(CartLine cartLine)
		{
			return _mapCartLineToOrderLine.Map(cartLine);
		}

		public virtual Order MapPurchaseOrderToOrder(PurchaseOrder purchaseOrder)
		{
			return _mapPurchaseOrderToOrder.Map(purchaseOrder);
		}

		public virtual Product MapCartProductToProduct(CartProduct cartProduct)
		{
			return _mapCartProductToProduct.Map(cartProduct);
		}
	}
}
