using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Sitecore;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Xml;
using Ucommerce.Api;
using Ucommerce.Infrastructure;
using Ucommerce.Search;
using Ucommerce.Search.Models;
using Match = System.Text.RegularExpressions.Match;

namespace Ucommerce.Sitecore.Pipelines
{
    public class ItemResolver : HttpRequestProcessor
    {
        private readonly List<RewriteRule> _rewriteRules = new List<RewriteRule>();
        private IIndex<Category> _categoryIndex => ObjectFactory.Instance.Resolve<IIndex<Category>>();
        private IIndex<Product> _productIndex => ObjectFactory.Instance.Resolve<IIndex<Product>>();
        private IIndex<ProductCatalog> _productCatalogIndex => ObjectFactory.Instance.Resolve<IIndex<ProductCatalog>>();
        private ICatalogContext _catalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();

        public void AddRules(XmlNode node)
        {
            var ruleFor = XmlUtil.GetAttribute("for", node);
            var ruleMatch = XmlUtil.GetAttribute("match", node);

            var rule = new RewriteRule
            {
                RuleFor = ruleFor,
                RuleMatch = ruleMatch,
            };

            _rewriteRules.Add(rule);
        }

        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (Context.Item != null || Context.Database == null || args.Url.ItemPath.Length == 0)
                return;

            foreach (var rule in _rewriteRules)
            {
                var re = new Regex(rule.RuleMatch, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var match = re.Match(args.Url.FilePath);

                if (match.Success)
                {
                    SetContextByRule(rule.RuleFor, match, args);

                    return;
                }
            }
        }

        private void SetContextByRule(string ruleFor, Match match, HttpRequestArgs args)
        {
            var catalogSlug = match.Groups["catalog"].ToString();
            var categorySlug = match.Groups["category"].ToString();
            var categoriesSlugs = match.Groups["categories"].Value
	            .Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries).ToList();
            var productSlug = match.Groups["product"].ToString();
            var variantSlug = match.Groups["variant"].ToString();

            ID id;
            switch (ruleFor)
            {
                case SitecoreConstants.RewriteCategoryProduct:
	                var currentProduct = GetProductBySlug(productSlug);
	                if (currentProduct == null) break;

		            id = new ID(currentProduct.Guid);
                    if (id == ID.Null) break;

                    Context.Item = Context.Database.GetItem(id);
	                SetContextForProductWithCategory(catalogSlug, categoriesSlugs, productSlug);
                    break;
                case SitecoreConstants.RewriteCategory:
	                var currentCategory = GetCategoryBySlug(categorySlug);
	                if (currentCategory == null) break;

	                id = new ID(currentCategory.Guid);
                    if (id == ID.Null) break;

                    Context.Item = Context.Database.GetItem(id);
		            SetContextForCategory(catalogSlug, categoriesSlugs, categorySlug);
                    break;
                case SitecoreConstants.RewriteCatalog:
	                var currentCatalog = GetCatalogBySlug(catalogSlug);
	                if (currentCatalog == null) break;

	                id = new ID(currentCatalog.Guid);
                    if (id == ID.Null) break;

					Context.Item = Context.Database.GetItem(id);
		            SetContextForCatalog(catalogSlug);
                    break;
                case SitecoreConstants.RewriteVariant:
	                var currentVariant = GetProductBySlug(variantSlug);
	                if (currentVariant == null) break;

	                id = new ID(currentVariant.Guid);
	                if(id == ID.Null) break;

	                Context.Item = Context.Database.GetItem(id);
	                SetContextForVariant(catalogSlug, categorySlug, productSlug, variantSlug);
	                break;
            }
        }

        protected virtual ProductCatalog GetCatalogBySlug(string catalogSlug)
        {
	        return _productCatalogIndex.Find().Where(catalog => catalog.Slug == catalogSlug)
		        .FirstOrDefault();
        }

        protected virtual Category GetCategoryBySlug(string categorySlug)
        {
	        return _categoryIndex.Find().Where(category => category.Slug == categorySlug)
		        .FirstOrDefault();
        }

        protected virtual Product GetProductBySlug(string productSlug)
        {
	        return _productIndex.Find().Where(product => product.Slug == productSlug).FirstOrDefault();
        }

        protected virtual List<Category> GetCategoriesBySlugs(List<string> categoriesSlugs)
        {
	        return _categoryIndex.Find()
		        .Where(c => categoriesSlugs.Contains(c.Slug) && c.ProductCatalog == _catalogContext.CurrentCatalog.Guid)
		        .ToList().ToList();
        }
        private void SetContextForProductWithCategory(string catalogSlug, List<string> categoriesSlugs, string productSlug)
        {
	        _catalogContext.CurrentCatalog = GetCatalogBySlug(catalogSlug);
	        _catalogContext.CurrentCategories = GetCategoriesBySlugs(categoriesSlugs);
	        _catalogContext.CurrentProduct = GetProductBySlug(productSlug);
        }

        private void SetContextForCategory(string catalogSlug, List<string> categoriesSlugs, string categorySlug)
        {
	        _catalogContext.CurrentCatalog = GetCatalogBySlug(catalogSlug);
	        _catalogContext.CurrentCategories = GetCategoriesBySlugs(categoriesSlugs);
	        _catalogContext.CurrentCategory = GetCategoryBySlug(categorySlug);
        }

        private void SetContextForCatalog(string catalogSlug)
        {
	        _catalogContext.CurrentCatalog = GetCatalogBySlug(catalogSlug);
        }

        private void SetContextForVariant(string catalogSlug, string categorySlug, string productSlug,
	        string variantSlug)
        {
	        _catalogContext.CurrentCatalog = GetCatalogBySlug(catalogSlug);
	        _catalogContext.CurrentCategory = GetCategoryBySlug(categorySlug);
	        _catalogContext.CurrentProduct = GetProductBySlug(productSlug);
	        // TODO: Set _catalogContext.CurrentVariant when it is added to the ICatalogContext interface.
        }

        private struct RewriteRule
        {
            public string RuleFor;
            public string RuleMatch;
        }
    }
}
