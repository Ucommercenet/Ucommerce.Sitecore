using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;

namespace UCommerce.Sitecore.Pipelines
{
	public class CatalogSavedTask : IPipelineTask<ProductCatalog>
	{
		private readonly ISitecoreContext _context;

		public CatalogSavedTask(ISitecoreContext context)
		{
			_context = context;
		}

		public PipelineExecutionResult Execute(ProductCatalog subject)
		{
			var provider = _context.DataProviderMaster;

			if (provider != null)
			{
				var task = new Task(() => provider.CatalogSaved(subject));
				task.Start();
			}

			return PipelineExecutionResult.Success;
		}
	}
}
