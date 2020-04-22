using Ucommerce.Infrastructure.Globalization;
using Ucommerce.Pipelines;

namespace Ucommerce.Sitecore.Pipelines
{
	public class LanguageSavedTask : IPipelineTask<Language>
	{
		private readonly ISitecoreContext _context;

		public LanguageSavedTask(ISitecoreContext context)
		{
			_context = context;
		}

		public PipelineExecutionResult Execute(Language subject)
		{
			var provider = _context.DataProviderMaster;

			if (provider != null)
			{
				provider.LanguageSaved(subject);
			}

			return PipelineExecutionResult.Success;
		}
	}
}
