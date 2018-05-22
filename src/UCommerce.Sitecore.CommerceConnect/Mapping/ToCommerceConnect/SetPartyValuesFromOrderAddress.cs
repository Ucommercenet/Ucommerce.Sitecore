using Sitecore.Commerce.Entities;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetPartyValuesFromOrderAddress : IMapValues<OrderAddress, Party>
	{
		public void MapValues(OrderAddress source, Party target)
		{
			target.ExternalId = source.AddressId.ToString();
			target.PartyId = source.AddressName;
			target.FirstName = source.FirstName;
			target.LastName = source.LastName;
		    target.Company = source.CompanyName;
			target.Email = source.EmailAddress;
			target.Address1 = source.Line1;
			target.Address2 = source.Line2;
			target.ZipPostalCode = source.PostalCode;
			target.City = source.City;
			target.State = source.State;
			target.Country = source.Country.Name;
			target.PhoneNumber = source.MobilePhoneNumber ?? source.PhoneNumber;
		}
	}
}
