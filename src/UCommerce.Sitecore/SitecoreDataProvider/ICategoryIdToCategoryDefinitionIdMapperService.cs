using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCommerce.Sitecore.SitecoreDataProvider
{
	public interface ICategoryIdToCategoryDefinitionIdMapperService
	{
		int MapToCategoryDefinitionId(int categoryId);
	}
}
