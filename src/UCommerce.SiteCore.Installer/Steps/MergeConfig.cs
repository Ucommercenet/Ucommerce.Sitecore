using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class MergeConfig : IStep
    {
        private readonly IInstallerLoggingService _loggingService;
        private readonly FileInfo _toBeTransformed;
        public IList<Transformation> Transformations { get; set; }

        public MergeConfig(FileInfo configuration, IList<Transformation> transformations, IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
            Transformations = transformations;
            _toBeTransformed = configuration;
        }

        public Task Run()
        {
            _loggingService.Information<MergeConfig>($"Merging Ucommerce configs into {_toBeTransformed.FullName}");

            var connectionStringAttribute = GetUcommerceConnectionString(_toBeTransformed);

            using (var transformer = new ConfigurationTransformer(_toBeTransformed))
            {
                foreach (var transformation in Transformations)
                {
                    transformer.Transform(
                        transformation.Path,
                        transformation.OnlyIfIsIntegrated,
                        ex =>
                        {
                            _loggingService.Error<int>(ex);
                            throw ex;
                        });
                }
            }

            SetConnectionStringAttribute(connectionStringAttribute);
            return Task.CompletedTask;
        }

        private XAttribute GetUcommerceConnectionString(FileInfo appConfig)
        {
            var xFile = XDocument.Load(appConfig.FullName);
            var element = xFile.Descendants()
                .FirstOrDefault(x => x.Name == "runtimeConfiguration" && x.Parent != null && x.Parent.Name == "commerce");
            return element?.Attribute("connectionString");
        }

        private void SetConnectionStringAttribute(XAttribute connectionStringAttribute)
        {
            if (connectionStringAttribute == null) return;

            var targetDocument = XDocument.Load(_toBeTransformed.FullName);

            var newAttribute = GetUcommerceConnectionString(_toBeTransformed);
            if (newAttribute == null)
            {
                return;
            }

            newAttribute.Value = connectionStringAttribute.Value;
            targetDocument.Save(_toBeTransformed.FullName);
        }
    }
}
