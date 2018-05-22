using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Orders;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Pipelines;
using UCommerce.Sitecore.CommerceConnect.Mapping;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Orders.SubmitVisitorOrder
{
    public class CartToOrderUCommerce : ServicePipelineProcessor<SubmitVisitorOrderRequest, SubmitVisitorOrderResult>
    {
        private readonly IBasketService _basketService;

        public CartToOrderUCommerce(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public override void Process(ServicePipelineArgs args)
        {
	        SubmitVisitorOrderRequest request;
	        SubmitVisitorOrderResult result;

			using (new DisposableThreadLifestyleScope())
            {
                CheckParametersAndSetupRequestAndResult(args, out request, out result);

				// Check if this call originates from the uCommerce "Checkout" pipeline.
	            var ucommerceOrder = request.Properties["UCommerceOrder"] as PurchaseOrder;
	            if (ucommerceOrder == null)
	            {
					// The call originates from Commerce Connect.
					// It is very important, that this run of the SubmitVisitorOrder pipeline
					// does nothing, except call the Checkout pipeline.
		            var basketPurchaseOrder = _basketService.GetBasketByCartExternalId(request.Cart.ExternalId);
		            if (basketPurchaseOrder == null)
		            {
			            result.Success = false;
			            return;
		            }

					// First call the Checkout pipeline.
		            ExecuteCheckoutPipeline(basketPurchaseOrder.PurchaseOrder);

					// Make sure nothing else in the pipeline is run.
		            result.Success = false;
		            result.Order = null;
	            }
	            else
	            {
					// Set the order, and signal, that the rest of the pipeline should be run.
					result.Order = MappingLibrary.MapPurchaseOrderToOrder(ucommerceOrder);
		            result.Success = true;
	            }
			}
        }

        private PurchaseOrder ExecuteCheckoutPipeline(PurchaseOrder basketPurchaseOrder)
        {
            var checkoutPipeline = PipelineFactory.Create<PurchaseOrder>("Checkout");
            var result = checkoutPipeline.Execute(basketPurchaseOrder);

            return result == PipelineExecutionResult.Error ? null : basketPurchaseOrder;
        }
    }
}
