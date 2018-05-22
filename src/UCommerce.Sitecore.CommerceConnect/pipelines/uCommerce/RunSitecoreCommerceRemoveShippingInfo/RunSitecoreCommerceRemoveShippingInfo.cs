using System;
using System.Collections.Generic;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Services.Carts;
using UCommerce.Pipelines;
using UCommerce.Pipelines.RemoveShipment;
using UCommerce.Sitecore.CommerceConnect.Mapping;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.UCommerce.RunSitecoreCommerceRemoveShippingInfo
{
	public class RunSitecoreCommerceRemoveShippingInfo : IPipelineTask<IPipelineArgs<RemoveShipmentRequest, RemoveShipmentResult>>
	{
		public PipelineExecutionResult Execute(IPipelineArgs<RemoveShipmentRequest, RemoveShipmentResult> subject)
		{
			if (subject.Request.Properties.ContainsKey("FromUCommerce"))
				if (!(bool)subject.Request.Properties["FromUCommerce"]) return PipelineExecutionResult.Success;

			var cart = MappingLibrary.MapPurchaseOrderToCart(subject.Request.PurchaseOrder);

			var cartService = new CartServiceProvider();
			var shippingList = new List<ShippingInfo>
			{
				new ShippingInfo()
				{
					ExternalId = subject.Request.Shipment.ShipmentId.ToString(), 
					PartyID = subject.Request.Shipment.ShipmentAddress.OrderAddressId. ToString(), 
					ShippingMethodID = subject.Request.Shipment.ShippingMethod.ShippingMethodId.ToString()
				}
			};
			var request = new RemoveShippingInfoRequest(cart, shippingList);
			request.Properties["FromUCommerce"] = true;

			var result = cartService.RemoveShippingInfo(request);

			return PipelineExecutionResult.Success;
		}
	}
}
