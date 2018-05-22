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
    public class AddressToParty : IMapping<IAddress,Party>
    {
        private readonly IMapValues<IAddress, Party> _setPartyValuesFromAddress;

        public AddressToParty(IMapValues<IAddress,Party> setPartyValuesFromAddress)
        {
            _setPartyValuesFromAddress = setPartyValuesFromAddress;
        }

        public Party Map(IAddress target)
        {
            Party party = new Party();

            _setPartyValuesFromAddress.MapValues(target,party);

            return party;
        }
    }
}
