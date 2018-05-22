using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Carts;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Sitecore.CommerceConnect.Mapping;
using UCommerce.Sitecore.CommerceConnect.Pipelines.Orders;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Carts.CreateCart
{
	public class CreateCart : ServicePipelineProcessor<CreateOrResumeCartRequest, CartResult>
	{
		private readonly IBasketService _basketService;

		public CreateCart(IBasketService basketService)
		{
			_basketService = basketService;
		}

		public override void Process(ServicePipelineArgs args)
		{
			CreateOrResumeCartRequest request;
			CartResult result;

			CheckParametersAndSetupRequestAndResult(args, out request, out result);

			using (new DisposableThreadLifestyleScope())
			{
				var basket = _basketService.CreateBasket(true);
				var cart =
					ObjectFactory.Instance.Resolve<IMapping<PurchaseOrder, Cart>>("PurchaseOrderToCart").Map(basket.PurchaseOrder);

				cart.UserId = request.UserId;
				cart.ShopName = request.Shop.Name;
				cart.Name = request.Name;
				cart.CustomerId = request.CustomerId;
				cart.Status = CartStatus.InProcess;
				result.Cart = cart;
			}
		}
	}
}
