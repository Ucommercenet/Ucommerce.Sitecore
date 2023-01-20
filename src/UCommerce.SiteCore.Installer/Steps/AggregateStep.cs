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
            foreach (var step in Steps)
            {
                await step.Run();
            }
        }
    }
}
