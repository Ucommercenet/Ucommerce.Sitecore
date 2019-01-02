using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Sitecore.Data.IDTables;
using Sitecore.Install.Framework;
using UCommerce.Installer;
using SqlCommand = System.Data.SqlClient.SqlCommand;

namespace UCommerce.Sitecore.Installer.Steps
{
    /// <summary>
    /// This class migrates ID values found in IDTable to the uCommerce database.
    /// </summary>
    /// <remarks>
    /// If a product, for example, has been given a Sitecore ID, then the uCommerce Guid value is updated to reflect this.
    /// </remarks>
    public class MigrateIdTableValues : IPostStep
    {
        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            new InstallationSteps.MigrateIdTableValues().Execute();
        }
    }
}
