using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.Services.Carts;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Sitecore.CommerceConnect.Pipelines.Orders;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Carts.SaveCart
{
	public class SaveCart : ServicePipelineProcessor<CartRequestWithCart, ServiceProviderResult>
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IBasketService _basketService;

		public SaveCart(IEntityFactory entityFactory, IBasketService basketService)
		{
			_entityFactory = entityFactory;
			_basketService = basketService;
		}

		public override void Process(ServicePipelineArgs args)
		{
			CartRequestWithCart request;
			ServiceProviderResult result;

			CheckParametersAndSetupRequestAndResult(args, out request, out result);

			using (new DisposableThreadLifestyleScope())
			{
				var purchaseOrder = _basketService.GetBasketByCartExternalId(request.Cart.ExternalId).PurchaseOrder;
				if (purchaseOrder == null)
				{
					SystemMessage systemMessage = (SystemMessage)_entityFactory.Create("SystemMessage");
					systemMessage.Message = string.Format("Purchase order for cartId = {0} could not be found.",
						request.Cart.ExternalId);
					args.Result.SystemMessages.Add(systemMessage);
					args.Result.Success = false;
					return;
				}

				purchaseOrder["_cartStatus"] = request.Cart.Status;
				purchaseOrder.Save();
			}
		}
	}
}
