using System;
using UCommerce.Installer;
using UCommerce.Installer.Extensions;

namespace UCommerce.Sitecore.Installer
{
	internal class SitecoreConnectionStringLocator : ConnectionStringLocator
	{
		protected override string DoLocate()
		{
			// try get conventional (or existingOne)
			string connectionString;

			bool set = TryConventionalConnectionString(out connectionString) ||
				TrySiteCoreConnectionString(out connectionString);

			if (!set) throw new NotSupportedException("Could not find a suitable connection string");
			
			return connectionString;
		}

		private bool TryConventionalConnectionString(out string connectionString)
		{
			return "uCommerce".TryGetConnectionString(out connectionString);
		}

		private bool TrySiteCoreConnectionString(out string connectionString)
		{
			return "web".TryGetConnectionString(out connectionString);
		}

		
	}
}
