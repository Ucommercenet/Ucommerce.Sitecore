using System.Linq;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Carts;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Sitecore.CommerceConnect.Pipelines.Orders;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Carts.UpdateCartLines
{
	/// <summary>
	/// Runs addCartLines if cartLines doesn't exist.
	/// </summary>
	public class RunAddCartLines : ServicePipelineProcessor<UpdateCartLinesRequest, CartResult>
	{
		private readonly IBasketService _basketService;

		public RunAddCartLines(IBasketService basketService)
		{
			_basketService = basketService;
		}

		public override void Process(ServicePipelineArgs args)
		{
			UpdateCartLinesRequest request;
			CartResult result;

			CheckParametersAndSetupRequestAndResult(args, out request, out result);

			using (new DisposableThreadLifestyleScope())
			{
				if (IsFromUcommerce(request) || !args.Result.Success) return;

				var basket = _basketService.GetBasketByCartExternalId(request.Cart.ExternalId);

				var existingOrderLines = basket.PurchaseOrder.OrderLines;

				var query = (from updatedOrderLine in request.Lines
					join originalOrderLine in existingOrderLines
						on updatedOrderLine.ExternalCartLineId equals originalOrderLine.OrderLineId.ToString() into matchedOrderLines
					from matchedOrderLine in matchedOrderLines.DefaultIfEmpty()
					where matchedOrderLine == null
					select new {NewCartLine = updatedOrderLine}).ToList();

				if (!query.Any()) return;

				var newCartLines = query.Select(x => x.NewCartLine).ToList();

				var cartServiceProvider = new CartServiceProvider();
				var addCartLinesRequest = new AddCartLinesRequest(request.Cart, newCartLines);
				cartServiceProvider.AddCartLines(addCartLinesRequest);
			}
		}
	}
}
