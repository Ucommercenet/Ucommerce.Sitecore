using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;

namespace UCommerce.Sitecore.Pipelines
{
	public class StoreSavedTask : IPipelineTask<ProductCatalogGroup>
	{
		private readonly ISitecoreContext _context;

		public StoreSavedTask(ISitecoreContext context)
		{
			_context = context;
		}

		public PipelineExecutionResult Execute(ProductCatalogGroup subject)
		{
			var provider = _context.DataProviderMaster;

			if (provider != null)
			{
				provider.StoreSaved(subject);
			}

			return PipelineExecutionResult.Success;
		}
	}
}
