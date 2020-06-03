using Sitecore.Data;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates
{
    public abstract class SitecoreItemWithValidName
    {
        public ItemDefinition ItemDefinition { get; private set; }

        public string DisplayName { get; private set; }

        protected void SetItemDefinition(ID itemId, string itemName, ID templateId, ID branchId)
        {
            ItemDefinition = ItemDefinitionFactory.BuildItemDefinition(itemId, itemName, templateId, branchId);
            DisplayName = itemName;
        }
    }
}
