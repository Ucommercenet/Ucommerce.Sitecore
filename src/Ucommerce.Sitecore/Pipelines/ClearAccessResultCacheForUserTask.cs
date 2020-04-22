using Sitecore.Caching;
using Ucommerce.Pipelines;
using Ucommerce.Pipelines.SaveUserRoles;

namespace Ucommerce.Sitecore.Pipelines
{
	public class ClearAccessResultCacheForUserTask : IPipelineTask<IPipelineArgs<SaveUserRolesRequest, SaveUserRolesResponse>>
	{
		public PipelineExecutionResult Execute(IPipelineArgs<SaveUserRolesRequest, SaveUserRolesResponse> subject)
		{
			CacheManager.ClearAccessResultCache();
			return PipelineExecutionResult.Success;
		}
	}
}
