using System.Linq;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Reflection;
using UCommerce.Sitecore.SitecoreDataProvider;

namespace UCommerce.Sitecore.Extensions
{
	/// <summary>
	/// Contains extension methods for Sitecore's Sitecore.Data.Database type.
	/// </summary>
	public static class DatabaseExtensions
	{
		public static DataProviderCollection GetDataProviders(this Database @this)
		{
			var fieldValue = ReflectionUtil.GetField(@this, "_dataProviders");
			return fieldValue as DataProviderCollection;
		}

		public static DataProviderMasterDatabase GetUcommerceDataProvider(this Database @this)
		{
			var providers = @this.GetDataProviders();
			if (providers == null) return null;
			return providers.OfType<DataProviderMasterDatabase>().FirstOrDefault();
		}
	}
}
