using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Carts;
using Sitecore.Diagnostics;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Pipelines;
using UCommerce.Pipelines.RemoveShipment;
using UCommerce.Sitecore.CommerceConnect.Mapping;
using UCommerce.Sitecore.CommerceConnect.Pipelines.Orders;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Carts.RemoveShippingInfo
{
	public class RemoveShippingInfoFromCart : ServicePipelineProcessor<RemoveShippingInfoRequest, RemoveShippingInfoResult>
    {
		private readonly IBasketService _basketService;
		private readonly IMapping<ICollection<Shipment>, IList<ShippingInfo>> _shipmentMapper;
		private readonly IMapping<PurchaseOrder, Cart> _purchaseOrderMapper;

		public RemoveShippingInfoFromCart(IBasketService basketService)
		{
			_basketService = basketService;
			_shipmentMapper = ObjectFactory.Instance.Resolve<IMapping<ICollection<Shipment>, IList<ShippingInfo>>>();
			_purchaseOrderMapper = ObjectFactory.Instance.Resolve<IMapping<PurchaseOrder, Cart>>();
		}

		public override void Process(ServicePipelineArgs args)
		{
			RemoveShippingInfoRequest request;
			RemoveShippingInfoResult result;

			using (new DisposableThreadLifestyleScope())
			{
                CheckParametersAndSetupRequestAndResult(args, out request, out result);

				Assert.ArgumentNotNull(request.ShippingInfo, "request.ShippingInfo");
				Assert.ArgumentNotNull(request.Cart, "request.Cart");
				Assert.ArgumentCondition(request.ShippingInfo.Count != 0, "request.ShippingInfo",
					"request.ShippingInfo cannot be empty");

				var purchaseOrder = _basketService.GetBasketByCartExternalId(request.Cart.ExternalId).PurchaseOrder;
				var removeShipmentPipeline = PipelineFactory.Create<IPipelineArgs<RemoveShipmentRequest, RemoveShipmentResult>>("RemoveShipment");

				if (!IsFromUcommerce(request))
				{
					foreach (var shippingInfo in request.ShippingInfo)
					{
						var shipment = GetShipmentFromPurchaseOrder(purchaseOrder, shippingInfo);
						var removeShipmentPipelineArgs = new RemoveShipmentPipelineArgs(new RemoveShipmentRequest()
						{
							PurchaseOrder = purchaseOrder,
							Shipment = shipment
						}, new RemoveShipmentResult());

						removeShipmentPipelineArgs.Request.Properties["FromUCommerce"] = false;
						removeShipmentPipeline.Execute(removeShipmentPipelineArgs);
					}

					PipelineFactory.Create<PurchaseOrder>("Basket").Execute(purchaseOrder);
				}

				var shippinInfo = _shipmentMapper.Map(purchaseOrder.Shipments);
				result.ShippingInfo = new ReadOnlyCollection<ShippingInfo>(shippinInfo);
				result.Cart = _purchaseOrderMapper.Map(purchaseOrder);
			}
		}

		private Shipment GetShipmentFromPurchaseOrder(PurchaseOrder purchaseOrder, ShippingInfo shippingInfo)
		{
			int shipmentId;
			if (!int.TryParse(shippingInfo.ExternalId, out shipmentId))
			{
				throw new ArgumentException(string.Format("Could not parse: {0} into an int, shippingInfo.ExternalId needs to be an int", shippingInfo.ExternalId));
			}

			var shipment = purchaseOrder.Shipments.FirstOrDefault(x => x.ShipmentId == shipmentId);
			if (shipment == null)
			{
				throw new ArgumentException(string.Format("No shipment with id: {0} could be found", shipmentId));
			}

			return shipment;
		}
	}
}
