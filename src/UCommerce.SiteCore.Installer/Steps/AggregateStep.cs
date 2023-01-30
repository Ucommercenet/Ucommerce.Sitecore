using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public abstract class AggregateStep : IStep
    {
        protected readonly List<IStep> Steps;

        protected AggregateStep()
        {
            Steps = new List<IStep>();
        }

        public async Task Run()
        {
            LogStart();
            foreach (var step in Steps)
            {
                await step.Run();
            }

            LogEnd();
        }

        protected virtual void LogEnd() { }
        protected virtual void LogStart() { }
    }
}
