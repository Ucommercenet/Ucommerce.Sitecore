using UCommerce.Pipelines;
using UCommerce.Pipelines.SaveUserGroupRoles;
using UCommerce.Pipelines.SaveUserRoles;

namespace UCommerce.Sitecore.Pipelines
{
    public class InformDataProviderThatPermissionsHaveChangedTask : IPipelineTask<IPipelineArgs<SaveUserRolesRequest, SaveUserRolesResponse>>, IPipelineTask<IPipelineArgs<SaveUserGroupRolesRequest, SaveUserGroupRolesResponse>>
    {
        private readonly ISitecoreContext _sitecoreContext;

        public InformDataProviderThatPermissionsHaveChangedTask(ISitecoreContext sitecoreContext)
        {
            _sitecoreContext = sitecoreContext;
        }

        public PipelineExecutionResult Execute(IPipelineArgs<SaveUserRolesRequest, SaveUserRolesResponse> subject)
        {
            InformDataProvider();

            return PipelineExecutionResult.Success;
        }

        public PipelineExecutionResult Execute(IPipelineArgs<SaveUserGroupRolesRequest, SaveUserGroupRolesResponse> subject)
        {
            InformDataProvider();

            return PipelineExecutionResult.Success;
        }

        private void InformDataProvider()
        {
            var dataProvider = _sitecoreContext.DataProviderMaster;

            if (dataProvider != null)
            {
                dataProvider.PermissionsChanged();
            }
        }
    }
}
