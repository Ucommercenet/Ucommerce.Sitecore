using System.Threading.Tasks;
using Ucommerce.EntitiesV2;
using Ucommerce.Pipelines;

namespace Ucommerce.Sitecore.Pipelines
{
	public class CategorySavedTask : IPipelineTask<Category>
	{
		private readonly ISitecoreContext _context;

		public CategorySavedTask(ISitecoreContext context)
		{
			_context = context;
		}

		public PipelineExecutionResult Execute(Category subject)
		{
			var provider = _context.DataProviderMaster;

			if (provider != null)
			{
				var task = new Task(() => provider.CategorySaved(subject));
				task.Start();
			}

			return PipelineExecutionResult.Success;
		}
	}
}
