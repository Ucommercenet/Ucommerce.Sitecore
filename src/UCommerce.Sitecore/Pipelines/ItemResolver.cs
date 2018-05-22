using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using Sitecore;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Xml;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Runtime;

namespace UCommerce.Sitecore.Pipelines
{
    public class ItemResolver : HttpRequestProcessor
    {
        private readonly List<RewriteRule> _rewriteRules = new List<RewriteRule>();

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

            var catalogContext = ObjectFactory.Instance.Resolve<ICatalogContext>();

            foreach (var rule in _rewriteRules)
            {
                var re = new Regex(rule.RuleMatch, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var match = re.Match(args.Url.FilePath);
                
                if (match.Success)
                {
                    SetContextByRule(rule.RuleFor, match, catalogContext);

                    return;
                }
            }
        }

        private void SetContextByRule(string ruleFor, Match match, ICatalogContext catalogContext)
        {
            var catalogIdAsString = match.Groups["productCatalog"].ToString();
            var categoryIdAsString = match.Groups["productCategory"].ToString();
            var productIdAsString = match.Groups["product"].ToString();

	        int catalogId;
	        int categoryId;
	        int productId;

	        int.TryParse(catalogIdAsString, out catalogId);
			int.TryParse(categoryIdAsString, out categoryId);
			int.TryParse(productIdAsString, out productId);

            ID id;
            switch (ruleFor)
            {
                case SitecoreConstants.RewriteCategoryProduct:
		            id = FindSitecoreIdForProduct(int.Parse(productIdAsString));
                    if (id == ID.Null) break;

                    Context.Item = Context.Database.GetItem(id);
					SetContextForProductWithCategory(catalogId, categoryId, productId, catalogContext);
                    break;
                case SitecoreConstants.RewriteProduct:
		            id = FindSitecoreIdForProduct(productId);
                    if (id == ID.Null) break;

                    Context.Item = Context.Database.GetItem(id);
					SetContextForProduct(catalogId, productId, catalogContext);
                    break;
                case SitecoreConstants.RewriteCategory:
		            id = FindSitecoreIdForCategory(categoryId);
                    if (id == ID.Null) break;

                    Context.Item = Context.Database.GetItem(id);
					SetContextForCategory(catalogId, categoryId, catalogContext);
                    break;
                case SitecoreConstants.RewriteCatalog:
		            id = FindSitecoreIdForCatalog(catalogId);
                    if (id == ID.Null) break;

					Context.Item = Context.Database.GetItem(id);
					SetContextForCatalog(catalogId, catalogContext);
                    break;
            }
        }

        private void SetContextForProductWithCategory(int catalogId, int categoryId, int productId, ICatalogContext catalogContext)
        {
            catalogContext.CurrentCatalog = ProductCatalog.Get(catalogId);
            catalogContext.CurrentCategory = Category.Get(categoryId);
            catalogContext.CurrentProduct = Product.Get(productId);
        }

        private void SetContextForProduct(int catalogId, int productId, ICatalogContext catalogContext)
        {
            catalogContext.CurrentCatalog = ProductCatalog.Get(catalogId);
            catalogContext.CurrentProduct = Product.Get(productId);
        }

        private void SetContextForCategory(int catalogId, int categoryId, ICatalogContext catalogContext)
        {
            catalogContext.CurrentCatalog = ProductCatalog.Get(catalogId);
            catalogContext.CurrentCategory = Category.Get(categoryId);
        }

        private void SetContextForCatalog(int catalogId, ICatalogContext catalogContext)
        {
            catalogContext.CurrentCatalog = ProductCatalog.Get(catalogId);
        }

        private struct RewriteRule
        {
            public string RuleFor;
            public string RuleMatch;
        }

	    private ID FindSitecoreIdForProduct(int productId)
	    {
		    var repository = ObjectFactory.Instance.Resolve<IRepository<Product>>();

		    var product = repository.Get(productId);

		    if (product != null)
		    {
			    return new ID(product.Guid);
		    }

		    return ID.Null;
	    }

		private ID FindSitecoreIdForCategory(int categoryId)
		{
			var repository = ObjectFactory.Instance.Resolve<IRepository<Category>>();

			var category = repository.Get(categoryId);

			if (category != null)
			{
				return new ID(category.Guid);
			}

			return ID.Null;
		}

		private ID FindSitecoreIdForCatalog(int catalogId)
		{
			var repository = ObjectFactory.Instance.Resolve<IRepository<ProductCatalog>>();

			var catalog = repository.Get(catalogId);

			if (catalog != null)
			{
				return new ID(catalog.Guid);
			}

			return ID.Null;
		}
	}
}
