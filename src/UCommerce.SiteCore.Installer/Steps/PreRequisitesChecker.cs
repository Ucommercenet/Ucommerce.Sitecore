using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using Sitecore.Install.Framework;
using Sitecore.Install.Utils;
using Sitecore.IO;
using Ucommerce.Installer;
using Ucommerce.Installer.Prerequisites;
using Ucommerce.Installer.Prerequisites.impl;

namespace Ucommerce.Sitecore.Installer.Steps
{
	public class CanCreateTablesSitecore : IPrerequisitStep
	{
		private readonly string _connectionString;
		private readonly IInstallerLoggingService _loggingService;
		
		public CanCreateTablesSitecore(string connectionString, IInstallerLoggingService loggingService)
		{
			_connectionString = connectionString;
			_loggingService = loggingService;
		}

		public bool MeetsRequirement(out string information)
		{
			var sqlRequirements = new StringBuilder();
			bool errors = !CanExecuteQuery("CREATE TABLE Ucommerce_installationTable ( id int )");
			
			if (errors)
			{
				sqlRequirements.AppendLine("Connection string are not granted: create / drop / alter table permissions.");
				sqlRequirements.AppendLine("Permissions needed for connectionstring: " + _connectionString);
				sqlRequirements.AppendLine("Make sure to grant above permissions to proceed.");
				_loggingService.Log<CanCreateTables>("The right permissions for the database isn't granted.: " + sqlRequirements.ToString());
			}

			information = sqlRequirements.ToString();

			return !errors;
		}

		private void TryExecutequery(string queryToExecute)
		{
			int amountToRetry = 3;
			int waitTimeInMs = 100;
			int actualRetries = 0;
			while (actualRetries <= amountToRetry)
			{
				actualRetries++;
				try
				{
					_loggingService.Log<CanCreateTablesSitecore>("Trying to verify SQL roles exists.");
					var con = new SqlConnection(_connectionString);
					var command = new System.Data.SqlClient.SqlCommand(queryToExecute, con);

					con.Open();

					command.Transaction = con.BeginTransaction();
					command.ExecuteNonQuery();
					command.Transaction.Rollback();

					con.Close();
					
					_loggingService.Log<CanCreateTablesSitecore>("Done verifying SQL roles.");
					return;
				}
				catch (Exception ex)
				{
					_loggingService.Log<CanCreateTablesSitecore>($"Iteration { actualRetries} / { amountToRetry } - Failed to execute query with message.: { ex.Message} ");
					Thread.Sleep(waitTimeInMs);
				}
			}
			
			throw new Exception();
		}

		private bool CanExecuteQuery(string queryToExecute)
		{
			try
			{
				TryExecutequery(queryToExecute);
			}
			catch (Exception ex)
			{
				return false;
			}

			return true;
		}
	}
	
	public class SitecorePreRequisitesChecker : IPostStep
	{
		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			var connectionStringLocator = new SitecoreInstallationConnectionStringLocator();

			var sitecoreInstallerLoggingService = new SitecoreInstallerLoggingService();

			var steps = new List<IPrerequisitStep>()
				{
					new CanCreateTablesSitecore(connectionStringLocator.LocateConnectionString(), sitecoreInstallerLoggingService),
					new CanModifyFiles(sitecoreInstallerLoggingService,FileUtil.MapPath("/")),
				};

			var checker = new PrerequisitesChecker(steps,new SitecoreInstallerLoggingService());

			string information;

			var meetsRequirements = checker.MeetsRequirement(out information);

			if (!meetsRequirements) throw new InstallationException(information);
		}
	}
}
