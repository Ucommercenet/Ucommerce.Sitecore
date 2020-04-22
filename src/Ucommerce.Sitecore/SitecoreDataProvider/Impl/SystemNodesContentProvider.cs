using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Ucommerce.Content;
using Ucommerce.EntitiesV2;
using Ucommerce.Extensions;
using Ucommerce.Security;
using Ucommerce.Sitecore.Extensions;
using Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.SystemData;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl
{
	internal class SystemNodesContentProvider
	{
		private readonly SystemNodeItem _currencies = new SystemNodeItem(FieldIds.SystemContent.CurrenciesFolderId, "Currencies"); // OK
		private readonly SystemNodeItem _emailProfiles = new SystemNodeItem(FieldIds.SystemContent.EmailProfilesFolderId, "Email profiles");
		private readonly SystemNodeItem _memberGroups = new SystemNodeItem(FieldIds.SystemContent.MemberGroupsFolderId, "Member groups");
		private readonly SystemNodeItem _memberTypes = new SystemNodeItem(FieldIds.SystemContent.MemberTypeFolderId, "Member types");
		private readonly SystemNodeItem _hostnames = new SystemNodeItem(FieldIds.SystemContent.HostnamesFolderId, "Hostnames");
		private readonly SystemNodeItem _orderNumberSeries = new SystemNodeItem(FieldIds.SystemContent.OrderNumberSeriesFolderId, "Order number series");
		private readonly SystemNodeItem _priceGroups = new SystemNodeItem(FieldIds.SystemContent.PriceGroupsFolderId, "Price groups"); // OK
		private readonly SystemNodeItem _dataTypeEnums = new SystemNodeItem(FieldIds.SystemContent.DataTypeEnums, "Data type enums");

		private readonly SystemNodeItem _emptyContextMenuNode = new SystemNodeItem(FieldIds.SystemContent.EmptyContextMenuNodeId, "Empty Context Menu");

		private readonly IDomainService _domainService;
		private readonly IMemberService _memberService;

		private const string _iconFolder = SitecoreConstants.UCommerceIconFolder;
		public SystemNodesContentProvider(IDomainService domainService, IMemberService memberService)

		{
			_domainService = domainService;
			_memberService = memberService;
		}

		/// <summary>
		/// Returns all the uCommerce system data.
		/// </summary>
		/// <returns>All uCommerce system data to represent in the Sitecore content tree.</returns>
		public IList<ISitecoreItem> GetSystemData()
		{
			var list = new List<ISitecoreItem>();

			list.Add(BuildItemNodes());

			return list;
		}

		private ISitecoreItem BuildItemNodes()
		{
			var root = new SystemNodeItem(FieldIds.SystemContent.UCommerceSystemDataFolderId, "uCommerce");
			root.AddToFieldList(FieldIDs.Icon, _iconFolder + "/ucommerce-logo.png");
			root.AddItem(_currencies);
			root.AddItem(_emailProfiles);
			root.AddItem(_memberGroups);
			root.AddItem(_memberTypes);
			root.AddItem(_hostnames);
			root.AddItem(_orderNumberSeries);
			root.AddItem(_priceGroups);
			root.AddItem(_dataTypeEnums);
			root.AddItem(_emptyContextMenuNode);

			BuildCurrencyItems();
			BuildEmailProfiles();
			BuildMemberGroups();
			BuildMemberTypes();
			BuildOrderNumberSeries();
			BuildPriceGroupItems();
			BuildDataTypeEnums();
			BuildHostnames();

			return root;
		}

		private void BuildDataTypeEnums()
		{
			var dataTypes = DataType.Find(x => x.DefinitionName == "Enum" || x.DefinitionName == "EnumMultiSelect").ToList();
			foreach (var dataType in dataTypes)
			{
				var id = dataType.SitecoreIdForEnum();
				var dataTypeItem = new SystemItem(id, dataType.Name);
				dataTypeItem.AddToFieldList(FieldIDs.Icon,_iconFolder+"/ui/ui-combo-box.png");
				foreach (var dataTypeEnum in dataType.DataTypeEnums)
				{
					var dataTypeEnumId = dataTypeEnum.SitecoreId();
					var dataTypeEnumItem = new SystemItem(dataTypeEnumId, dataTypeEnum.Name);
					dataTypeEnumItem.AddToFieldList(FieldIDs.Icon, _iconFolder + "/ui/ui-button.png");

					dataTypeItem.AddItem(dataTypeEnumItem);
				}
				dataTypeItem.SetRevision(dataType.Guid.Derived(dataType.ModifiedOn));
				_dataTypeEnums.AddItem(dataTypeItem);
			}
			_dataTypeEnums.AddToFieldList(FieldIDs.Icon, _iconFolder + "/ui/ui-scroll-pane-list.png");
		}

		// Price groups
		private void BuildPriceGroupItems()
		{
			var priceGroups = PriceGroup.Find(x => !x.Deleted).ToList();
			foreach (var pg in priceGroups)
			{
				var id = pg.SitecoreId();
				var item = new SystemItem(id, GetPriceGroupName(pg));
				item.AddToFieldList(FieldIDs.Icon, _iconFolder + "/ui/money-coin.png");

				_priceGroups.AddItem(item);
			}
			_priceGroups.AddToFieldList(FieldIDs.Icon, _iconFolder + "/ui/money.png");
			_priceGroups.SetRevision(Guid.NewGuid());
		}

		private string GetPriceGroupName(PriceGroup priceGroup)
		{
			return string.Format("{0} ({1})", priceGroup.Name, priceGroup.Currency.ISOCode);
		}

		// Email profiles
		private void BuildEmailProfiles()
		{
			var emailProfiles = EmailProfile.Find(x => !x.Deleted).ToList();
			foreach (var profile in emailProfiles)
			{
				var id = profile.SitecoreId();
				var item = new SystemItem(id, profile.Name);
				item.AddToFieldList(FieldIDs.Icon, _iconFolder + "/ui/mails.png");
				_emailProfiles.AddItem(item);
			}
			_emailProfiles.AddToFieldList(FieldIDs.Icon, _iconFolder + "/ui/mail-open-table.png");
			_emailProfiles.SetRevision(Guid.NewGuid());
		}

		// Member groups
		private void BuildMemberGroups()
		{
			var memberGroups = _memberService.GetMemberGroups();
			foreach (var memberGroup in memberGroups)
			{
				var id = memberGroup.SitecoreId();
				var item = new SystemItem(id, memberGroup.Name);
				item.AddToFieldList(FieldIDs.Icon, _iconFolder + "/ui/id_cards.png");

				_memberGroups.AddItem(item);
			}
			_memberGroups.AddToFieldList(FieldIDs.Icon, _iconFolder + "/ui/users4.png");
			_memberGroups.SetRevision(Guid.NewGuid());
		}

		// Member types
		private void BuildMemberTypes()
		{
			var memberTypes = _memberService.GetMemberTypes();
			foreach (var memberType in memberTypes)
			{
				var id = memberType.SitecoreId();
				var item = new SystemItem(id, memberType.Name);
				item.AddToFieldList(FieldIDs.Icon, _iconFolder + "/ui/user1.png");
				_memberTypes.AddItem(item);
			}

			_memberTypes.SetRevision(Guid.NewGuid());
		}

		// Order number series.
		private void BuildOrderNumberSeries()
		{
			var orderNumberSeries = OrderNumberSerie.Find(x => !x.Deleted).ToList();
			foreach (var serie in orderNumberSeries)
			{
				var id = serie.SitecoreId();
				var item = new SystemItem(id, serie.OrderNumberName);
				item.AddToFieldList(FieldIDs.Icon, _iconFolder + "/ui/counter.png");
				_orderNumberSeries.AddItem(item);
			}
			_orderNumberSeries.AddToFieldList(FieldIDs.Icon, _iconFolder + "/ui/counter-count-up.png");
			_orderNumberSeries.SetRevision(Guid.NewGuid());

		}

		// Currency
		private void BuildCurrencyItems()
		{
			var currencies = Currency.Find(x => !x.Deleted).ToList();
			foreach (var currency in currencies)
			{
				var id = currency.SitecoreId();
				var item = new SystemItem(id, currency.Name);
				item.AddToFieldList(FieldIDs.Icon, _iconFolder + "/ui/money--arrow.png");


				_currencies.AddItem(item);
			}
			_currencies.AddToFieldList(FieldIDs.Icon, _iconFolder + "/ui/currency-euro.png");
			_currencies.SetRevision(Guid.NewGuid());
		}

		// Hostnames
		private void BuildHostnames()
		{
			var domains = _domainService.GetDomains();

			var defaultItem = new SystemItem(FieldIds.SystemContent.DefaultHostnameId, "Default");
			defaultItem.AddToFieldList(FieldIDs.Icon, _iconFolder + "/ui/globe-medium-green.png");

			_hostnames.AddItem(defaultItem);

			foreach (var domain in domains)
			{
				var id = domain.SitecoreId();
				var item = new SystemItem(id, domain.DomainName);
				item.AddToFieldList(FieldIDs.Icon, _iconFolder + "/ui/globe-medium-green.png");
				_hostnames.AddItem(item);
			}
			_hostnames.AddToFieldList(FieldIDs.Icon, _iconFolder + "/ui/globe-green.png");
			_hostnames.SetRevision(Guid.NewGuid());
		}
	}
}
