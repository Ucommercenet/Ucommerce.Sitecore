using System;
using System.Linq;
using Ucommerce.SystemInformation;

namespace Ucommerce.Sitecore.SystemInformation
{
    public class GetHostSystemInfo: IGetHostSystemInfo
    {
        public HostSystemInfo Get()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var sitecoreAssembly = assemblies.FirstOrDefault(x => x.FullName.ToLower().Contains("sitecore.kernel"));
            if (sitecoreAssembly == null)
            {
                return null;
            }

            return new HostSystemInfo
            {
                Name = "Sitecore",
                Version = sitecoreAssembly.GetName().Version
            };
        }
    }
}
