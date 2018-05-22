using Sitecore;
using Sitecore.Caching;
using Sitecore.Data;
using UCommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Content;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates
{
	internal class BucketFolderItem : FolderItem
	{
		public BucketFolderItem(ID id, string name) : base(id, name)
		{
            SetItemDefinition(Id, FolderName, new ID("{ADB6CA4F-03EF-4F47-B9AC-9CE2BA53FF97}"), ID.Null);
            ((ICacheable)ItemDefinition).Cacheable = SitecoreConstants.EnableCacheable;
        }

        public bool HoldsProducts { get; set; }

		public override void AddItem(ISitecoreItem item)
		{
			if (item is ContentNodeSitecoreItem)
			{
				HoldsProducts = true;
			}
			base.AddItem(item);
		}
	}
}
