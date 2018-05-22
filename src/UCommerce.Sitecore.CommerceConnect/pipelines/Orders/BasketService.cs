using System;
using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Pipelines;
using UCommerce.Pipelines.GetBasket;
using UCommerce.Runtime;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Orders
{
	public class BasketService : IBasketService
	{
		public Basket GetBasketByCartExternalId(string externalId)
		{
			var purchaseOrderRepository = ObjectFactory.Instance.Resolve<IRepository<PurchaseOrder>>();

			Guid basketId;
			if (!Guid.TryParse(externalId, out basketId))
			{
				throw new FormatException(string.Format("External cart id {0} couldn't be converted to a basket id", externalId));
			}

			return new Basket(purchaseOrderRepository.Select(x => x.BasketId == basketId).FirstOrDefault());
		}

		public Basket CreateBasket(bool internalRequest)
		{
			PipelineExecutionResult pipelineResult;
			var getBasketPipelineArgs = ExecuteGetBasketPipeline(internalRequest, true, out pipelineResult);

			getBasketPipelineArgs.Response.PurchaseOrder["PurchaseOrderIsNew"] =
				getBasketPipelineArgs.Response.PurchaseOrderIsNew.ToString();

			if (getBasketPipelineArgs.Response.PurchaseOrder == null &&
				pipelineResult != PipelineExecutionResult.Success)
				throw new ArgumentException("No basket exists for the current user.");

			if (getBasketPipelineArgs.Response.PurchaseOrder == null) return null;

			return new Basket(getBasketPipelineArgs.Response.PurchaseOrder);
		}

		private static GetBasketPipelineArgs ExecuteGetBasketPipeline(bool internalRequest, bool createBasket,
			out PipelineExecutionResult pipelineResult)
		{
			var getBasketPipeline = PipelineFactory.Create<IPipelineArgs<GetBasketRequest, GetBasketResponse>>("GetBasket");
			var catalogContext = ObjectFactory.Instance.Resolve<ICatalogContext>();

			var getBasketPipelineArgs = new GetBasketPipelineArgs(
				new GetBasketRequest
				{
					Create = createBasket,
					PriceGroup = catalogContext.CurrentPriceGroup,
					ProductCatalogGroup = catalogContext.CurrentCatalogGroup
				},
				new GetBasketResponse());

			getBasketPipelineArgs.Request.Properties["FromUCommerce"] = !internalRequest;

			pipelineResult = getBasketPipeline.Execute(getBasketPipelineArgs);
			return getBasketPipelineArgs;
		}
	}
}
