using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Entities.Customers;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect.Customer
{
    public class AddressToCustomerParty : IMapping<IAddress,CustomerParty>
    {
        private readonly IMapValues<IAddress, CustomerParty> _setCustomerPartyValuesFromAddress;

        public AddressToCustomerParty(IMapValues<IAddress, CustomerParty> setCustomerPartyValuesFromAddress)
        {
            _setCustomerPartyValuesFromAddress = setCustomerPartyValuesFromAddress;
        }

        public CustomerParty Map(IAddress target)
        {
            CustomerParty customerParty = new CustomerParty();

            _setCustomerPartyValuesFromAddress.MapValues(target,customerParty);

            return customerParty;
        }
    }
}
