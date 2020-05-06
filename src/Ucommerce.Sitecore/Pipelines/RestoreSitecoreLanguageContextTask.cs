using System;
using Sitecore;
using Sitecore.Globalization;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure.Logging;
using Ucommerce.Pipelines;

namespace Ucommerce.Sitecore.Pipelines
{
    public class RestoreSitecoreLanguageContextTask : IPipelineTask<PurchaseOrder>
    {
        private readonly ILoggingService _loggingService;

        public RestoreSitecoreLanguageContextTask(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }


        public PipelineExecutionResult Execute(PurchaseOrder purchaseOrder)
        {
            if (string.IsNullOrEmpty(purchaseOrder.CultureCode))
            {
                return PipelineExecutionResult.Success;
            }

            try
            {
                Context.SetLanguage(Language.Parse(purchaseOrder.CultureCode), false);
            }
            catch (Exception ex)
            {
                _loggingService.Log<RestoreSitecoreLanguageContextTask>(ex, "Error trying to restore the Sitecore language context");
            }

            return PipelineExecutionResult.Success;
        }
    }
}