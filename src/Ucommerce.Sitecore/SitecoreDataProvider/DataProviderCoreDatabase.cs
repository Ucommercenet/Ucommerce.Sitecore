using System.Linq;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.DataProviders;
using Sitecore.Data.Managers;
using Ucommerce.Infrastructure;

namespace Ucommerce.Sitecore.SitecoreDataProvider
{
    public class DataProviderCoreDatabase : DataProviderAllDatabases
    {
        private readonly ID _contextMenusItemId = ID.Parse("{959F7FBB-1E68-4921-BFC7-10119122F6BB}");
        private readonly ID _listTypesNodesId = ID.Parse("{72352EFD-3C06-404D-93FA-737F669F41FA}");

        private VersionUriList _versions;
        private readonly object _lock = new object();

        public override ItemDefinition GetItemDefinition(ID itemId, CallContext context)
        {
            if (itemId == FieldIds.SystemContent.EmptyContextMenuNodeId)
            {
                return new ItemDefinition(itemId, "Empty Context Menu", TemplateIDs.Node, ID.Null);
            }

            if (itemId == FieldIds.SystemContent.UcommerceProductsMultilistWithSearch)
            {
                // Custom TreeList with Search, performance enhanced for displaying products.
                return new ItemDefinition(itemId, SitecoreConstants.FieldTypeProductsTreeListWithSearch,
                    TemplateIDs.TemplateFieldType, ID.Null);
            }

            if (itemId == FieldIds.SystemContent.UcommerceCategoriesTreeList)
            {
                // Custom TreeList, performance enhanced for displaying categories.
                return new ItemDefinition(itemId, SitecoreConstants.FieldTypeCategoriesTreeList,
                    TemplateIDs.TemplateFieldType, ID.Null);
            }

            return null;
        }

        public override IDList GetChildIDs(ItemDefinition itemDefinition, CallContext context)
        {
            if (itemDefinition.ID == _contextMenusItemId)
            {
                return new IDList { FieldIds.SystemContent.EmptyContextMenuNodeId };
            }

            if (itemDefinition.ID == _listTypesNodesId)
            {
                return new IDList
                {
                    FieldIds.SystemContent.UcommerceProductsMultilistWithSearch,
                    FieldIds.SystemContent.UcommerceCategoriesTreeList
                };
            }

            return null;
        }

        public override FieldList GetItemFields(ItemDefinition itemDefinition, VersionUri versionUri,
            CallContext context)
        {
            if (itemDefinition.ID == FieldIds.SystemContent.UcommerceProductsMultilistWithSearch)
            {
                return new FieldList
                {
                    {ID.Parse("{6BF5ED07-BC70-42F4-BB77-C017D340950E}"), "Ucommerce.Sitecore"},
                    {
                        ID.Parse("{9FE6154A-A16E-4827-99F6-F51533C34EC8}"),
                        "Ucommerce.Sitecore.SitecoreDataProvider.Impl.ProductsMultilistWithSearch"
                    }
                };
            }

            if (itemDefinition.ID == FieldIds.SystemContent.UcommerceCategoriesTreeList)
            {
                var fieldList = new FieldList
                {
                    {ID.Parse("{6BF5ED07-BC70-42F4-BB77-C017D340950E}"), "Ucommerce.Sitecore"},
                    {
                        ID.Parse("{9FE6154A-A16E-4827-99F6-F51533C34EC8}"),
                        "Ucommerce.Sitecore.SitecoreDataProvider.Impl.CategoriesTreelist"
                    }
                };

                return fieldList;
            }

            return null;
        }

        public override ID GetParentID(ItemDefinition itemDefinition, CallContext context)
        {
            if (itemDefinition.ID == FieldIds.SystemContent.EmptyContextMenuNodeId)
            {
                return _contextMenusItemId;
            }

            if (itemDefinition.ID == FieldIds.SystemContent.UcommerceProductsMultilistWithSearch)
            {
                return _listTypesNodesId;
            }

            if (itemDefinition.ID == FieldIds.SystemContent.UcommerceCategoriesTreeList)
            {
                return _listTypesNodesId;
            }

            return null;
        }

        public override VersionUriList GetItemVersions(ItemDefinition itemDefinition, CallContext context)
        {
            if ((itemDefinition.ID != FieldIds.SystemContent.UcommerceProductsMultilistWithSearch) &&
                (itemDefinition.ID != FieldIds.SystemContent.UcommerceCategoriesTreeList)) return null;

            if (_versions == null)
            {
                lock (_lock)
                {
                    if (_versions == null)
                    {
                        var languages = LanguageManager
                            .GetLanguages(Factory.GetDatabase(SitecoreConstants.SitecoreCoreDatabaseName)).Distinct();
                        var versions = new VersionUriList();
                        foreach (var language in languages)
                        {
                            versions.Add(language, Version.First);
                        }

                        _versions = versions;
                    }
                }
            }

            return _versions;
        }
    }
}