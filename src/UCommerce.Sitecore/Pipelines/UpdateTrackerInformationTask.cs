using System;
using System.Linq;
using System.Reflection;
using Sitecore.Analytics;
using UCommerce.Pipelines;
using UCommerce.Pipelines.AddAddress;
using UCommerce.Sitecore.Sitecore75Compatability;
using UCommerce.Sitecore.UI.Resources;

namespace UCommerce.Sitecore.Pipelines
{
    public class UpdateTrackerInformationTask : IPipelineTask<IPipelineArgs<AddAddressRequest, AddAddressResult>>
    {
        public UpdateTrackerInformationTask()
        {
        }
        public PipelineExecutionResult Execute(IPipelineArgs<AddAddressRequest, AddAddressResult> subject)
        {

            var trackerType = typeof(Tracker);
            if (trackerType == null) return PipelineExecutionResult.Success;


            //Enabled and Current are properties only present in SC82 and below.
            //This should never be called in Sitecore 9.
            var hasEnabledProperty = trackerType.GetProperties(BindingFlags.Static).Any(x => x.Name == "Enabled");
            var hasCurrentProperty = trackerType.GetProperties(BindingFlags.Static).Any(x => x.Name == "Current");

            if (hasCurrentProperty && hasEnabledProperty)
            {   
                new UpdateTrackerInformation().Execute(subject.Request.FirstName, subject.Request.LastName, 
                    subject.Request.EmailAddress, subject.Request.PhoneNumber, subject.Request.AddressName);
            }

            return PipelineExecutionResult.Success;
        }
    }
}
