using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using Sitecore;
using Sitecore.Buckets.Extensions;
using Sitecore.Buckets.FieldTypes;
using Sitecore.Buckets.Util;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Query;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources;
using Sitecore.Text;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Logging;
using ScContext = Sitecore.Context;
using Version = Sitecore.Data.Version;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl
{
	/// <summary>
	/// This class is a specialization of the BucketList class from Sitecore.
	/// </summary>
	/// <remarks>
	/// The class has been rewritten, to use a "shallow" item, instead of the full item, when displaying uCommerce items.
	/// Instead of the full item, that would include a very slow call to "GetItemFields()" for all the items,
	/// a simpler item is used. One that only sets the "ItemDefinition" for the item. This is a very fast operation.
	/// 
	/// Since the control only needs the Name of the Item to render, to net effect is to save the calls to "GetItemFields()"
	/// This speeds up the rendering of the control tremendously.
	/// </remarks>
	public class ProductsMultilistWithSearch : BucketList
	{
		new protected int PageNumber { get; set; }
		new protected string Filter { get; set; }
		new protected string TypeHereToSearch { get; set; }
		new protected string Of { get; set; }
		new protected bool EnableSetNewStartLocation { get; set; }

		new private string ScriptParameters
		{
			get
			{
				return string.Format("'{0}'", string.Join("', '", ID, ClientID, PageNumber, "/sitecore/shell/Applications/Buckets/Services/Search.ashx", Filter, GetDatabaseUrlParameter("&"), TypeHereToSearch, Of, EnableSetNewStartLocation));
			}
		}

		protected override Item[] GetItems(Item current)
		{
			Assert.ArgumentNotNull(current, "current");
			NameValueCollection nameValues1 = StringUtil.GetNameValues(Source, '=', '&');
			foreach (string index in nameValues1.AllKeys)
				nameValues1[index] = HttpUtility.JavaScriptStringEncode(nameValues1[index]);

			bool enableSetNewStartLocation;
			if (bool.TryParse(nameValues1["EnableSetNewStartLocation"], out enableSetNewStartLocation))
			{
				EnableSetNewStartLocation = enableSetNewStartLocation;
			}

			string str1 = nameValues1["StartSearchLocation"];
			if (string.IsNullOrEmpty(str1))
				str1 = ItemIDs.RootID.ToString();
	
			string str2 = MakeFilterQueryable(str1);
			if (!IsGuid(str2))
			{
				string paramValue = str2;
				str2 = ItemIDs.RootID.ToString();
				LogSourceQueryError(current, "StartSearchLocation", paramValue, str2);
			}

			string str3 = nameValues1["Filter"];
			if (str3 != null)
			{
				NameValueCollection nameValues2 = StringUtil.GetNameValues(str3, ':', '|');
				if (nameValues2.Count == 0 && str3 != string.Empty)
					Filter = Filter + "&_content=" + str3;
				foreach (string index in nameValues2.Keys)
					Filter = Filter + "&" + index + "=" + nameValues2[index];
			}
			List<SearchStringModel> searchStringModel = string.IsNullOrEmpty(str3) ? new List<SearchStringModel>() : SearchStringModelParseQueryString(str3).ToList();
			searchStringModel.Add(new SearchStringModel("_path", NormalizeGuid(str2).ToLowerInvariant(), "must"));
			ExtractQueryStringAndPopulateModel(nameValues1, searchStringModel, "FullTextQuery", "_content", "_content", false);
			ExtractQueryStringAndPopulateModel(nameValues1, searchStringModel, "Language", "language", "parsedlanguage", false);
			ExtractQueryStringAndPopulateModel(nameValues1, searchStringModel, "SortField", "sort", "sort", false);
			ExtractQueryStringAndPopulateModel(nameValues1, searchStringModel, "TemplateFilter", "template", "template", true);
			string sourceString = nameValues1["PageSize"];
			string str4 = string.IsNullOrEmpty(sourceString) ? "10" : sourceString;
			int result;
			if (!int.TryParse(str4, out result))
			{
				result = 10;
				LogSourceQueryError(current, "PageSize", str4, result.ToString());
			}
			int pageSize = string.IsNullOrEmpty(str4) ? 10 : result;
			Filter = Filter + (object)"location=" + NormalizeGuid(string.IsNullOrEmpty(str2) ? ScContext.ContentDatabase.GetItem(ItemID).GetParentBucketItemOrRootOrSelf().ID.ToString() : str2, true) + "&pageSize=" + pageSize;
			using (IProviderSearchContext searchContext = ContentSearchManager.GetIndex((SitecoreIndexableItem)ScContext.ContentDatabase.GetItem(str2)).CreateSearchContext())
			{
				IQueryable<SitecoreUISearchResultItem> query = LinqHelper.CreateQuery<SitecoreUISearchResultItem>(searchContext, searchStringModel);
				int num = query.Count();
				PageNumber = num % pageSize == 0 ? num / pageSize : num / pageSize + 1;

				var dataProvider = ObjectFactory.Instance.Resolve<ISitecoreContext>().DataProviderMaster;

				return Page(query, 0, pageSize).ToList().Select(x => ConvertUiSearchResultItemToActualItem(x, dataProvider)).Where(i => i != null).ToArray();
			}
		}

		protected override void DoRender(HtmlTextWriter output)
		{
			var loggingService = ObjectFactory.Instance.Resolve<ILoggingService>();
			var stopwatch = new Stopwatch();

			loggingService.Log<ProductsMultilistWithSearch>("Starting ProductsMultilistWithSearch.DoRender() ...");

			stopwatch.Start();

			ArrayList selected;
			OrderedDictionary unselected;
			GetSelectedItems(GetItems(ScContext.ContentDatabase.GetItem(ItemID)), out selected, out unselected);
			var stringBuilder = new StringBuilder();
			foreach (DictionaryEntry dictionaryEntry in unselected)
			{
				var obj = dictionaryEntry.Value as Item;
				if (obj != null)
				{
					stringBuilder.Append(obj.DisplayName + ",");
					stringBuilder.Append(GetItemValue(obj) + ",");
				}
			}
			output.Write("<input type='hidden' width='100%' id='multilistValues{0}' value='{1}' style='width: 200px;margin-left:3px;'>", ClientID, stringBuilder);
			ServerProperties["ID"] = ID;
			string str1 = string.Empty;
			if (ReadOnly)
				str1 = " disabled='disabled'";
			output.Write("<input id='" + ID + "_Value' type='hidden' value='" + StringUtil.EscapeQuote(Value) + "' />");
			output.Write("<table" + GetControlAttributes() + ">");
			output.Write("<tr>");
			output.Write("<td class='scContentControlMultilistCaption' width='50%'>" + Translate.Text("All") + "</td>");
			output.Write("<td width='20'>" + Images.GetSpacer(20, 1) + "</td>");
			output.Write("<td class='scContentControlMultilistCaption' width='50%'>" + Translate.Text("Selected") + "</td>");
			output.Write("<td width='20'>" + Images.GetSpacer(20, 1) + "</td>");
			output.Write("</tr>");
			output.Write("<tr>");
			output.Write("<td valign='top' height='100%'>");
			output.Write(
				"<div style='width:200%;overflow:hidden;height:30px'><input type='text' width='100%' class='scIgnoreModified bucketSearch inactive' value='" +
				TypeHereToSearch + "' id='filterBox" + ClientID + "' " +
				(ScContext.ContentDatabase.GetItem(ItemID).Access.CanWrite() ? string.Empty : "disabled") + ">");
			output.Write("<span id='prev" + ClientID +
			             "' class='hovertext' style='cursor:pointer;' onMouseOver=\"this.style.color='#666'\" onMouseOut=\"this.style.color='#000'\"> <img width='10' height='10' src='/sitecore/shell/Applications/Buckets/images/right.png' style='margin-top: 1px;'> " +
			             Translate.Text("prev") + " |</span>");
			output.Write("<span id='next" + ClientID +
			             "' class='hovertext' style='cursor:pointer;' onMouseOver=\"this.style.color='#666'\" onMouseOut=\"this.style.color='#000'\"> " +
			             Translate.Text("next") +
			             " <img width='10' height='10' src='/sitecore/shell/Applications/Buckets/images/left.png' style='margin-top: 1px;'>  </span>");
			output.Write("<span id='refresh" + ClientID +
			             "' class='hovertext' style='cursor:pointer;' onMouseOver=\"this.style.color='#666'\" onMouseOut=\"this.style.color='#000'\"> " +
			             Translate.Text("refresh") +
			             " <img width='10' height='10' src='/sitecore/shell/Applications/Buckets/images/refresh.png' style='margin-top: 1px;'>  </span>");
			output.Write("<span id='goto" + ClientID +
			             "' class='hovertext' style='cursor:pointer;' onMouseOver=\"this.style.color='#666'\" onMouseOut=\"this.style.color='#000'\"> " +
			             Translate.Text("go to item") +
			             " <img width='10' height='10' src='/sitecore/shell/Applications/Buckets/images/text.png' style='margin-top: 1px;'>  </span>");
			output.Write("<span style='padding-left:34px;'><strong>" + Translate.Text("Page Number") +
			             ": </strong></span><span id='pageNumber" + ClientID + "'></span></div>");
			string str2 = !UIUtil.IsIE() || UIUtil.GetBrowserMajorVersion() != 9 ? "10" : "11";
			output.Write("<select id=\"" + ID +
			             "_unselected\" class=\"scContentControlMultilistBox\" multiple=\"multiple\" size=\"" + str2 + "\"" +
			             str1 + " >");
			foreach (DictionaryEntry dictionaryEntry in unselected)
			{
				var obj = dictionaryEntry.Value as Item;
				if (obj != null)
				{
					var str3 = OutputString(obj);
					output.Write("<option value='" + GetItemValue(obj) + "'>" + str3 + "</option>");
				}
			}
			output.Write("</select>");
			output.Write("</td>");
			output.Write("<td valign='top'>");
			output.Write(
				"<img class='' height='16' width='16' border='0' alt='' style='margin: 2px;' src='/sitecore/shell/themes/standard/Images/blank.png'/>");
			output.Write("<br />");
			RenderButton(output, "Core/16x16/arrow_blue_right.png", string.Empty, "btnRight" + ClientID);
			output.Write("<br />");
			RenderButton(output, "Core/16x16/arrow_blue_left.png", string.Empty, "btnLeft" + ClientID);
			output.Write("</td>");
			output.Write("<td valign='top' height='100%'>");
			output.Write("<select style='margin-top:28px' id='" + ID +
			             "_selected' class='scContentControlMultilistBox' multiple='multiple' size='" + str2 + "'" + str1 + ">");

			var dataProvider = ObjectFactory.Instance.Resolve<ISitecoreContext>().DataProviderMaster;

			for (int index = 0; index < selected.Count; ++index)
			{
				var obj1 = selected[index] as Item;
				if (obj1 != null)
				{
					string str3 = OutputString(obj1);
					output.Write("<option value='" + GetItemValue(obj1) + "'>" + str3 + "</option>");
				}
				else
				{
					var path = selected[index] as string;
					if (path != null)
					{
						var id = new ShortID(path);
						Item obj2 = GetItemFromId(id.ToID(), dataProvider);
						string str3 = obj2 == null ? path + ' ' + Translate.Text("[Item not found]") : OutputString(obj2);
						output.Write("<option value='" + path + "'>" + str3 + "</option>");
					}
				}
			}
			output.Write("</select>");
			output.Write("</td>");
			output.Write("<td valign='top'>");
			output.Write(
				"<img class='' height='16' width='16' border='0' alt='' style='margin: 2px;' src='/sitecore/shell/themes/standard/Images/blank.png'/>");
			output.Write("<br />");
			RenderButton(output, "Core/16x16/arrow_blue_up.png", "javascript:scContent.multilistMoveUp('" + ID + "')",
				"btnUp" + ClientID);
			output.Write("<br />");
			RenderButton(output, "Core/16x16/arrow_blue_down.png",
				"javascript:scContent.multilistMoveDown('" + ID + "')", "btnDown" + ClientID);
			output.Write("</td>");
			output.Write("</tr>");
			output.Write(
				"<div style='border:1px solid #999999;font:8pt tahoma;display:none;padding:2px;margin:4px 0px 4px 0px;height:14px' id='" +
				ID + "_all_help'></div>");
			output.Write(
				"<div style='border:1px solid #999999;font:8pt tahoma;display:none;padding:2px;margin:4px 0px 4px 0px;height:14px' id='" +
				ID + "_selected_help'></div>");
			output.Write("</table>");
			RenderScript(output);

			stopwatch.Stop();
			loggingService.Log<ProductsMultilistWithSearch>(string.Format("ProductsMultilistWithSearch.DoRender() took {0} ms", stopwatch.ElapsedMilliseconds));
		}

		/// <summary>
		/// Returns a shallow item, instead of the full Sitecore item.
		/// </summary>
		/// <remarks>
		/// This returns a complete item, except for all the fields.
		/// Excluding the fields greatly enhances performance.
		/// </remarks>
		protected virtual Item ConvertUiSearchResultItemToActualItem(SitecoreUISearchResultItem searchResultItem, DataProviderMasterDatabase dataProvider)
		{
			var uri = searchResultItem.Uri ?? new ItemUri(searchResultItem["_uniqueid"]);

			var definition = dataProvider.GetItemDefinition(uri.ItemID, null);

			if (definition == null) return null;

			var item = new Item(definition.ID, new ItemData(definition, uri.Language, uri.Version, new FieldList()),
				GetMasterDatabase());

			return item;
		}

		/// <summary>
		/// Returns a shallow item, instead of the full Sitecore item.
		/// </summary>
		/// <remarks>
		/// This returns a complete item, except for all the fields.
		/// Excluding the fields greatly enhances performance.
		/// </remarks>
		protected virtual Item GetItemFromId(ID itemId, DataProviderMasterDatabase dataProvider)
		{
			var definition = dataProvider.GetItemDefinition(itemId, null);

			if (definition == null) return null;

			var item = new Item(definition.ID, new ItemData(definition, Language.Current, Version.First, new FieldList()),
				GetMasterDatabase());

			return item;
		}

		public override string OutputString(Item item)
		{
			var templateName = item.TemplateName;
			return string.Format("{0} ({1} - Products)", item.DisplayName, templateName);
		}

		private static string GetDatabaseUrlParameter(string delimiter)
		{
			if (ScContext.ContentDatabase != null)
			{
				return delimiter + "sc_content=" + ScContext.ContentDatabase.Name;
			}
			
			return string.Empty;
		}
		private static bool IsGuid(string s)
		{
			return new Regex("^[A-Fa-f0-9]{32}$|^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$").Match(s).Success;
		}

		private static string NormalizeGuid(string guid, bool lowercase)
		{
			if (string.IsNullOrEmpty(guid))
				return guid;
			string str = ShortID.Encode(guid);
			if (!lowercase)
				return str;
			
			return str.ToLowerInvariant();
		}

		private static string NormalizeGuid(string guid)
		{
			if (!string.IsNullOrEmpty(guid) && guid.StartsWith("{"))
				return ShortID.Encode(guid);
			return guid;
		}

		private static IQueryable<TSource> Page<TSource>(IQueryable<TSource> source, int page, int pageSize)
		{
			return source.Skip(page * pageSize).Take(pageSize);
		}

		private Database GetMasterDatabase()
		{
			return ScContext.ContentDatabase;
		}

		new protected virtual string MakeFilterQueryable(string locationFilter)
		{
			if (locationFilter != null && locationFilter.StartsWith("query:"))
			{
				locationFilter = locationFilter.Replace("->", "=");
				string query = locationFilter.Substring(6);
				bool flag = query.StartsWith("fast:");
				if (!flag)
					QueryParser.Parse(query);
				locationFilter = (flag ? ScContext.ContentDatabase.GetItem(ItemID).Database.SelectSingleItem(query) : ScContext.ContentDatabase.GetItem(ItemID).Axes.SelectSingleItem(query)).ID.ToString();
			}
			return locationFilter;
		}

		new protected virtual void LogSourceQueryError(Item current, string paramName, string paramValue, string value)
		{
			Assert.ArgumentNotNull(current, "current");
			Log.SingleWarn(string.Format("The '{0}' parameter in the Source field of the '{1}' field in the '{2}' template contains an invalid value: '{3}'. The following value will be used instead: '{4}'.", (object)paramName, (object)current.Fields[FieldID].Name, (object)current.Template.Name, (object)paramValue, (object)value), this);
		}

		new protected virtual void ExtractQueryStringAndPopulateModel(NameValueCollection values,
			List<SearchStringModel> searchStringModel, string parameterName, string filterName, string indexFieldName,
			bool checkForQuery)
		{
			string str1 = values[parameterName] ?? string.Empty;
			if (str1 == string.Empty)
				return;

			string operation = "must";
			string[] strArray = str1.Split(new char[1]
			{
				'|'
			});
			if (strArray.Count() > 1)
				operation = "should";
			foreach (string templateFilter in strArray)
			{
				string str2 = templateFilter;
				if (checkForQuery)
					str2 = this.MakeTemplateFilterQueryable(templateFilter);
				if (parameterName == "SortField")
				{
					if (templateFilter.Contains("[") && templateFilter.Contains("]"))
					{
						operation =
							templateFilter.Substring(templateFilter.IndexOf('[') + 1,
								templateFilter.IndexOf(']') - templateFilter.IndexOf('[') - 1).ToLowerInvariant();
						str2 = str2.Remove(str2.IndexOf('['), str2.IndexOf(']') - str2.IndexOf('[') + 1);
					}
					else
						operation = "asc";
				}
				string str3 = Filter + string.Format("&{0}={1}", filterName, str2);
				Filter = str3;
				searchStringModel.Add(new SearchStringModel(indexFieldName, str2, operation)
				{
					Operation = operation
				});
			}
		}

		new protected virtual void GetSelectedItems(Item[] sources, out ArrayList selected, out OrderedDictionary unselected)
		{
			Assert.ArgumentNotNull(sources, "sources");
			var listString = new ListString(Value);
			unselected = new OrderedDictionary();
			selected = new ArrayList(listString.Count);
			for (int index = 0; index < listString.Count; ++index)
				selected.Add(listString[index]);
			foreach (Item obj in sources)
			{
				string str = obj.ID.ToString();
				int index = listString.IndexOf(str);
				if (index >= 0)
					selected[index] = obj;
				else
					unselected.Add(MainUtil.GetSortKey(obj.Name), obj);
			}
		}

		new protected virtual void RenderButton(HtmlTextWriter output, string icon, string click, string id)
		{
			Assert.ArgumentNotNull(output, "output");
			Assert.ArgumentNotNull(icon, "icon");
			Assert.ArgumentNotNull(click, "click");
			var imageBuilder = new ImageBuilder()
			{
				Src = icon,
				Width = 16,
				Height = 16,
				Margin = "2px",
				ID = id
			};
			if (!ReadOnly)
				imageBuilder.OnClick = click;
			output.Write(imageBuilder.ToString());
		}

		new protected virtual void RenderScript(HtmlTextWriter output)
		{
			string str = "<script type='text/javascript'>\r\n                                    (function() {\r\n                                        if (!document.getElementById('BucketListJs')) {\r\n                                            var head = document.getElementsByTagName('head')[0];\r\n                                            head.appendChild(new Element('script', { type: 'text/javascript', src: '/sitecore/shell/Controls/BucketList/BucketList.js', id: 'BucketListJs' }));\r\n                                            head.appendChild(new Element('link', { rel: 'stylesheet', href: '/sitecore/shell/Controls/BucketList/BucketList.css' }));\r\n                                        }\r\n                                        var stopAt = Date.now() + 5000;\r\n                                        var timeoutId = setTimeout(function() {\r\n                                            if (Sitecore.InitBucketList) {\r\n                                                Sitecore.InitBucketList(" + this.ScriptParameters + ");\r\n                                                clearTimeout(timeoutId);\r\n                                            } else if (Date.now() > stopAt) {\r\n                                                clearTimeout(timeoutId);\r\n                                            }\r\n                                        }, 100);\r\n                                    }());\r\n                              </script>";
			output.Write(str);
		}

		/// <summary>
		/// Parses a query string into a number of SearchStringModels
		/// </summary>
		/// <remarks>
		/// This method has been reconstructed from a static method in Sitecore.
		/// 
		/// It was located in a class called: Sitecore.Buckets.Util.UIFilterHelpers in version 7.0
		/// It is located in: Sitecore.ContentSearch.Utilities.SearchStringModel in version 7.5
		/// 
		/// So in order to be able to work with both sets of assemblies, we need to have the implementation here.
		/// </remarks>
		private static IEnumerable<SearchStringModel> SearchStringModelParseQueryString(string queryString)
		{
			var list = new List<SearchStringModel>();
			string str1 = queryString;
			foreach (string str2 in str1.Split(new[] { '|' }))
			{
				string[] strArray = str2.Split(new[] { ':' });
				if (strArray.Length > 1)
				{
					string str3 = strArray[0];
					string str4 = strArray[1];
					if (str3.StartsWith("+"))
						list.Add(new SearchStringModel
						{
							Operation = "must",
							Type = str3.Replace("+", "").Replace("-", ""),
							Value = str4
						});
					else if (str3.StartsWith("-"))
						list.Add(new SearchStringModel
						{
							Operation = "not",
							Type = str3.Replace("+", "").Replace("-", ""),
							Value = str4
						});
					else
						list.Add(new SearchStringModel
						{
							Operation = "should",
							Type = str3.Replace("+", "").Replace("-", ""),
							Value = str4
						});
				}
				else
					list.Add(new SearchStringModel
					{
						Operation = "must",
						Type = "_content",
						Value = strArray[0]
					});
			}
			return list;
		}
	}
}
