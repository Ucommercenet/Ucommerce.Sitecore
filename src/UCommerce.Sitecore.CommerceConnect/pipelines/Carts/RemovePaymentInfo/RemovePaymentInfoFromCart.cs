using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Carts;
using Sitecore.Diagnostics;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Pipelines;
using UCommerce.Sitecore.CommerceConnect.Mapping;
using UCommerce.Sitecore.CommerceConnect.Pipelines.Orders;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Carts.RemovePaymentInfo
{
	public class RemovePaymentInfoFromCart : ServicePipelineProcessor<RemovePaymentInfoRequest, RemovePaymentInfoResult>
	{
		private readonly IBasketService _basketService;
		private readonly IMapping<ICollection<Payment>, IList<PaymentInfo>> _paymentMapper;
		private readonly IMapping<PurchaseOrder, Cart> _purchaseOrderMapper;

		public RemovePaymentInfoFromCart(IBasketService basketService)
		{
			_basketService = basketService;
			_paymentMapper = ObjectFactory.Instance.Resolve<IMapping<ICollection<Payment>, IList<PaymentInfo>>>();
			_purchaseOrderMapper = ObjectFactory.Instance.Resolve<IMapping<PurchaseOrder, Cart>>();
		}

		public override void Process(ServicePipelineArgs args)
		{
			RemovePaymentInfoRequest request;
			RemovePaymentInfoResult result;

			using (new DisposableThreadLifestyleScope())
			{
				CheckParametersAndSetupRequestAndResult(args, out request, out result);

				Assert.ArgumentNotNull(request.Payments, "request.Payments");
				Assert.ArgumentNotNull(request.Cart, "request.Cart");

				var purchaseOrder = _basketService.GetBasketByCartExternalId(request.Cart.ExternalId).PurchaseOrder;

				foreach (var paymentInfo in request.Payments)
				{
					RemovePayment(purchaseOrder, paymentInfo);
				}

				PipelineFactory.Create<PurchaseOrder>("Basket").Execute(purchaseOrder);

				var paymentInfos = _paymentMapper.Map(purchaseOrder.Payments);
				result.Payments = new ReadOnlyCollection<PaymentInfo>(paymentInfos);
				result.Cart = _purchaseOrderMapper.Map(purchaseOrder);
			}
		}

		protected virtual void RemovePayment(PurchaseOrder purchaseOrder, PaymentInfo paymentInfo)
		{
			var payment = purchaseOrder.Payments.FirstOrDefault(x => x.PaymentId.ToString() == paymentInfo.ExternalId);

			if (payment != null)
			{
				purchaseOrder.RemovePayment(payment);
			}
		}
	}
}
