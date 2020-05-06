using Ucommerce.EntitiesV2;
using Ucommerce.Pipelines;

namespace Ucommerce.Sitecore.Pipelines
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
