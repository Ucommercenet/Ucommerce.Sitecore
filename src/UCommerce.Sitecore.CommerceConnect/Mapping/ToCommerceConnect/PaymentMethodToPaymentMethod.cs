using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Globalization;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
    public class PaymentMethodToPaymentMethod : IMapping<PaymentMethod, global::Sitecore.Commerce.Entities.Payments.PaymentMethod>
    {
        private readonly ILocalizationContext _localizationContext;

        public PaymentMethodToPaymentMethod()
        {
            _localizationContext = ObjectFactory.Instance.Resolve<ILocalizationContext>();
        }

        public global::Sitecore.Commerce.Entities.Payments.PaymentMethod Map(PaymentMethod paymentMethod)
        {
            var ccPaymentMethod = new global::Sitecore.Commerce.Entities.Payments.PaymentMethod()
            {
                PaymentOptionId = paymentMethod.PaymentMethodId.ToString(),
                ExternalId = paymentMethod.PaymentMethodId.ToString(),
                Name = paymentMethod.Name
            };

            if (paymentMethod.GetDescription(_localizationContext.CurrentCultureCode) != null)
            {
                ccPaymentMethod.Description = paymentMethod.GetDescription(_localizationContext.CurrentCultureCode).Description;
            }

            return ccPaymentMethod;
        }
    }
}
