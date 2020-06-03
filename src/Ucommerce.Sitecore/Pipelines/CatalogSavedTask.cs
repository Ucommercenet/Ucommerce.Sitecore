using System.Threading.Tasks;
using Ucommerce.EntitiesV2;
using Ucommerce.Pipelines;

namespace Ucommerce.Sitecore.Pipelines
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
