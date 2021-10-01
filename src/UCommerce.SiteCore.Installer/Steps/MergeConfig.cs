using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
	public class MergeConfig : MergeConfigKeepingConnectionStringValue, IPostStep
	{
        private readonly IInstallerLoggingService _logging;
        private readonly FileInfo _toBeTransformed;
	    public IList<Transformation> Transformations { get; set; }

        public MergeConfig(string configurationVirtualPath, IList<Transformation> transformations, IInstallerLoggingService logging)
		{
            _logging = logging;
            InitializeTargetDocumentPath(configurationVirtualPath);
		    Transformations = transformations;
			_toBeTransformed = new FileInfo(HostingEnvironment.MapPath(configurationVirtualPath));
		}


		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			ReadConnectionStringAttribute();

			using (var transformer = new ConfigurationTransformer(_toBeTransformed))
			{
				foreach (var transformation in Transformations)
				{
					transformer.Transform(
						new FileInfo(HostingEnvironment.MapPath(transformation.VirtualPath)),
						transformation.OnlyIfIisIntegrated,
						ex => _logging.Error<MergeConfig>(ex));
				}
			}

			SetConnectionStringAttribute();
		}
	}
}
