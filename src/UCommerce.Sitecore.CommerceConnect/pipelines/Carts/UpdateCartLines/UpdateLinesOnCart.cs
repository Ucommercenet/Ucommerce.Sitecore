using System.Linq;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Carts;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Sitecore.CommerceConnect.Mapping;
using UCommerce.Sitecore.CommerceConnect.Pipelines.Orders;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Carts.UpdateCartLines
{
	/// <summary>
	/// Updates existing orderLines.
	/// </summary>
	public class UpdateLinesOnCart : ServicePipelineProcessor<UpdateCartLinesRequest, CartResult>
	{
		private readonly IBasketService _basketService;

		public UpdateLinesOnCart(IBasketService basketService)
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
				if (IsFromUcommerce(request))
				{
					return;
				}

				if (!result.Success) return;

				var basket = _basketService.GetBasketByCartExternalId(request.Cart.ExternalId);

				var existingOrderLines = basket.PurchaseOrder.OrderLines;

				var query = from updatedOrderLine in request.Lines
					join originalOrderLine in existingOrderLines
						on updatedOrderLine.ExternalCartLineId equals originalOrderLine.OrderLineId.ToString()
					select new {updatedOrderLine, originalOrderLine};

				foreach (var line in query.ToList())
				{
					var updatedOrderLine = MappingLibrary.MapCartLineToOrderLine(line.updatedOrderLine);
					UpdateOrderLine(updatedOrderLine, line.originalOrderLine);
				}
			}
		}

		private void UpdateOrderLine(OrderLine updatedOrderLine, OrderLine orderLine)
		{
			if (orderLine == null) return;

			orderLine.Discounts = updatedOrderLine.Discounts;
			orderLine.Total = updatedOrderLine.Total;
			orderLine.Sku = updatedOrderLine.Sku;
			orderLine.VariantSku = updatedOrderLine.VariantSku;
			orderLine.Quantity = updatedOrderLine.Quantity;
			orderLine.Price = updatedOrderLine.Price;
			orderLine.ProductName = updatedOrderLine.ProductName;
		}
	}
}
