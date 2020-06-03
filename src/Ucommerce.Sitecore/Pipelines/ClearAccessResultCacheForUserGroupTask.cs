using Sitecore.Caching;
using Ucommerce.Pipelines;
using Ucommerce.Pipelines.SaveUserGroupRoles;

namespace Ucommerce.Sitecore.Pipelines
{
	public class ClearAccessResultCacheForUserGroupTask : IPipelineTask<IPipelineArgs<SaveUserGroupRolesRequest, SaveUserGroupRolesResponse>>
	{
		public PipelineExecutionResult Execute(IPipelineArgs<SaveUserGroupRolesRequest, SaveUserGroupRolesResponse> subject)
		{
			CacheManager.ClearAccessResultCache();

            return PipelineExecutionResult.Success;
		}
	}
}
