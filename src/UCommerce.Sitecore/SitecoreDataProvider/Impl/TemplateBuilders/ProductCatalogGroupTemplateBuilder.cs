using System;
using System.Collections.Generic;
using System.IO;
using Sitecore.Data;
using Sitecore.Data.Items;
using UCommerce.Content;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Logging;
using UCommerce.Pipelines;
using UCommerce.Security;
using UCommerce.Sitecore.Extensions;
using UCommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders.BaseTemplates;
using UCommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders.BaseTemplates.TemplateBuilderExtentions;
using UCommerce.Tree;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders
{
	internal class ProductCatalogGroupTemplateBuilder : ITemplateBuilder
	{
		private Guid _revision = new Guid("{D96BE829-74BB-47AB-A9B7-8EBB347FC293}");

		private readonly BaseProductCatalogGroupTemplate _baseProductCatalogGroupTemplate;
		private ILoggingService _loggingService;

		public ProductCatalogGroupTemplateBuilder(ILoggingService loggingService)
		{
			_loggingService = loggingService;
			_baseProductCatalogGroupTemplate = new BaseProductCatalogGroupTemplate();
		}

		public ID GetTemplateId(ITreeNodeContent node)
		{
			return BaseProductCatalogGroupTemplate.ProductCatalogGroupBaseTemplateId;
		}

		public void AddFieldValues(ITreeNodeContent node, FieldList list, VersionUri version)
		{
			var store = ProductCatalogGroup.Get(int.Parse(node.ItemId));

			if (store != null)
				_baseProductCatalogGroupTemplate.AddBaseFieldValues(store, list, version);
		}

		public bool SaveItem(ITreeNodeContent node, ItemChanges changes)
		{
			if (!changes.HasFieldsChanged) return false;

			var store = ProductCatalogGroup.Get(int.Parse(node.ItemId));
			if (store == null)
			{
				string message = string.Format("Store with id: {0} not found for ITreeNodeContent. ", node.ItemId);
				_loggingService.Log<ProductCatalogGroupTemplateBuilder>(message);
				throw new InvalidDataException(message);
			}
			
			foreach (FieldChange fieldChange in changes.FieldChanges) UpdateStoreValueFor(fieldChange, store,changes);

			ObjectFactory.Instance.Resolve<IPipeline<ProductCatalogGroup>>("SaveProductCatalogGroup").Execute(store);
			
			return true;
		}

		private void UpdateStoreValueFor(FieldChange fieldChange, ProductCatalogGroup store,ItemChanges changes)
		{
			if (this.ValueDidNotChangeFor(fieldChange)) return;

			//We do NOT handle statistics e.g updated at and updated by this way. Underlying Repositories deals with it.
			if (this.FieldBelongsToStatistics(fieldChange)) return; 

			if (fieldChange.FieldID == FieldIds.Store.CreateCustomersAsMembersFieldId)
				store.CreateCustomersAsMembers = (fieldChange.Value == "1");
			else if (fieldChange.FieldID == FieldIds.Store.ProductCatalogGroupIdFieldId)
				return;
			else if (fieldChange.FieldID == FieldIds.Store.CurrencyFieldId)
				UpdateStoreCurrency(store, fieldChange.Value);
			else if (fieldChange.FieldID == FieldIds.Store.DescriptionFieldId)
				store.Description = fieldChange.Value;
			else if (fieldChange.FieldID == FieldIds.Store.EmailProfileFieldId)
				UpdateStoreEmailProfile(fieldChange.Value, store);
			else if (fieldChange.FieldID == FieldIds.Store.HostNameFieldId)
				UpdateHostname(fieldChange.Value, store);
			else if (fieldChange.FieldID == FieldIds.Store.MemberGroupFieldId)
				UpdateStoreMemberGroup(fieldChange.Value, store);
			else if (fieldChange.FieldID == FieldIds.Store.MemberTypeFieldId)
				UpdateStoreMemberType(fieldChange.Value, store);
			else if (fieldChange.FieldID == FieldIds.Store.NameFieldId)
				UpdateStoreName(store, fieldChange.Value, changes);
			else if (fieldChange.FieldID == FieldIds.Store.OrderNumberSeriesFieldId)
				UpdateStoreOrderNumbers(fieldChange.Value, store);
			else if (fieldChange.FieldID == FieldIds.Store.ProductReviewRequiresApprovalFieldId)
				store.ProductReviewsRequireApproval = (fieldChange.Value == "1");
			else
			{
				_loggingService.Log<ProductCatalogGroupTemplateBuilder>(
					string.Format("Could not find property: {0} for store: {1}.",fieldChange.Definition.Key,store.Name));
			}
		}

		private void UpdateStoreMemberGroup(string value, ProductCatalogGroup store)
		{
			var memberService = ObjectFactory.Instance.Resolve<IMemberService>();

			foreach (var memberGroup in memberService.GetMemberGroups())
			{
				if (memberGroup.SitecoreId().ToString() == value)
				{
					store.MemberGroupId = memberGroup.MemberGroupId;
					return;
				}
			}

			store.MemberGroupId = "-1";
		}

		private void UpdateStoreMemberType(string value, ProductCatalogGroup store)
		{
			var memberService = ObjectFactory.Instance.Resolve<IMemberService>();

			foreach (var memberType in memberService.GetMemberTypes())
			{
				if (memberType.SitecoreId().ToString() == value)
				{
					store.MemberTypeId = memberType.MemberTypeId;
					return;
				}
			}

			store.MemberTypeId = "-1";
		}

		private void UpdateHostname(string value, ProductCatalogGroup store)
		{
			if (string.IsNullOrEmpty(value))
			{
				store.DomainId = string.Empty;
				return;
			}

			ID id;
			if (ID.TryParse(value, out id))
			{
				if (id == FieldIds.SystemContent.DefaultHostnameId)
				{
					store.DomainId = string.Empty;
					return;
				}

				var domainService = ObjectFactory.Instance.Resolve<IDomainService>();

				foreach (var domain in domainService.GetDomains())
				{
					if (domain.SitecoreId() == id)
					{
						store.DomainId = domain.DomainId;
						return;
					}
				}

				return;
			}
			
			_loggingService.Log<ProductCatalogGroupTemplateBuilder>(
				string.Format("Failed to update host name for store. Could not find Sitecore ID for domain with id: {0}.",
				              value));
		}

		private void UpdateStoreOrderNumbers(string value, ProductCatalogGroup store)
		{
			ID id;
			if (ID.TryParse(value, out id))
			{
				var orderNumbers = OrderNumberSerie.SingleOrDefault(x => x.Guid == id.Guid);
				if (orderNumbers == null)
				{
					_loggingService.Log<ProductCatalogGroupTemplateBuilder>(
						string.Format("Failed to update order number series for store. Could not find order numbers serie with guid: {0}.",
						              id.Guid));
					return;
				}

				store.OrderNumberSerie = orderNumbers;
			}
			else
			{
				_loggingService.Log<ProductCatalogGroupTemplateBuilder>(
					string.Format("Failed to update order number series for store. Could not find Sitecore ID for order numbers serie with id: {0}.",
								  value));
			}
		}

		private void UpdateStoreEmailProfile(string value, ProductCatalogGroup store)
		{
			ID id;
			if (ID.TryParse(value, out id))
			{
				var emailProfile = EmailProfile.SingleOrDefault(x => x.Guid == id.Guid);
				if (emailProfile == null)
				{
					_loggingService.Log<ProductCatalogGroupTemplateBuilder>(
						string.Format("Failed to update email profile for store. Could not find email profile with guid: {0}.", id.Guid));
					return;
				}

				store.EmailProfile = emailProfile;
			}
			else
			{
				_loggingService.Log<ProductCatalogGroupTemplateBuilder>(
					string.Format("Failed to update email profile for store. Could not find Sitecore ID for email profile with id: {0}.", value));
			}
		}

		private void UpdateStoreCurrency(ProductCatalogGroup store, string value)
		{
			ID id;
			if (ID.TryParse(value, out id))
			{
				var currency = Currency.SingleOrDefault(x => x.Guid == id.Guid);
				if (currency == null)
				{
					_loggingService.Log<ProductCatalogGroupTemplateBuilder>(
						string.Format("Failed to update currency for store. Could not find currency with guid: {0}.", id.Guid));

					return;
				}

				store.Currency = currency;
			}
			else
			{
				_loggingService.Log<ProductCatalogGroupTemplateBuilder>(
					string.Format("Failed to update currency for store. Could not find Sitecore ID for currency with id: {0}.", value));
			}
		}

		private void UpdateStoreName(ProductCatalogGroup store, string value, ItemChanges changes)
		{
			if (ProductCatalogGroup.SingleOrDefault(x => x.Name == value) != null)
			{
				_loggingService.Log<ProductCatalogGroupTemplateBuilder>(string.Format("Failed to update store name for store. Store with name: {0} already exists.", value));

				return;
			}
			store.Name = value;
		}

		public bool Supports(ITreeNodeContent node)
		{
			return node.NodeType == "productCatalogGroup";
		}

		public IEnumerable<ISitecoreItem> BuildTemplates()
		{
			var template = _baseProductCatalogGroupTemplate.BuildBaseStoreTemplate();
			template.SetRevision(_revision);
			return new List<ISitecoreItem> { template };
		}
	}
}
