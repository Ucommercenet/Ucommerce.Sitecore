using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using Sitecore;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Xml;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure;
using Ucommerce.Search;
using Match = System.Text.RegularExpressions.Match;

namespace Ucommerce.Sitecore.Pipelines
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
		            id = FindSitecoreIdForProduct();
                    if (id == ID.Null) break;

                    Context.Item = Context.Database.GetItem(id);
                    break;
                case SitecoreConstants.RewriteProduct:
		            id = FindSitecoreIdForProduct();
                    if (id == ID.Null) break;

                    Context.Item = Context.Database.GetItem(id);
                    break;
                case SitecoreConstants.RewriteCategory:
		            id = FindSitecoreIdForCategory();
                    if (id == ID.Null) break;

                    Context.Item = Context.Database.GetItem(id);
                    break;
                case SitecoreConstants.RewriteCatalog:
		            id = FindSitecoreIdForCatalog();
                    if (id == ID.Null) break;

					Context.Item = Context.Database.GetItem(id);
                    break;
            }
        }

        private struct RewriteRule
        {
            public string RuleFor;
            public string RuleMatch;
        }

	    private ID FindSitecoreIdForProduct()
	    {
		    var product = ObjectFactory.Instance.Resolve<ICatalogContext>().CurrentProduct;

		    if (product != null)
		    {
			    return new ID(product.Guid);
		    }

		    return ID.Null;
	    }

		private ID FindSitecoreIdForCategory()
		{
			var category = ObjectFactory.Instance.Resolve<ICatalogContext>().CurrentCategory;

			if (category != null)
			{
				return new ID(category.Guid);
			}

			return ID.Null;
		}

		private ID FindSitecoreIdForCatalog()
		{
			var catalog = ObjectFactory.Instance.Resolve<ICatalogContext>().CurrentCatalog;

			if (catalog != null)
			{
				return new ID(catalog.Guid);
			}

			return ID.Null;
		}
	}
}
