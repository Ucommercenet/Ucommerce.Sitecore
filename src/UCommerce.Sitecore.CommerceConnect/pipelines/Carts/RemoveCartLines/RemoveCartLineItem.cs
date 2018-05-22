using System.Linq;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Carts;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Pipelines;
using UCommerce.Pipelines.UpdateLineItem;
using UCommerce.Sitecore.CommerceConnect.Mapping;
using UCommerce.Sitecore.CommerceConnect.Pipelines.Orders;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Carts.RemoveCartLines
{
	public class RemoveCartLineItem : ServicePipelineProcessor<RemoveCartLinesRequest, CartResult>
	{
		private readonly IBasketService _basketService;

		public RemoveCartLineItem(IBasketService basketService)
		{
			_basketService = basketService;
		}

		public override void Process(ServicePipelineArgs args)
		{
			RemoveCartLinesRequest request;
			CartResult result;

			CheckParametersAndSetupRequestAndResult(args, out request, out result);

			using (new DisposableThreadLifestyleScope())
			{
				Basket basket = _basketService.GetBasketByCartExternalId(request.Cart.ExternalId);

				if (IsFromUcommerce(request))
				{
					using (new DisposableThreadLifestyleScope())
					{
						result.Cart = MappingLibrary.MapPurchaseOrderToCart(basket.PurchaseOrder);
						return;
					}
				}

				var updateLineItemPipeline = ObjectFactory.Instance.Resolve<IPipeline<IPipelineArgs<UpdateLineItemRequest, UpdateLineItemResponse>>>("UpdateLineItem");

				foreach (var cartLine in request.Lines)
				{
					var updateLineItemRequest = new UpdateLineItemRequest
					{
						OrderLine = basket.PurchaseOrder.OrderLines.FirstOrDefault(x => x.OrderLineId == int.Parse(cartLine.ExternalCartLineId)),
						Quantity = 0,
					};
					updateLineItemRequest.Properties.Add("FromUCommerce", false);

					var updateLineItemResponse = new UpdateLineItemResponse();
					updateLineItemPipeline.Execute(new UpdateLineItemPipelineArgs(updateLineItemRequest, updateLineItemResponse));
				}

				var basketPipeline = ObjectFactory.Instance.Resolve<IPipeline<PurchaseOrder>>("Basket");
				basketPipeline.Execute(basket.PurchaseOrder);

				result.Cart = MappingLibrary.MapPurchaseOrderToCart(basket.PurchaseOrder);
			}
		}
	}
}
