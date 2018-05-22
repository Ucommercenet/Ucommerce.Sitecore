using System.Collections.ObjectModel;
using Sitecore;
using Sitecore.Commerce.Contacts;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Services.Carts;
using UCommerce.Pipelines;
using UCommerce.Pipelines.AddToBasket;
using UCommerce.Sitecore.CommerceConnect.Mapping;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.UCommerce.RunSitecoreCommerceAddCartLines
{
	public class RunSitecoreCommerceAddCartLines : IPipelineTask<IPipelineArgs<AddToBasketRequest, AddToBasketResponse>>
	{
		public PipelineExecutionResult Execute(IPipelineArgs<AddToBasketRequest, AddToBasketResponse> subject)
		{
			if (subject.Request.Properties.ContainsKey("FromUCommerce"))
				if (!(bool)subject.Request.Properties["FromUCommerce"]) return PipelineExecutionResult.Success;

			//var cart = MappingLibrary.MapPurchaseOrderToCart(subject.Request.PurchaseOrder);

			var contactFactory = new ContactFactory();
			string userId = contactFactory.GetContact();

			var cartServiceProvider = new CartServiceProvider();
			var createCartRequest = new CreateOrResumeCartRequest(Context.GetSiteName(), userId);

			//should it do anything???
			var cart = cartServiceProvider.CreateOrResumeCart(createCartRequest).Cart;

			var cartLine = MappingLibrary.MapOrderLineToCartLine(subject.Response.OrderLine);

			var request = new AddCartLinesRequest(cart, new Collection<CartLine> { cartLine });

			request.Properties["FromUCommerce"] = true;

			var serviceProvider = new CartServiceProvider();

			serviceProvider.AddCartLines(request);

			return PipelineExecutionResult.Success;
		}
	}
}
