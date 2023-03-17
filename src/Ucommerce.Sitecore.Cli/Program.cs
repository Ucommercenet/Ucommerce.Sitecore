using System.Threading.Tasks;
using CliFx;

namespace Ucommerce.Sitecore.Cli
{
    internal class Program
    {
        public static async Task<int> Main()
        {
            return await new CliApplicationBuilder()
                .AddCommandsFromThisAssembly()
                .Build()
                .RunAsync();
        }
    }
}