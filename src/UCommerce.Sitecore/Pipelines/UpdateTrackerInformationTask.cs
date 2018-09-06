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
        private readonly ISitecoreVersionResolver _sitecoreVersionResolver;

        public UpdateTrackerInformationTask(ISitecoreVersionResolver sitecoreVersionResolver)
        {
            _sitecoreVersionResolver = sitecoreVersionResolver;
        }
        public PipelineExecutionResult Execute(IPipelineArgs<AddAddressRequest, AddAddressResult> subject)
        {

            var trackerType = typeof(Tracker);
            if (trackerType == null) return PipelineExecutionResult.Success;

            var hasEnabledProperty = trackerType.GetProperties(BindingFlags.Static).Any(x => x.Name == "Enabled");
            var hasCurrentProperty = trackerType.GetProperties(BindingFlags.Static).Any(x => x.Name == "Current");

            if (hasCurrentProperty && hasEnabledProperty)
            {   
                new UpdateTrackerInformation(_sitecoreVersionResolver).Execute(subject.Request.FirstName, subject.Request.LastName, 
                    subject.Request.EmailAddress, subject.Request.PhoneNumber, subject.Request.AddressName);
            }

            return PipelineExecutionResult.Success;

        }
    }
}
