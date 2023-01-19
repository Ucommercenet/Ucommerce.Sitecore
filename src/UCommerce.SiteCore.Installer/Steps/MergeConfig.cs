using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
	public class MergeConfig : MergeConfigKeepingConnectionStringValue, IStep
	{
		private readonly FileInfo _toBeTransformed;
	    public IList<Transformation> Transformations { get; set; }

        public MergeConfig(FileInfo configurationVirtualPath, IList<Transformation> transformations)
		{
			InitializeTargetDocumentPath(configurationVirtualPath.FullName);
		    Transformations = transformations;
			_toBeTransformed = configurationVirtualPath;
		}


		public async Task Run()
		{
			ReadConnectionStringAttribute();

			using (var transformer = new ConfigurationTransformer(_toBeTransformed))
			{
				foreach (var transformation in Transformations)
				{
					transformer.Transform(
						new FileInfo(HostingEnvironment.MapPath(transformation.path)),
						transformation.onlyIfIsIntegrated,
						ex => new SitecoreInstallerLoggingService().Error<int>(ex));
				}
			}

			SetConnectionStringAttribute();
		}
	}
}
