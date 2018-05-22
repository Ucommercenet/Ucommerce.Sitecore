using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;

namespace UCommerce.Sitecore.Pipelines
{
	public class ProductDeletedTask : IPipelineTask<Product>
	{
		private readonly ISitecoreContext _context;

		public ProductDeletedTask(ISitecoreContext context)
		{
			_context = context;
		}


		public PipelineExecutionResult Execute(Product subject)
		{
			var provider = _context.DataProviderMaster;

			if (provider != null)
			{
				provider.ProductDeleted(subject);
			}

			return PipelineExecutionResult.Success;
		}
	}
}
