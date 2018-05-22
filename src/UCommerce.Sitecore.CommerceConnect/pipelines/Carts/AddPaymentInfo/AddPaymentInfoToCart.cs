using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Carts;
using Sitecore.Diagnostics;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Sitecore.CommerceConnect.Mapping;
using UCommerce.Sitecore.CommerceConnect.Pipelines.Orders;
using UCommerce.Sitecore.CommerceConnect.Entities.Carts;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Carts.AddPaymentInfo
{
    public class AddPaymentInfoToCart : ServicePipelineProcessor<AddPaymentInfoRequest, AddPaymentInfoResult>
    {
        private readonly IBasketService _basketService;
        private readonly IMapping<ICollection<Payment>, IList<PaymentInfo>> _paymentMapper;
        private readonly IMapping<PurchaseOrder, Cart> _purchaseOrderMapper;

        public AddPaymentInfoToCart(IBasketService basketService)
        {
            _basketService = basketService;
            _paymentMapper = ObjectFactory.Instance.Resolve<IMapping<ICollection<Payment>, IList<PaymentInfo>>>();
            _purchaseOrderMapper = ObjectFactory.Instance.Resolve<IMapping<PurchaseOrder, Cart>>();
        }

        public override void Process(ServicePipelineArgs args)
        {
	        AddPaymentInfoRequest request;
	        AddPaymentInfoResult result;

            using (new DisposableThreadLifestyleScope())
            {
                CheckParametersAndSetupRequestAndResult(args, out request, out result);

                Assert.ArgumentNotNull(request.Payments, "request.Payments");
                Assert.ArgumentNotNull(request.Cart, "request.Cart");

                var purchaseOrder = _basketService.GetBasketByCartExternalId(request.Cart.ExternalId).PurchaseOrder;

                foreach (var paymentInfo in request.Payments)
                {
                    var paymentMethod = GetPaymentMethod(paymentInfo);
	                var paymentInfoWithAmount = paymentInfo as PaymentInfoWithAmount;
	                if (paymentInfoWithAmount != null)
	                {
						// If a specific amount was given, create a Payment with that amount.
		                CreatePayment(purchaseOrder, paymentMethod, paymentInfoWithAmount.Amount);
	                }
	                else
	                {
						// If no amount was given, create a payment with the default amount.
						CreatePayment(purchaseOrder, paymentMethod, -1);
					}
				}

                global::UCommerce.Pipelines.PipelineFactory.Create<PurchaseOrder>("Basket").Execute(purchaseOrder);

                var paymentInfos = _paymentMapper.Map(purchaseOrder.Payments);
                result.Payments = new ReadOnlyCollection<PaymentInfo>(paymentInfos);
                result.Cart = _purchaseOrderMapper.Map(purchaseOrder);
            }
        }

		protected virtual Payment CreatePayment(PurchaseOrder purchaseOrder, PaymentMethod paymentMethod, decimal amount)
		{
			var payment = TransactionLibrary.CreatePayment(paymentMethod.PaymentMethodId, amount: amount, requestPayment: false, overwriteExisting: false);
			return payment;
		}

		protected virtual PaymentMethod GetPaymentMethod(PaymentInfo paymentInfo)
        {
            int paymentMethodId;
            if (!int.TryParse(paymentInfo.PaymentMethodID, out paymentMethodId))
            {
                throw new ArgumentException(string.Format("Could not parse: {0} into an int, paymentInfo.PaymentMethodID needs to be an int", paymentInfo.PaymentMethodID));
            }

            var paymentMethodRepository = ObjectFactory.Instance.Resolve<IRepository<PaymentMethod>>();
            var paymentMethod = paymentMethodRepository.Get(paymentMethodId);
            if (paymentMethod == null)
            {
                throw new ArgumentException(string.Format("No payment method with id: {0} could be found", paymentMethodId));
            }

            return paymentMethod;
        }
    }
}

