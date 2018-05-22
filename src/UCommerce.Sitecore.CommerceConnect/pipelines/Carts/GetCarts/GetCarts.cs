using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Carts;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Carts.GetCarts
{
	public class GetCarts : ServicePipelineProcessor<GetCartsRequest, GetCartsResult>
	{
		private readonly ICartService _cartService;

		public GetCarts(ICartService cartService)
		{
			_cartService = cartService;
		}

		public override void Process(ServicePipelineArgs args)
		{
			GetCartsRequest request;
			GetCartsResult result;

			CheckParametersAndSetupRequestAndResult(args, out request, out result);

			result.Carts = _cartService.GetAll((request));
		}
	}
}
