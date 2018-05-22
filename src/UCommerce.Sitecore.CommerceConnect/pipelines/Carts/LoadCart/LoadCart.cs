using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Carts;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Carts.LoadCart
{
	public class LoadCart : ServicePipelineProcessor<LoadCartRequest, CartResult>
	{
		private readonly ICartService _cartService;

		public LoadCart(ICartService cartService)
		{
			_cartService = cartService;
		}

		public override void Process(ServicePipelineArgs args)
		{
			LoadCartRequest request;
			CartResult result;

			CheckParametersAndSetupRequestAndResult(args, out request, out result);
			
			if (result.Cart != null)
				return;

			var cart = _cartService.GetByCartId(request.CartId, request.Shop.Name);
			if (cart == null) return;

			cart.UserId = request.UserId;

			result.Cart = cart;
		}
	}
}
