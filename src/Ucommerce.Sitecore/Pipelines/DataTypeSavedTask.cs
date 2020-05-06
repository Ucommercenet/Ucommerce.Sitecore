using Ucommerce.EntitiesV2;
using Ucommerce.Pipelines;

namespace Ucommerce.Sitecore.Pipelines
{
	public class DataTypeSavedTask : IPipelineTask<DataType>
	{
		private readonly ISitecoreContext _context;

		public DataTypeSavedTask(ISitecoreContext context)
		{
			_context = context;
		}

		public PipelineExecutionResult Execute(DataType subject)
		{
			var provider = _context.DataProviderMaster;

			if (provider != null)
			{
				provider.DataTypeSaved(subject);
			}

			return PipelineExecutionResult.Success;
		}
	}
}
