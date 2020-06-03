using Ucommerce.EntitiesV2;
using Ucommerce.Pipelines;

namespace Ucommerce.Sitecore.Pipelines
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
