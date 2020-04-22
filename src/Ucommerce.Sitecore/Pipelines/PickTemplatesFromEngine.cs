using Sitecore.Pipelines;
using Ucommerce.Infrastructure;
using Ucommerce.Infrastructure.Configuration;

namespace Ucommerce.Sitecore.Pipelines
{
	public class PickTemplatesFromEngine
	{
		public void Process(PipelineArgs args)
		{
            // Wrapping this code in a Disposable thread lifestyle scope.
            //
            // This is done because the code is being run as part of a module initialization.
            // And at this early stage of the ASP.NET lifestyle, the PerRequestLifestyle module is not yet initialized.
            // Therefor the IOC scope we get when using ObjectFactory is a thread scope. And that needs to be cleaned up.
		    using (new DisposableThreadLifestyleScope())
		    {
                var sitecoreContext = ObjectFactory.Instance.Resolve<ISitecoreContext>();
                var dataProviderMasterDatabase = sitecoreContext.DataProviderMaster;

                if (dataProviderMasterDatabase == null)
                    return;

                dataProviderMasterDatabase.ResetTemplatesCollection();
            }
        }
	}
}