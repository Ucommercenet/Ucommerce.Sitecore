using Ucommerce.EntitiesV2;
using Ucommerce.Pipelines;

namespace Ucommerce.Sitecore.Pipelines
{
	public class ProductDefinitionSavedTask : IPipelineTask<ProductDefinition>
	{
		private readonly ISitecoreContext _context;

		public ProductDefinitionSavedTask(ISitecoreContext context)
		{
			_context = context;
		}

		public PipelineExecutionResult Execute(ProductDefinition subject)
		{
			var provider = _context.DataProviderMaster;

			if (provider != null)
			{
				provider.ProductDefinitionSaved(subject);
			}

			return PipelineExecutionResult.Success;
		}
	}
}
