namespace UCommerce.Sitecore.SitecoreDataProvider.Impl
{
	public class UCommerceId
	{
		public UCommerceId(string nodeType, string itemId)
		{
			NodeType = nodeType;
			ItemId = itemId;

			if (string.IsNullOrEmpty(ItemId))
			{
				ItemId = "0";
			}
		}

		public static UCommerceId FromId(string id)
		{
			var parts = id.Split(new[] {';'});
			return new UCommerceId(parts[0], parts[1]);
		}

		public string NodeType { get; set; }
		public string ItemId { get; set; }

		public string ToId()
		{
			return string.Format("{0};{1}", NodeType, ItemId);
		}
	}
}
