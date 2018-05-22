using Sitecore;
using Sitecore.Commerce.Contacts;
using Sitecore.Commerce.Services.Carts;
using UCommerce.Pipelines;
using UCommerce.Pipelines.CreateBasket;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.UCommerce.RunSitecoreCommerceCreateOrResumeCartFromCreateBasket
{
	public class RunSitecoreCommerceCreateOrResumeCart : IPipelineTask<IPipelineArgs<CreateBasketRequest, CreateBasketResponse>>
	{
		public PipelineExecutionResult Execute(IPipelineArgs<CreateBasketRequest, CreateBasketResponse> subject)
		{
            if (subject.Request.Properties.ContainsKey("FromUCommerce"))
                if (!(bool)subject.Request.Properties["FromUCommerce"]) return PipelineExecutionResult.Success;

            if (subject.Response.PurchaseOrder == null) { return PipelineExecutionResult.Success; }

			var contactFactory = new ContactFactory();
			string userId = contactFactory.GetContact();
			subject.Response.PurchaseOrder["CommerceConnectUserIdentifier"] = userId;

			var cartServiceProvider = new CartServiceProvider();
			var createCartRequest = new CreateOrResumeCartRequest(Context.GetSiteName(), userId);

			//should it do anything???
			cartServiceProvider.CreateOrResumeCart(createCartRequest);

			return PipelineExecutionResult.Success;
		}
	}
}
