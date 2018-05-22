using System.Collections.ObjectModel;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Services.Carts;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Pipelines.UpdateLineItem;
using UCommerce.Sitecore.CommerceConnect.Mapping;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.UCommerce.RunSitecoreCommerceUpdateLineItem
{
	public class RunSitecoreCommerceUpdateLineItem : IPipelineTask<IPipelineArgs<UpdateLineItemRequest, UpdateLineItemResponse>>
	{
		public PipelineExecutionResult Execute(IPipelineArgs<UpdateLineItemRequest, UpdateLineItemResponse> subject)
		{
			if (subject.Request.Properties.ContainsKey("FromUCommerce"))
				if (!(bool)subject.Request.Properties["FromUCommerce"]) return PipelineExecutionResult.Success;

			if (subject.Request.Quantity > 0)
			{
				var cart = MappingLibrary.MapPurchaseOrderToCart(subject.Request.OrderLine.PurchaseOrder);
				var cartLine = MappingLibrary.MapOrderLineToCartLine(subject.Request.OrderLine);
				var cartServiceProvider = new CartServiceProvider();

				var request = new UpdateCartLinesRequest(cart, new Collection<CartLine> { cartLine });
				request.Properties["FromUCommerce"] = true;
				cartServiceProvider.UpdateCartLines(request);
			}
			return PipelineExecutionResult.Success;

		}
	}
}
