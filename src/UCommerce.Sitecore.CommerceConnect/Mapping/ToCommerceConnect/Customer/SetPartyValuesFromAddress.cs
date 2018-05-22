using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Entities;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect.Customer
{
    public class SetPartyValuesFromAddress : IMapValues<IAddress,Party>
    {
        public void MapValues(IAddress source, Party target)
        {
            target.Address1 = source.Line1;
            target.Address2 = source.Line2;
            target.City = source.City;
            target.Company = source.CompanyName;
            target.Country = source.Country.Name;
            target.Email = source.EmailAddress;
            target.FirstName = source.FirstName;
            target.LastName = source.LastName;
            target.PhoneNumber = source.PhoneNumber;
            target.State = source.State;
            target.ZipPostalCode = source.PostalCode;
        }
    }
}
