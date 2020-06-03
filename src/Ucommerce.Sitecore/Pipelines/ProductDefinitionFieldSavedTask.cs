using Ucommerce.EntitiesV2;
using Ucommerce.Pipelines;

namespace Ucommerce.Sitecore.Pipelines
{
	public class ProductDefinitionFieldSavedTask : IPipelineTask<ProductDefinitionField>
	{
		private readonly ISitecoreContext _context;

		public ProductDefinitionFieldSavedTask(ISitecoreContext context)
		{
			_context = context;
		}

		public PipelineExecutionResult Execute(ProductDefinitionField subject)
		{
			var provider = _context.DataProviderMaster;

			if (provider != null)
			{
				provider.ProductDefinitionSaved(subject.ProductDefinition);
			}

			return PipelineExecutionResult.Success;
		}
	}
}
