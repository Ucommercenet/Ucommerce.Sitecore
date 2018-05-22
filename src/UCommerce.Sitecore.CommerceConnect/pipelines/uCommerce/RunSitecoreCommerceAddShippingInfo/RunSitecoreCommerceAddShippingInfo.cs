using System.Collections.Generic;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Services.Carts;
using UCommerce.Pipelines;
using UCommerce.Pipelines.CreateShipment;
using UCommerce.Sitecore.CommerceConnect.Mapping;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.UCommerce.RunSitecoreCommerceAddShippingInfo
{
	public class RunSitecoreCommerceAddShippingInfo : IPipelineTask<IPipelineArgs<CreateShipmentRequest, CreateShipmentResult>>
	{
		public PipelineExecutionResult Execute(IPipelineArgs<CreateShipmentRequest, CreateShipmentResult> subject)
		{
			if (subject.Request.Properties.ContainsKey("FromUCommerce"))
				if (!(bool)subject.Request.Properties["FromUCommerce"]) return PipelineExecutionResult.Success;

			var cart = MappingLibrary.MapPurchaseOrderToCart(subject.Request.PurchaseOrder);

			var cartService = new CartServiceProvider();
			var shippingList = new List<ShippingInfo>
			{
				new ShippingInfo() { ShippingMethodID = subject.Request.ShippingMethod.ShippingMethodId.ToString() }
			};
			var request = new AddShippingInfoRequest(cart, shippingList);
			request.Properties["FromUCommerce"] = true;

			var result = cartService.AddShippingInfo(request);

			return PipelineExecutionResult.Success;
		}
	}
}
