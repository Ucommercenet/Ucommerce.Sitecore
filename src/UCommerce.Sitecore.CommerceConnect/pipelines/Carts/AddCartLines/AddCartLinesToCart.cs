using System;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.Services.Carts;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Pipelines;
using UCommerce.Pipelines.AddToBasket;
using UCommerce.Pipelines.GetProduct;
using UCommerce.Runtime;
using UCommerce.Sitecore.CommerceConnect.Mapping;
using UCommerce.Sitecore.CommerceConnect.Pipelines.Orders;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Carts.AddCartLines
{
	public class AddCartLinesToCart : ServicePipelineProcessor<AddCartLinesRequest, CartResult>
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IBasketService _basketService;

		public AddCartLinesToCart(IEntityFactory entityFactory, IBasketService basketService)
		{
			_entityFactory = entityFactory;
			_basketService = basketService;
		}

		public override void Process(ServicePipelineArgs args)
		{
			AddCartLinesRequest request;
			CartResult result;
			CheckParametersAndSetupRequestAndResult(args, out request, out result);

			if (!IsFromUcommerce(request))
			{
				using (new DisposableThreadLifestyleScope())
				{
					var cart = request.Cart;
					var basket = _basketService.GetBasketByCartExternalId(cart.ExternalId);

					if (basket == null || basket.PurchaseOrder == null)
					{
						SystemMessage systemMessage = (SystemMessage)_entityFactory.Create("SystemMessage");
						systemMessage.Message = string.Format("Purchase order for cartId = {0} could not be found.", cart.ExternalId);
						args.Result.SystemMessages.Add(systemMessage);
						args.Result.Success = false;

						return;
					}


					var addToBasketPipeline = ObjectFactory.Instance.Resolve<IPipeline<IPipelineArgs<AddToBasketRequest, AddToBasketResponse>>>("AddToBasket");

					var catalogContext = ObjectFactory.Instance.Resolve<ICatalogContext>();

					foreach (var cartLine in request.Lines)
					{
						var addToBasketPipelineArgs = GetAddToBasketPipelineArgs(catalogContext, basket.PurchaseOrder, cartLine);
						addToBasketPipeline.Execute(addToBasketPipelineArgs);
					}
				}
			}
		}

		private AddToBasketPipelineArgs GetAddToBasketPipelineArgs(ICatalogContext catalogContext, PurchaseOrder purchaseOrder, CartLine cartLine)
		{
			var addToBasketPipelineArgs = new AddToBasketPipelineArgs(
					new AddToBasketRequest
					{
						AddToExistingOrderLine = true,
						ExecuteBasketPipeline = false,
						PriceGroup = catalogContext.CurrentPriceGroup,
						Product = GetProduct(cartLine.Product),
						PurchaseOrder = purchaseOrder,
						Quantity = (int)cartLine.Quantity
					},
					new AddToBasketResponse());

			addToBasketPipelineArgs.Request.Properties["FromUCommerce"] = false;

			return addToBasketPipelineArgs;
		}

		private Product GetProduct(CartProduct uCommerceCartProduct)
		{
			var getProductPipeline = ObjectFactory.Instance.Resolve<IPipeline<IPipelineArgs<GetProductRequest, GetProductResponse>>>("GetProduct");

			//Map cartProduct to uCommerceV2.Product
			Product product = MappingLibrary.MapCartProductToProduct(uCommerceCartProduct);

			var getProductPipelineArgs = new GetProductPipelineArgs(
				new GetProductRequest(new ProductIdentifier(product.Sku, product.VariantSku)),
				new GetProductResponse());

			if (getProductPipeline.Execute(getProductPipelineArgs) == PipelineExecutionResult.Error)
				throw new ArgumentException(string.Format("No product with id {0} was found", getProductPipelineArgs.Request.ProductIdentifier.Sku));

			return getProductPipelineArgs.Response.Product;
		}
	}
}
