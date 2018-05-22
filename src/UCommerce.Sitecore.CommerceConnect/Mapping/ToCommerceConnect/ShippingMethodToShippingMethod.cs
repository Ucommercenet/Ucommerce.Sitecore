using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Globalization;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class ShippingMethodToShippingMethod : IMapping<ShippingMethod, global::Sitecore.Commerce.Entities.Shipping.ShippingMethod>
	{
		private readonly ILocalizationContext _localizationContext;

		public ShippingMethodToShippingMethod()
		{
			_localizationContext = ObjectFactory.Instance.Resolve<ILocalizationContext>();
		}

		public global::Sitecore.Commerce.Entities.Shipping.ShippingMethod Map(ShippingMethod shippingMethod)
		{
			var ccShippingMethod = new global::Sitecore.Commerce.Entities.Shipping.ShippingMethod()
			{
				ExternalId = shippingMethod.ShippingMethodId.ToString(),
				Name = shippingMethod.Name
			};

			if (shippingMethod.HasDescription(_localizationContext.CurrentCultureCode))
			{
				ccShippingMethod.Description = shippingMethod.GetDescription(_localizationContext.CurrentCultureCode).Description;
			}
			
			return ccShippingMethod;
		}
	}
}
