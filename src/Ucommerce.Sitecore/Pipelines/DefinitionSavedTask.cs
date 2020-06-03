using Ucommerce.EntitiesV2;
using Ucommerce.EntitiesV2.Definitions;
using Ucommerce.Pipelines;

namespace Ucommerce.Sitecore.Pipelines
{
	public class DefinitionSavedTask : IPipelineTask<IDefinition>
	{
		private readonly ISitecoreContext _context;

		public DefinitionSavedTask(ISitecoreContext context)
		{
			_context = context;
		}

		public PipelineExecutionResult Execute(IDefinition subject)
		{
			var provider = _context.DataProviderMaster;

			if (provider != null)
			{
				var productDefinition = subject as ProductDefinition;
				if (productDefinition != null)
				{
					provider.ProductDefinitionSaved(productDefinition);
				}
				else
				{
					provider.DefinitionSaved(subject);
				}
			}

			return PipelineExecutionResult.Success;
		}
	}
}
