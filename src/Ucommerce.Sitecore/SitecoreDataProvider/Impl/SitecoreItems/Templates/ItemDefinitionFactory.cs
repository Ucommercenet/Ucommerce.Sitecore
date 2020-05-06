using Sitecore.Data;
using SitecoreConfiguration = Sitecore.Configuration;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates
{
    public class ItemDefinitionFactory
    {
        public static ItemDefinition BuildItemDefinition(ID itemId, string itemName, ID templateId, ID branchId)
        {
            // We are making sure, that the name of the item is a valid item name.
            itemName = ProposeValidItemName(itemName);
            return new ItemDefinition(itemId, itemName, templateId, branchId);
        }

        private static string ProposeValidItemName(string itemName)
        {
            if (string.IsNullOrEmpty(itemName)) return "Unknown item name";

            var invalidItemNameChars = SitecoreConfiguration.Settings.InvalidItemNameChars;

            foreach (var invalidChar in invalidItemNameChars)
            {
                itemName = itemName.Replace(invalidChar, ' ');
            }

            return itemName;
        }

    }
}
