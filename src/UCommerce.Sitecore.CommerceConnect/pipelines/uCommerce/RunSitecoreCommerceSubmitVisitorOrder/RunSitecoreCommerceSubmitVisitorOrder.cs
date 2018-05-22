using Sitecore;
using Sitecore.Commerce.Contacts;
using Sitecore.Commerce.Services.Carts;
using Sitecore.Commerce.Services.Orders;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.UCommerce.RunSitecoreCommerceSubmitVisitorOrder
{
    public class RunSitecoreCommerceSubmitVisitorOrder : IPipelineTask<PurchaseOrder>
    {
        public PipelineExecutionResult Execute(PurchaseOrder subject)
        {
            var cartServiceProvider = new CartServiceProvider();
            var contactFactory = new ContactFactory();
            string userId = contactFactory.GetContact();
            var createCartRequest = new CreateOrResumeCartRequest(Context.GetSiteName(), userId);

            var cart = cartServiceProvider.CreateOrResumeCart(createCartRequest).Cart;
            
            var orderService = new OrderServiceProvider();

            var request = new SubmitVisitorOrderRequest(cart);

			// Pass the PurchaseOrder object to Commerce Connect, so it can set use the data for
			// the page events, etc.
			request.Properties.Add("UCommerceOrder", subject);

			var result = orderService.SubmitVisitorOrder(request);

            return PipelineExecutionResult.Success;
        }
    }
}
