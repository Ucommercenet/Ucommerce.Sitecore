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
		private readonly IInstallerLoggingService _log;

		private readonly Dictionary<int, Guid> _dictionaryOfTemplateIds = new Dictionary<int, Guid>();
	
		private readonly Dictionary<int, Guid> _dictionaryOfVariantTemplateIds = new Dictionary<int, Guid>();
		private Dictionary<Guid, int> _reversedDictionaryOfVariantTemplateIds;
		
		private readonly Dictionary<Guid, Guid> _dictionaryTemplateIdToStandardValueItemId = new Dictionary<Guid, Guid>();
		private readonly List<IDTableEntry> _entriesToBeDeleted = new List<IDTableEntry>();

		public MigrateIdTableValues()
		{
			_log = new SitecoreInstallerLoggingService();
			var locator = new SitecoreConnectionStringLocator();
			ConnectionString = locator.Locate();
			SqlStatements = new List<string>();
		}

		private string ConnectionString { get; set; }
		private IList<string> SqlStatements { get; set; }

		/// <summary>
		/// Migrates the current IDTable data to the new IDTable ID scheme.
		/// </summary>
		/// <remarks>
		/// Runs through all the uCommerce entries in the IDTable.
		/// Each entry is examined to determine what action to perform.
		/// 
		/// If the entry corresponds to a uCommerce entity, that entity's Guid is updated.
		/// If the corresponging entry cannot be updated, then instead we:
		///  1. Migrate the standard values to the new ID.
		///  2. Delte the IDTable entry to get it to be recreated with the new ID.
		/// </remarks>
		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			if (DataIsAlreadyMigrated()) { return; }

			var watch = new Stopwatch();
			watch.Start();
			
			var entries = GetAllUCommerceIdTableEntries();

			foreach (var entry in entries)
			{
				// This also populates the dictionaries with the data used by the Migrate...() methods called below.
				PrepareMigrationOfEntry(entry);
			}

			MigrateVariantTemplateIds();
			MigrateStandardValueItemIds();

			DeleteEntriesToBeRegeneratedWithNewId();

			_log.Log<MigrateIdTableValues>("Combined sql statement for migration of IDTable data: " + CombineStatementsToSingleStatement());
			RunAllSqlStatementsAgainstDatabase();

			MarkDataAsMigrated();

			watch.Stop();
			_log.Log<MigrateIdTableValues>(string.Format("Migration of IDTable entries took {0} ms", watch.ElapsedMilliseconds));
		}

		private static IEnumerable<IDTableEntry> GetAllUCommerceIdTableEntries()
		{
			return IDTable.GetKeys("uCommerceDataProvider");
		}

		private void DeleteEntriesToBeRegeneratedWithNewId()
		{
			while (_entriesToBeDeleted.Count > 0)
			{
				var first = _entriesToBeDeleted.First();
				_entriesToBeDeleted.Remove(first);

				IDTable.RemoveKey(first.Prefix, first.Key);
				_log.Log<MigrateIdTableValues>(string.Format("IDTable.RemoveKey({0},{1})", first.Prefix, first.Key));
			}
		}

		private void MigrateVariantTemplateIds()
		{
			foreach (var key in _dictionaryOfVariantTemplateIds.Keys)
			{
				var originalTemplateId = _dictionaryOfTemplateIds[key];
				var newVariantTemplateId = originalTemplateId.Derived("VariantTemplate");
				MigrateStandardTemplateFieldValues(_dictionaryOfVariantTemplateIds[key], newVariantTemplateId);
			}
		}

		private void MigrateStandardValueItemIds()
		{
			_reversedDictionaryOfVariantTemplateIds = _dictionaryOfVariantTemplateIds.Keys.ToDictionary(x => _dictionaryOfVariantTemplateIds[x]);
			foreach (var key in _dictionaryTemplateIdToStandardValueItemId.Keys)
			{
				Guid newTemplateGuid = FindNewTemplateGuid(key);
				Guid newStandardValuesGuid = newTemplateGuid.Derived("__Standard Values");
				MigrateStandardTemplateFieldValues(_dictionaryTemplateIdToStandardValueItemId[key], newStandardValuesGuid);
			}
		}

		private void MigrateStandardTemplateFieldValues(Guid oldValue, Guid newValue)
		{
			if (oldValue == newValue) return;

			var sql = string.Format("update uCommerce_UnversionedField set ItemId = '{0}' where ItemId = '{1}';", newValue, oldValue);
			SqlStatements.Add(sql);
			sql = string.Format("update uCommerce_VersionedField set ItemId = '{0}' where ItemId = '{1}';", newValue, oldValue);
			SqlStatements.Add(sql);
			sql = string.Format("update uCommerce_SharedField set ItemId = '{0}' where ItemId = '{1}';", newValue, oldValue);
			SqlStatements.Add(sql);
		}

		private Guid FindNewTemplateGuid(Guid key)
		{
			if (_reversedDictionaryOfVariantTemplateIds.ContainsKey(key))
			{
				var mainKey = _dictionaryOfTemplateIds[_reversedDictionaryOfVariantTemplateIds[key]];
				return mainKey.Derived("VariantTemplate");
			}
			// No change in ID.
			return key;
		}

		private void PrepareMigrationOfEntry(IDTableEntry entry)
		{
			if (entry.Key.StartsWith("{"))
			{
				PrepareMigrationOfStandardValuesItem(entry);
				_entriesToBeDeleted.Add(entry);
				return;
			}
			var nodetype = GetNodeTypeFromKey(entry.Key);
			switch (nodetype)
			{
				case "product":
				case "productVariant":
					PrepareMigrationOfEntity(entry, BuildSqlToUpgradeProduct);
					break;
				case "productCategory":
					PrepareMigrationOfEntity(entry, BuildSqlToUpgradeCategory);
					break;
				case "productCatalog":
					PrepareMigrationOfEntity(entry, BuildSqlToUpgradeCatalog);
					break;
				case "productCatalogGroup":
					PrepareMigrationOfEntity(entry, BuildSqlToUpgradeStore);
					break;
				case "currency":
					PrepareMigrationOfEntity(entry, BuildSqlToUpgradeCurrency);
					break;
				case "productDefinitionTemplate":
					_dictionaryOfTemplateIds.Add(GetIdFromKey(entry.Key), entry.ID.Guid);
					PrepareMigrationOfEntity(entry, BuildSqlToUpgradeProductDefinition);
					break;
				case "productDefinitionTemplateVariant":
					_dictionaryOfVariantTemplateIds.Add(GetIdFromKey(entry.Key), entry.ID.Guid);
					_entriesToBeDeleted.Add(entry);
					break;
				case "definitionTemplate":
					PrepareMigrationOfEntity(entry, BuildSqlToUpgradeCategoryDefinition);
					break;
				case "ProductDefinitionFieldTemplateField":
				case "ProductDefinitionFieldTemplateFieldForVariant":
					PrepareMigrationOfEntity(entry, BuildSqlToUpgradeProductDefinitionField);
					break;
				case "CategoryDefinitionDynamicFieldsField":
					PrepareMigrationOfEntity(entry, BuildSqlToUpgradeCategoryDefinitionField);
					break;
				default:
					_log.Log<MigrateIdTableValues>(string.Format("Skipping migration of entry with key: {0}", entry.Key));
					break;
			}
		}

		private void PrepareMigrationOfStandardValuesItem(IDTableEntry entry)
		{
			_dictionaryTemplateIdToStandardValueItemId.Add(entry.ParentID.Guid, entry.ID.Guid);
		}

		private void PrepareMigrationOfEntity(IDTableEntry entry, Func<int, Guid, string> sqlBuilder)
		{
			int id = GetIdFromKey(entry.Key);
			string sql = sqlBuilder(id, entry.ID.Guid);
			StoreSqlCommandForLaterExecution(sql);
		}

		private void StoreSqlCommandForLaterExecution(string sql)
		{
			SqlStatements.Add(sql);
		}

		private string BuildSqlToUpgradeProduct(int id, Guid guid)
		{
			return string.Format("update uCommerce_Product set Guid = '{0}' where ProductId = {1};", guid, id);
		}

		private string BuildSqlToUpgradeCategory(int id, Guid guid)
		{
			return string.Format("update uCommerce_Category set Guid = '{0}' where CategoryId = {1};", guid, id);
		}

		private string BuildSqlToUpgradeCatalog(int id, Guid guid)
		{
			return string.Format("update uCommerce_ProductCatalog set Guid = '{0}' where ProductCatalogId = {1};", guid, id);
		}

		private string BuildSqlToUpgradeStore(int id, Guid guid)
		{
			return string.Format("update uCommerce_ProductCatalogGroup set Guid = '{0}' where ProductCatalogGroupId = {1};", guid, id);
		}

		private string BuildSqlToUpgradeCurrency(int id, Guid guid)
		{
			return string.Format("update uCommerce_Currency set Guid = '{0}' where CurrencyId = {1};", guid, id);
		}

		private string BuildSqlToUpgradeProductDefinition(int id, Guid guid)
		{
			return string.Format("update uCommerce_ProductDefinition set Guid = '{0}' where ProductDefinitionId = {1};", guid, id);
		}

		private string BuildSqlToUpgradeCategoryDefinition(int id, Guid guid)
		{
			return string.Format("update uCommerce_Definition set Guid = '{0}' where DefinitionId = {1};", guid, id);
		}

		private string BuildSqlToUpgradeProductDefinitionField(int id, Guid guid)
		{
			return string.Format("update uCommerce_ProductDefinitionField set Guid = '{0}' where ProductDefinitionFieldId = {1};", guid, id);
		}

		private string BuildSqlToUpgradeCategoryDefinitionField(int id, Guid guid)
		{
			return string.Format("update uCommerce_DefinitionField set Guid = '{0}' where DefinitionFieldId = {1};", guid, id);
		}

		private void RunAllSqlStatementsAgainstDatabase()
		{
			var combinedSqlStatement = CombineStatementsToSingleStatement();
			using (var conn = new SqlConnection(ConnectionString))
			{
				try
				{
					var cmd = new SqlCommand { Connection = conn };
					conn.Open();
					cmd.CommandText = combinedSqlStatement;
					cmd.CommandTimeout = 600;
					cmd.ExecuteNonQuery();
				}				
				catch (SqlException exception)
				{
					_log.Log<DbInstaller>(exception,
											 string.Format(
												"Error running sql statement:{1},  error: {0}.", exception,
												combinedSqlStatement));
				}
			}
		}

		private string CombineStatementsToSingleStatement()
		{
			var sb = new StringBuilder();
			sb.AppendLine("BEGIN TRANSACTION");
			foreach (var statement in SqlStatements)
			{
				sb.AppendLine(statement);
			}
			sb.AppendLine("COMMIT TRANSACTION");
			return sb.ToString();
		}

		private bool DataIsAlreadyMigrated()
		{
			// TODO: Find a way to recognize that the data has been migrated.
			return false;
		}

		private void MarkDataAsMigrated()
		{
			// TODO: Find a way to store the knowledge that the data was migrated.
		}

		private int GetIdFromKey(string key)
		{
			var keyParts = key.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
			if (keyParts.Length != 2)
			{
				throw new ArgumentException("The key is not on the expected format. " + key);
			}
			return int.Parse(keyParts[1]);
		}

		private string GetNodeTypeFromKey(string key)
		{
			var keyParts = key.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
			if (keyParts.Length != 2)
			{
				throw new ArgumentException("The key is not on the expected format. " + key);
			}
			return keyParts[0];
		}
	}
}
