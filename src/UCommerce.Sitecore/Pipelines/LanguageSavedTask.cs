using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCommerce.Infrastructure.Globalization;
using UCommerce.Pipelines;

namespace UCommerce.Sitecore.Pipelines
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
