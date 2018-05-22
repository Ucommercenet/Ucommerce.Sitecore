using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Prices;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetCartValuesFromPurchaseOrder : IMapValues<PurchaseOrder, Cart>
	{
		private readonly IMapping<PurchaseOrder, IList<CartAdjustment>> _purchaseOrderToAdjustments;
		private readonly IMapping<PurchaseOrder, Total> _purchaseOrderToTotal;
		private readonly IMapping<ICollection<OrderLine>, IList<CartLine>> _orderLinesToCartLines;
		private readonly IMapping<ICollection<OrderAddress>, IList<Party>> _orderAddressesToParties;
		private readonly IMapping<ICollection<Shipment>, IList<ShippingInfo>> _shipmentsToShipping;
		private readonly IMapping<ICollection<Payment>, IList<PaymentInfo>> _paymentsToPayment;

		public SetCartValuesFromPurchaseOrder(IMapping<PurchaseOrder, IList<CartAdjustment>> purchaseOrderToAdjustments,
			IMapping<PurchaseOrder, Total> purchaseOrderToTotal, IMapping<ICollection<OrderLine>, IList<CartLine>> orderLinesToCartLines,
			IMapping<ICollection<OrderAddress>, IList<Party>> orderAddressesToParties, IMapping<ICollection<Shipment>,
			IList<ShippingInfo>> shipmentsToShipping, IMapping<ICollection<Payment>, IList<PaymentInfo>> paymentsToPayment)
		{
			_purchaseOrderToAdjustments = purchaseOrderToAdjustments;
			_purchaseOrderToTotal = purchaseOrderToTotal;
			_orderLinesToCartLines = orderLinesToCartLines;
			_orderAddressesToParties = orderAddressesToParties;
			_shipmentsToShipping = shipmentsToShipping;
			_paymentsToPayment = paymentsToPayment;
		}

		public void MapValues(PurchaseOrder purchaseOrder, Cart cart)
		{
			cart.Adjustments = new ReadOnlyCollection<CartAdjustment>(_purchaseOrderToAdjustments.Map(purchaseOrder));
			cart.Total = _purchaseOrderToTotal.Map(purchaseOrder);
			cart.Lines = new ReadOnlyCollection<CartLine>(_orderLinesToCartLines.Map(purchaseOrder.OrderLines));
			cart.Parties = new ReadOnlyCollection<Party>(_orderAddressesToParties.Map(purchaseOrder.OrderAddresses));
			cart.Shipping = new ReadOnlyCollection<ShippingInfo>(_shipmentsToShipping.Map(purchaseOrder.Shipments));
			cart.Payment = new ReadOnlyCollection<PaymentInfo>(_paymentsToPayment.Map(purchaseOrder.Payments));
		}
	}
}
