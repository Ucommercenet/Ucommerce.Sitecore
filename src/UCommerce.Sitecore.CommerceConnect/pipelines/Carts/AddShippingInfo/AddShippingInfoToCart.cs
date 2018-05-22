using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Carts;
using Sitecore.Diagnostics;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Pipelines;
using UCommerce.Pipelines.CreateShipment;
using UCommerce.Sitecore.CommerceConnect.Mapping;
using UCommerce.Sitecore.CommerceConnect.Pipelines.Orders;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Carts.AddShippingInfo
{
	public class AddShippingInfoToCart : ServicePipelineProcessor<AddShippingInfoRequest, AddShippingInfoResult>
	{
		private readonly IBasketService _basketService;
		private readonly IMapping<ICollection<Shipment>, IList<ShippingInfo>> _shipmentMapper;
		private readonly IMapping<PurchaseOrder, Cart> _purchaseOrderMapper;

		public AddShippingInfoToCart(IBasketService basketService)
		{
			_basketService = basketService;
			_shipmentMapper = ObjectFactory.Instance.Resolve<IMapping<ICollection<Shipment>, IList<ShippingInfo>>>();
			_purchaseOrderMapper = ObjectFactory.Instance.Resolve<IMapping<PurchaseOrder, Cart>>();
		}

		public override void Process(ServicePipelineArgs args)
		{
			AddShippingInfoRequest request;
			AddShippingInfoResult result;

			using (new DisposableThreadLifestyleScope())
			{
				CheckParametersAndSetupRequestAndResult(args, out request, out result);

				Assert.ArgumentNotNull(request.ShippingInfo, "request.ShippingInfo");
				Assert.ArgumentNotNull(request.Cart, "request.Cart");
				Assert.ArgumentCondition(request.ShippingInfo.Count != 0, "request.ShippingInfo",
					"request.ShippingInfo cannot be empty");

				Assert.ArgumentCondition(request.ShippingInfo.Count == 1, "request.ShippingInfo",
					"request.ShippingInfo cannot contain more than 1 ShippingInfo");

				var shipments = new List<Shipment>();
				var purchaseOrder = _basketService.GetBasketByCartExternalId(request.Cart.ExternalId).PurchaseOrder;

				if (!IsFromUcommerce(request))
				{
					var createShipmentPipeline = PipelineFactory.Create<IPipelineArgs<CreateShipmentRequest, CreateShipmentResult>>("CreateShipment");

					var createShipmentPipelineArgs = new CreateShipmentPipelineArgs(new CreateShipmentRequest()
					{
						ShippingMethod = GetShippingMethod(request.ShippingInfo.First()),
						OverwriteExisting = false,
						PurchaseOrder = purchaseOrder,
						ShippingAddress = GetOrderAddress(GetPartyFromCart(request.ShippingInfo.First(), request.Cart))
					}, new CreateShipmentResult());

					createShipmentPipelineArgs.Request.Properties["FromUCommerce"] = false;

					createShipmentPipeline.Execute(createShipmentPipelineArgs);
					shipments.Add(createShipmentPipelineArgs.Response.Shipment);

					PipelineFactory.Create<PurchaseOrder>("Basket").Execute(purchaseOrder);
				}
				else
				{
					shipments.Add(purchaseOrder.Shipments.Last());
				}
				
				var shippingInfos = _shipmentMapper.Map(shipments);
				result.ShippingInfo = new ReadOnlyCollection<ShippingInfo>(shippingInfos);
				result.Cart = _purchaseOrderMapper.Map(purchaseOrder);
			}
		}

		private OrderAddress GetOrderAddress(Party party)
		{
			int orderAddressId;
			if (!int.TryParse(party.ExternalId, out orderAddressId))
			{
				throw new ArgumentException(string.Format("Could not parse: {0} into an int, party.ExternalId needs to be an int", party.ExternalId));
			}

			var orderAddressRepository = ObjectFactory.Instance.Resolve<IRepository<OrderAddress>>();
			var orderAddress = orderAddressRepository.Get(orderAddressId);
			if (orderAddress == null)
			{
				throw new ArgumentException(string.Format("No order address with id: {0} could be found", party.PartyId));
			}

			return orderAddress;
		}

		private ShippingMethod GetShippingMethod(ShippingInfo shippingInfo)
		{
			int shippingMethodId;
			if (!int.TryParse(shippingInfo.ShippingMethodID, out shippingMethodId))
			{
				throw new ArgumentException(string.Format("Could not parse: {0} into an int, shippingInfo.ShippingMethodID needs to be an int", shippingInfo.ShippingMethodID));
			}

			var shippingMethodRepository = ObjectFactory.Instance.Resolve<IRepository<ShippingMethod>>();
			var shippingMethod = shippingMethodRepository.Get(shippingMethodId);
			if (shippingMethod == null)
			{
				throw new ArgumentException(string.Format("No shipping method with id: {0} could be found", shippingMethodId));
			}

			return shippingMethod;
		}

		private Party GetPartyFromCart(ShippingInfo shippingInfo, Cart cart)
		{
			var party = cart.Parties.FirstOrDefault(x => x.PartyId == shippingInfo.PartyID);
			if (party == null)
			{
				throw new ArgumentException(string.Format("Cannot find party with PartyId: \"{0}\" in Cart.Parties", shippingInfo.PartyID));
			}

			return party;
		}
	}
}
