using Ucommerce.EntitiesV2.Definitions;
using Ucommerce.Pipelines;

namespace Ucommerce.Sitecore.Pipelines
{
	public class DefinitionFieldSavedTask : IPipelineTask<IDefinitionField>
	{
		private readonly ISitecoreContext _context;

		public DefinitionFieldSavedTask(ISitecoreContext context)
		{
			_context = context;
		}

		public PipelineExecutionResult Execute(IDefinitionField subject)
		{
			var provider = _context.DataProviderMaster;

			if (provider != null)
			{
				provider.DefinitionFieldSaved(subject);
			}

			return PipelineExecutionResult.Success;
		}
	}
}
