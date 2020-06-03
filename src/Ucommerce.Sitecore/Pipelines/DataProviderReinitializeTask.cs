using Ucommerce.Pipelines;

namespace Ucommerce.Sitecore.Pipelines
{

	/// <summary>
	/// Executing this task informs the Data Provider that data has changed, and it should re-initialize itself.
	/// </summary>
	/// <typeparam name="T">The data type of the pipeline.</typeparam>
	public class DataProviderReinitializeTask<T> : IPipelineTask<T>
	{
		private readonly ISitecoreContext _context;

		public DataProviderReinitializeTask(ISitecoreContext context)
		{
			_context = context;
		}

		public PipelineExecutionResult Execute(T subject)
		{
			var provider = _context.DataProviderMaster;

			if (provider != null)
			{
				provider.DataChangedPleaseReinitialize();
			}

			return PipelineExecutionResult.Success;
		}
	}
}
