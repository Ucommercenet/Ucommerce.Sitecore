using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Customers;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect.Customer
{
    public class SetCustomerPartyValuesFromAddress : IMapValues<IAddress, CustomerParty>
    {
        public void MapValues(IAddress source, CustomerParty target)
        {
            target.Name = source.AddressName;
            target.PartyType = CustomerPartyTypes.BuyerParty;
            target.ExternalId = source.AddressId.ToString();
        }
    }
}
