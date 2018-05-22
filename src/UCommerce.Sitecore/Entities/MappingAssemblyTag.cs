using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.Entities
{
	/// <summary>
	/// When this class is registered in Castle, all mappings from this Assembly will be added to NHibernate.
	/// </summary>
	internal class MappingAssemblyTag : IContainsNHibernateMappingsTag { }
}
