using Sitecore;
using Sitecore.Commerce.Contacts;
using Sitecore.Commerce.Services.Carts;
using UCommerce.Pipelines;
using UCommerce.Pipelines.GetBasket;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.UCommerce.RunSitecoreCommerceCreateOrResumeCartFromGetBasket
{
	public class RunSitecoreCommerceCreateOrResumeCart : IPipelineTask<IPipelineArgs<GetBasketRequest, GetBasketResponse>>
	{
		public PipelineExecutionResult Execute(IPipelineArgs<GetBasketRequest, GetBasketResponse> subject)
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
