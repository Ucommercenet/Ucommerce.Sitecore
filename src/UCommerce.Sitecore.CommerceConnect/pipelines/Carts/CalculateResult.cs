using Sitecore;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Carts;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Pipelines;
using UCommerce.Sitecore.CommerceConnect.Mapping;
using UCommerce.Sitecore.CommerceConnect.Pipelines.Orders;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Carts
{
	/// <summary>
	/// Responsible for executing the uCommerce Basket pipeline. 
	/// </summary>
	public class CalculateResult : ServicePipelineProcessor<CartLinesRequest, CartResult>
	{
		private readonly IBasketService _basketService;

		public CalculateResult(IBasketService basketService)
		{
			_basketService = basketService;
		}

		public override void Process(ServicePipelineArgs args)
		{
			CartLinesRequest request;
			CartResult result;
			CheckParametersAndSetupRequestAndResult(args, out request, out result);

			using (new DisposableThreadLifestyleScope())
			{
				Basket basket = _basketService.GetBasketByCartExternalId(request.Cart.ExternalId);

				if (IsFromUcommerce(request))
				{

					result.Cart = MappingLibrary.MapPurchaseOrderToCart(basket.PurchaseOrder);
					return;
				}


				if (!result.Success) return;

				var basketPipeline = ObjectFactory.Instance.Resolve<IPipeline<PurchaseOrder>>("Basket");
				basketPipeline.Execute(basket.PurchaseOrder);

				result.Cart = MappingLibrary.MapPurchaseOrderToCart(basket.PurchaseOrder);

				if (string.IsNullOrEmpty(result.Cart.ShopName))
				{
					result.Cart.ShopName = Context.GetSiteName();
				}
			}
		}
	}
}
