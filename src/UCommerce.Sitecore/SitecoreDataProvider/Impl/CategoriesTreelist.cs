using System.Diagnostics;
using System.Linq;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Text;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;
using System;
using System.Collections;
using System.ComponentModel;
using System.Web.UI;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Logging;
using Control = Sitecore.Web.UI.HtmlControls.Control;
using ScContext = Sitecore.Context;
using Version = Sitecore.Data.Version;
using ScID = Sitecore.Data.ID;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl
{
	/// <summary>
	/// This class is has been reconstructed from the TreeList class in Sitecore.
	/// </summary>
	/// <remarks>
	/// The class has been rewritten, to use a "shallow" item, instead of the full item, when displaying uCommerce items.
	/// Instead of the full item, that would include a very slow call to "GetItemFields()" for all the items,
	/// a simpler item is used. One that only sets the "ItemDefinition" for the item. This is a very fast operation.
	/// 
	/// Since the control only needs the Name of the Item to render, to net effect is to save the calls to "GetItemFields()"
	/// This speeds up the rendering of the control tremendously.
	/// </remarks>
	public class CategoriesTreelist : Control, IContentField
	{
		private string _itemID;
		private Listbox _listBox;
		private string _source;

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="T:Sitecore.Shell.Applications.ContentEditor.TreeList"/> allows the multiple selection.
		/// 
		/// </summary>
		/// 
		/// <value>
		/// <c>true</c> if the <see cref="T:Sitecore.Shell.Applications.ContentEditor.TreeList"/> allows the  multiple selection; otherwise, <c>false</c>.
		/// 
		/// </value>
		[Category("Data")]
		[Description("If set to Yes, allows the same item to be selected more than once")]
		public bool AllowMultipleSelection
		{
			get { return GetViewStateBool("AllowMultipleSelection"); }
			set { SetViewStateBool("AllowMultipleSelection", value); }
		}

		/// <summary>
		/// Gets the data context parameters.
		/// 
		/// </summary>
		/// 
		/// <value>
		/// The data context parameters.
		/// </value>
		/// <contract><requires name="value" condition="not null"/><ensures condition="not null"/></contract>
		public string DatabaseName
		{
			get { return GetViewStateString("DatabaseName"); }
			set
			{
				Assert.ArgumentNotNull(value, "value");
				SetViewStateString("DatabaseName", value);
			}
		}

		/// <summary>
		/// Gets or sets field that will be used as source for ListItem header. If empty- DisplayName will be used.
		/// 
		/// </summary>
		public string DisplayFieldName
		{
			get { return GetViewStateString("DisplayFieldName"); }
			set
			{
				Assert.ArgumentNotNull(value, "value");
				SetViewStateString("DisplayFieldName", value);
			}
		}

		/// <summary>
		/// Gets or sets the exclude items for display.
		/// 
		/// </summary>
		/// 
		/// <value>
		/// The exclude items for display.
		/// </value>
		/// <contract><requires name="value" condition="not null"/><ensures condition="not null"/></contract>
		[Category("Data")]
		[Description("Comma separated list of item names/ids.")]
		public string ExcludeItemsForDisplay
		{
			get { return GetViewStateString("ExcludeItemsForDisplay"); }
			set
			{
				Assert.ArgumentNotNull(value, "value");
				SetViewStateString("ExcludeItemsForDisplay", value);
			}
		}

		/// <summary>
		/// Gets or sets the exclude templates for display.
		/// 
		/// </summary>
		/// 
		/// <value>
		/// The exclude templates for display.
		/// </value>
		/// <contract><requires name="value" condition="not null"/><ensures condition="not null"/></contract>
		[Description(
			"Comma separated list of template names. If this value is set, items based on these template will not be displayed in the tree."
			)]
		[Category("Data")]
		public string ExcludeTemplatesForDisplay
		{
			get { return GetViewStateString("ExcludeTemplatesForDisplay"); }
			set
			{
				Assert.ArgumentNotNull(value, "value");
				SetViewStateString("ExcludeTemplatesForDisplay", value);
			}
		}

		/// <summary>
		/// Gets or sets the exclude templates for selection.
		/// 
		/// </summary>
		/// 
		/// <value>
		/// The exclude templates for selection.
		/// </value>
		/// <contract><requires name="value" condition="not null"/><ensures condition="not null"/></contract>
		[Category("Data")]
		[Description(
			"Comma separated list of template names. If this value is set, items based on these template will not be included in the menu."
			)]
		public string ExcludeTemplatesForSelection
		{
			get { return GetViewStateString("ExcludeTemplatesForSelection"); }
			set
			{
				Assert.ArgumentNotNull(value, "value");
				SetViewStateString("ExcludeTemplatesForSelection", value);
			}
		}

		/// <summary>
		/// Gets or sets the include items for display.
		/// 
		/// </summary>
		/// 
		/// <value>
		/// The include items for display.
		/// </value>
		/// <contract><requires name="value" condition="not null"/><ensures condition="not null"/></contract>
		[Description("Comma separated list of items names/ids.")]
		[Category("Data")]
		public string IncludeItemsForDisplay
		{
			get { return GetViewStateString("IncludeItemsForDisplay"); }
			set
			{
				Assert.ArgumentNotNull(value, "value");
				SetViewStateString("IncludeItemsForDisplay", value);
			}
		}

		/// <summary>
		/// Gets or sets the include templates for display.
		/// 
		/// </summary>
		/// 
		/// <value>
		/// The include templates for display.
		/// </value>
		/// <contract><requires name="value" condition="not null"/><ensures condition="not null"/></contract>
		[Description(
			"Comma separated list of template names. If this value is set, only items based on these template can be displayed in the menu."
			)]
		[Category("Data")]
		public string IncludeTemplatesForDisplay
		{
			get { return GetViewStateString("IncludeTemplatesForDisplay"); }
			set
			{
				Assert.ArgumentNotNull(value, "value");
				SetViewStateString("IncludeTemplatesForDisplay", value);
			}
		}

		/// <summary>
		/// Gets or sets the include templates for selection.
		/// 
		/// </summary>
		/// 
		/// <value>
		/// The include templates for selection.
		/// </value>
		/// <contract><requires name="value" condition="not null"/><ensures condition="not null"/></contract>
		[Category("Data")]
		[Description(
			"Comma separated list of template names. If this value is set, only items based on these template can be included in the menu."
			)]
		public string IncludeTemplatesForSelection
		{
			get { return GetViewStateString("IncludeTemplatesForSelection"); }
			set
			{
				Assert.ArgumentNotNull(value, "value");
				SetViewStateString("IncludeTemplatesForSelection", value);
			}
		}

		/// <summary>
		/// Gets or sets the item ID.
		/// 
		/// </summary>
		/// 
		/// <value>
		/// The item ID.
		/// </value>
		/// <contract><requires name="value" condition="not null"/><ensures condition="nullable"/></contract>
		public string ItemID
		{
			get { return _itemID; }
			set
			{
				Assert.ArgumentNotNull(value, "value");
				_itemID = value;
			}
		}

		/// <summary>
		/// Gets or sets the item language.
		/// 
		/// </summary>
		/// 
		/// <value>
		/// The item language.
		/// </value>
		public string ItemLanguage
		{
			get { return GetViewStateString("ItemLanguage"); }
			set
			{
				Assert.ArgumentNotNull(value, "value");
				SetViewStateString("ItemLanguage", value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="T:Sitecore.Shell.Applications.ContentEditor.TreeList"/> is read-only.
		/// 
		/// </summary>
		/// 
		/// <value>
		/// <c>true</c> if the <see cref="T:Sitecore.Shell.Applications.ContentEditor.TreeList"/> is read-only; otherwise, <c>false</c>.
		/// 
		/// </value>
		public bool ReadOnly { get; set; }

		/// <summary>
		/// Gets or sets the source.
		/// 
		/// </summary>
		/// 
		/// <value>
		/// The source.
		/// </value>
		/// <contract><requires name="value" condition="not null"/><ensures condition="nullable"/></contract>
		public string Source
		{
			get { return _source; }
			set
			{
				Assert.ArgumentNotNull(value, "value");
				_source = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sitecore.Shell.Applications.ContentEditor.TreeList"/> class.
		/// 
		/// </summary>
		public CategoriesTreelist()
		{
			Class = "scContentControl scContentControlTreelist";
			Background = "white";
			Activation = true;
			ReadOnly = false;
		}

		/// <summary>
		/// Raises the load event.
		/// 
		/// </summary>
		/// <param name="args">The arguments.</param><contract><requires name="args" condition="not null"/></contract>
		protected override void OnLoad(EventArgs args)
		{
			var loggingService = ObjectFactory.Instance.Resolve<ILoggingService>();
			var stopwatch = new Stopwatch();

			loggingService.Log<CategoriesTreelist>("Starting CategoriesTreelist.OnLoad() ...");

			stopwatch.Start();

			Assert.ArgumentNotNull(args, "args");
			if (!ScContext.ClientPage.IsEvent)
			{
				SetProperties();
				var border1 = new Border();
				Controls.Add(border1);
				GetControlAttributes();
				foreach (string key in Attributes.Keys)
					border1.Attributes.Add(key, Attributes[key]);
				border1.Attributes["id"] = ID;
				var border2 = new Border {Class = "scTreeListHalfPart"};
				var border3 = border2;
				border1.Controls.Add(border3);
				var border4 = new Border();
				border3.Controls.Add(border4);
				SetViewStateString("ID", ID);
				var controls1 = border4.Controls;
				var literal1 = new Literal("All") {Class = "scContentControlMultilistCaption"};
				var literal2 = literal1;
				controls1.Add(literal2);
				var scrollbox1 = new Scrollbox
				{
					ID = GetUniqueID("S"),
					Class = "scScrollbox scContentControlTree"
				};
				var scrollbox2 = scrollbox1;
				border4.Controls.Add(scrollbox2);
				var treeviewEx = new TreeviewEx {ID = ID + "_all", DblClick = ID + ".Add", AllowDragging = false};
				scrollbox2.Controls.Add(treeviewEx);
				var border5 = new Border {Class = "scContentControlNavigation"};
				var border6 = border5;
				border3.Controls.Add(border6);
				var literalControl1 = new LiteralControl(new ImageBuilder
				{
					Src = "Applications/16x16/nav_right_blue.png",
					ID = (ID + "_right"),
					OnClick = ScContext.ClientPage.GetClientEvent(ID + ".Add")
				}.ToString() + new ImageBuilder
				{
					Src = "Applications/16x16/nav_left_blue.png",
					ID = (ID + "_left"),
					OnClick = ScContext.ClientPage.GetClientEvent(ID + ".Remove")
				});
				border6.Controls.Add(literalControl1);
				var border7 = new Border {Class = "scTreeListHalfPart"};
				var border8 = border7;
				border1.Controls.Add(border8);
				var border9 = new Border {Class = "scFlexColumnContainerWithoutFlexie"};
				var border10 = border9;
				border8.Controls.Add(border10);
				var controls2 = border10.Controls;
				var literal3 = new Literal("Selected") {Class = "scContentControlMultilistCaption"};
				var literal4 = literal3;
				controls2.Add(literal4);
				var border11 = new Border {Class = "scContentControlSelectedList"};
				var border12 = border11;
				border10.Controls.Add(border12);
				var listbox = new Listbox();
				border12.Controls.Add(listbox);
				_listBox = listbox;
				listbox.ID = ID + "_selected";
				listbox.DblClick = ID + ".Remove";
				listbox.Style["width"] = "100%";
				listbox.Size = "10";
				listbox.Attributes["onchange"] = "javascript:document.getElementById('" + ID +
				                                 "_help').innerHTML=this.selectedIndex>=0?this.options[this.selectedIndex].innerHTML:''";
				listbox.Attributes["class"] = "scContentControlMultilistBox scFlexContentWithoutFlexie";
				_listBox.TrackModified = false;
				treeviewEx.Enabled = !ReadOnly;
				listbox.Disabled = ReadOnly;
				border10.Controls.Add(
					new LiteralControl("<div class='scContentControlTreeListHelp' id=\"" + ID + "_help\"></div>"));
				var border13 = new Border {Class = "scContentControlNavigation"};
				var border14 = border13;
				border8.Controls.Add(border14);
				var literalControl2 = new LiteralControl(new ImageBuilder
				{
					Src = "Applications/16x16/nav_up_blue.png",
					ID = (ID + "_up"),
					OnClick = ScContext.ClientPage.GetClientEvent(ID + ".Up")
				}.ToString() + new ImageBuilder
				{
					Src = "Applications/16x16/nav_down_blue.png",
					ID = (ID + "_down"),
					OnClick = ScContext.ClientPage.GetClientEvent(ID + ".Down")
				});
				border14.Controls.Add(literalControl2);
				var dataContext = new DataContext();
				border1.Controls.Add(dataContext);
				dataContext.ID = GetUniqueID("D");
				dataContext.Filter = FormTemplateFilterForDisplay();
				treeviewEx.DataContext = dataContext.ID;
				treeviewEx.DisplayFieldName = DisplayFieldName;
				dataContext.DataViewName = "Master";
				if (!string.IsNullOrEmpty(DatabaseName))
					dataContext.Parameters = "databasename=" + DatabaseName;
				dataContext.Root = DataSource;
				dataContext.Language = Language.Parse(ItemLanguage);
				treeviewEx.ShowRoot = true;
				RestoreState();
			}
			base.OnLoad(args);

			stopwatch.Stop();
			loggingService.Log<CategoriesTreelist>(string.Format("CategoriesTreelist.OnLoad() took {0} ms", stopwatch.ElapsedMilliseconds));
		}

		/// <summary>
		/// Adds data.
		/// 
		/// </summary>
		protected void Add()
		{
			if (Disabled)
				return;
			var viewStateString = GetViewStateString("ID");
			var treeviewEx = FindControl(viewStateString + "_all") as TreeviewEx;
			Assert.IsNotNull(treeviewEx, typeof (DataTreeview));
			var listbox = FindControl(viewStateString + "_selected") as Listbox;
			Assert.IsNotNull(listbox, typeof (Listbox));
			var selectionItem = treeviewEx.GetSelectionItem(Language.Parse(ItemLanguage), Version.Latest);
			if (selectionItem == null)
			{
				SheerResponse.Alert("Select an item in the Content Tree.", new string[0]);
			}
			else
			{
				if (HasExcludeTemplateForSelection(selectionItem))
					return;
				if (IsDeniedMultipleSelection(selectionItem, listbox))
				{
					SheerResponse.Alert("You cannot select the same item twice.", new string[0]);
				}
				else
				{
					if (!HasIncludeTemplateForSelection(selectionItem))
						return;
					SheerResponse.Eval("scForm.browser.getControl('" + viewStateString + "_selected').selectedIndex=-1");
					var listItem = new ListItem {ID = GetUniqueID("L")};
					ScContext.ClientPage.AddControl(listbox, listItem);
					listItem.Header = GetHeaderValue(selectionItem);
					listItem.Value = listItem.ID + "|" + selectionItem.ID;
					SheerResponse.Refresh(listbox);
					SetModified();
				}
			}
		}

		/// <summary>
		/// Executes the Down event.
		/// 
		/// </summary>
		protected void Down()
		{
			if (Disabled)
				return;
			var listbox = FindControl(GetViewStateString("ID") + "_selected") as Listbox;
			Assert.IsNotNull(listbox, typeof (Listbox));
			var num = -1;
			for (var index = listbox.Controls.Count - 1; index >= 0; --index)
			{
				var listItem = listbox.Controls[index] as ListItem;
				Assert.IsNotNull(listItem, typeof (ListItem));
				if (!listItem.Selected)
				{
					num = index - 1;
					break;
				}
			}
			for (var index = num; index >= 0; --index)
			{
				var listItem = listbox.Controls[index] as ListItem;
				Assert.IsNotNull(listItem, typeof (ListItem));
				if (listItem.Selected)
				{
					SheerResponse.Eval("scForm.browser.swapNode(scForm.browser.getControl('" + listItem.ID +
					                   "'), scForm.browser.getControl('" + listItem.ID + "').nextSibling);");
					listbox.Controls.Remove(listItem);
					listbox.Controls.AddAt(index + 1, listItem);
				}
			}
			SetModified();
		}

		/// <summary>
		/// Gets the header value.
		/// 
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>
		/// Header text for list item.
		/// </returns>
		protected virtual string GetHeaderValue(Item item)
		{
			Assert.ArgumentNotNull(item, "item");
			var str = string.IsNullOrEmpty(DisplayFieldName) ? item.DisplayName : item[DisplayFieldName];
			return string.IsNullOrEmpty(str) ? item.DisplayName : str;
		}

		/// <summary>
		/// Removes the selected item.
		/// 
		/// </summary>
		protected void Remove()
		{
			if (Disabled)
				return;
			var viewStateString = GetViewStateString("ID");
			var listbox = FindControl(viewStateString + "_selected") as Listbox;
			Assert.IsNotNull(listbox, typeof (Listbox));
			SheerResponse.Eval("scForm.browser.getControl('" + viewStateString + "_all').selectedIndex=-1");
			SheerResponse.Eval("scForm.browser.getControl('" + viewStateString + "_help').innerHTML=''");
			foreach (var listItem in listbox.Selected)
			{
				SheerResponse.Remove(listItem.ID);
				listbox.Controls.Remove(listItem);
			}
			SheerResponse.Refresh(listbox);
			SetModified();
		}

		/// <summary>
		/// Moves the selected items up.
		/// 
		/// </summary>
		protected void Up()
		{
			if (Disabled)
				return;
			var listbox = FindControl(GetViewStateString("ID") + "_selected") as Listbox;
			Assert.IsNotNull(listbox, typeof (Listbox));
			var selectedItem = listbox.SelectedItem;
			if (selectedItem == null)
				return;
			var num = listbox.Controls.IndexOf(selectedItem);
			if (num == 0)
				return;
			SheerResponse.Eval("scForm.browser.swapNode(scForm.browser.getControl('" + selectedItem.ID +
			                   "'), scForm.browser.getControl('" + selectedItem.ID + "').previousSibling);");
			listbox.Controls.Remove(selectedItem);
			listbox.Controls.AddAt(num - 1, selectedItem);
			SetModified();
		}

		/// <summary>
		/// Gets the value.
		/// 
		/// </summary>
		/// 
		/// <returns>
		/// The value of the field.
		/// </returns>
		/// <contract><ensures condition="not null"/></contract>
		public string GetValue()
		{
			var listString = new ListString();
			var listbox = FindControl(GetViewStateString("ID") + "_selected") as Listbox;
			Assert.IsNotNull(listbox, typeof (Listbox));
			foreach (ListItem control in listbox.Items)
			{
				string[] strArray = control.Value.Split(new char[1]
				{
					'|'
				});
				if (strArray.Length > 1)
					listString.Add(strArray[1]);
			}
			return listString.ToString();
		}

		/// <summary>
		/// Sets the value.
		/// 
		/// </summary>
		/// <param name="text">The text.</param><contract><requires name="text" condition="not null"/></contract>
		public void SetValue(string text)
		{
			Assert.ArgumentNotNull(text, "text");
			Value = text;
		}

		/// <summary>
		/// Sets the modified.
		/// 
		/// </summary>
		protected static void SetModified()
		{
			ScContext.ClientPage.Modified = true;
		}

		/// <summary>
		/// Can be used after <c>OnLoad()</c> is called.
		///             Fulfills parsing Of Value and restores <c>Listbox</c> state
		/// 
		/// </summary>
		/// 
		/// <returns/>
		/// <contract><ensures condition="not null"/></contract>
		private string FormTemplateFilterForDisplay()
		{
			if (string.IsNullOrEmpty(IncludeTemplatesForDisplay) && string.IsNullOrEmpty(ExcludeTemplatesForDisplay) &&
			    (string.IsNullOrEmpty(IncludeItemsForDisplay) && string.IsNullOrEmpty(ExcludeItemsForDisplay)))
				return string.Empty;
			var str1 = string.Empty;
			var str2 = ("," + IncludeTemplatesForDisplay + ",").ToLowerInvariant();
			var str3 = ("," + ExcludeTemplatesForDisplay + ",").ToLowerInvariant();
			var str4 = "," + IncludeItemsForDisplay + ",";
			var str5 = "," + ExcludeItemsForDisplay + ",";
			if (!string.IsNullOrEmpty(IncludeTemplatesForDisplay))
			{
				if (!string.IsNullOrEmpty(str1))
					str1 = str1 + " and ";
				str1 = str1 +
				       string.Format("(contains('{0}', ',' + @@templateid + ',') or contains('{0}', ',' + @@templatekey + ','))",
					       str2);
			}
			if (!string.IsNullOrEmpty(ExcludeTemplatesForDisplay))
			{
				if (!string.IsNullOrEmpty(str1))
					str1 = str1 + " and ";
				str1 = str1 +
				       string.Format(
					       "not (contains('{0}', ',' + @@templateid + ',') or contains('{0}', ',' + @@templatekey + ','))",
					       str3);
			}
			if (!string.IsNullOrEmpty(IncludeItemsForDisplay))
			{
				if (!string.IsNullOrEmpty(str1))
					str1 = str1 + " and ";
				str1 = str1 +
				       string.Format("(contains('{0}', ',' + @@id + ',') or contains('{0}', ',' + @@key + ','))", str4);
			}
			if (!string.IsNullOrEmpty(ExcludeItemsForDisplay))
			{
				if (!string.IsNullOrEmpty(str1))
					str1 = str1 + " and ";
				str1 = str1 +
				       string.Format("not (contains('{0}', ',' + @@id + ',') or contains('{0}', ',' + @@key + ','))", str5);
			}
			return str1;
		}

		/// <summary>
		/// Determines whether [has exclude template for selection] [the specified item].
		/// 
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>
		/// <c>true</c> if [has exclude template for selection] [the specified item]; otherwise, <c>false</c>.
		/// 
		/// </returns>
		/// <contract><requires name="item" condition="none"/></contract>
		private bool HasExcludeTemplateForSelection(Item item)
		{
			return item == null || HasItemTemplate(item, ExcludeTemplatesForSelection);
		}

		/// <summary>
		/// Determines whether [has include template for selection] [the specified item].
		/// 
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>
		/// <c>true</c> if [has include template for selection] [the specified item]; otherwise, <c>false</c>.
		/// 
		/// </returns>
		/// <contract><requires name="item" condition="not null"/></contract>
		private bool HasIncludeTemplateForSelection(Item item)
		{
			Assert.ArgumentNotNull(item, "item");
			return IncludeTemplatesForSelection.Length == 0 || HasItemTemplate(item, IncludeTemplatesForSelection);
		}

		/// <summary>
		/// Determines whether [has item template] [the specified item].
		/// 
		/// </summary>
		/// <param name="item">The item.</param><param name="templateList">The template list.</param>
		/// <returns>
		/// <c>true</c> if [has item template] [the specified item]; otherwise, <c>false</c>.
		/// 
		/// </returns>
		/// <contract><requires name="item" condition="none"/><requires name="templateList" condition="not null"/></contract>
		private static bool HasItemTemplate(Item item, string templateList)
		{
			Assert.ArgumentNotNull(templateList, "templateList");
			if (item == null || templateList.Length == 0)
				return false;
			var strArray = templateList.Split(new[]
			{
				','
			});
			var arrayList = new ArrayList(strArray.Length);
			foreach (var templateName in strArray)
				arrayList.Add(templateName.Trim().ToLowerInvariant());
			return arrayList.Contains(item.TemplateName.Trim().ToLowerInvariant());
		}

		/// <summary>
		/// Determines whether this instance denies multiple selection.
		/// 
		/// </summary>
		/// <param name="item">The item.</param><param name="listbox">The <c>listbox</c>.</param>
		/// <returns>
		/// <c>true</c> if this instance denies multiple selection; otherwise, <c>false</c>.
		/// 
		/// </returns>
		private bool IsDeniedMultipleSelection(Item item, Listbox listbox)
		{
			Assert.ArgumentNotNull(listbox, "listbox");
			if (item == null)
				return true;
			if (AllowMultipleSelection)
				return false;
			return (from Control control in listbox.Controls
				select control.Value.Split(new[]
				{
					'|'
				})).Any(strArray => strArray.Length >= 2 && strArray[1] == item.ID.ToString());
		}

		/// <summary>
		/// Restores the state.
		/// 
		/// </summary>
		private void RestoreState()
		{
			var dataProvider = ObjectFactory.Instance.Resolve<ISitecoreContext>().DataProviderMaster;

			var strArray = Value.Split(new[] { '|' } );
			if (strArray.Length <= 0)
				return;
			var database = ScContext.ContentDatabase;
			if (!string.IsNullOrEmpty(DatabaseName))
				database = Factory.GetDatabase(DatabaseName);
			foreach (var path in strArray)
			{
				if (!string.IsNullOrEmpty(path))
				{
					var listItem = new ListItem {ID = GetUniqueID("I")};
					_listBox.Controls.Add(listItem);
					listItem.Value = listItem.ID + "|" + path;
					var obj = GetItemFromDatabase(database, path, dataProvider);
					listItem.Header = obj == null ? path + ' ' + Translate.Text("[Item not found]") : GetHeaderValue(obj);
				}
			}
			SheerResponse.Refresh(_listBox);
		}

		/// <summary>
		/// Returns a shallow item, instead of the full Sitecore item.
		/// </summary>
		/// <remarks>
		/// This returns a complete item, except for all the fields.
		/// Excluding the fields greatly enhances performance.
		/// </remarks>
		private Item GetItemFromDatabase(Database database, string path, DataProviderMasterDatabase dataProvider)
		{
			var definition = dataProvider.GetItemDefinition(new ID(path), null);

			if (definition == null) return null;

			var item = new Item(definition.ID, new ItemData(definition, Language.Current, Version.First, new FieldList()),
				database);

			return item;
		}

		/// <summary>
		/// Sets the properties.
		/// 
		/// </summary>
		private void SetProperties()
		{
			var @string = StringUtil.GetString(new string[1]
			{
				Source
			});
			if (ScID.IsID(@string))
				DataSource = Source;
			else if (Source != null && !@string.Trim().StartsWith("/", StringComparison.OrdinalIgnoreCase))
			{
				ExcludeTemplatesForSelection = StringUtil.ExtractParameter("ExcludeTemplatesForSelection", Source).Trim();
				IncludeTemplatesForSelection = StringUtil.ExtractParameter("IncludeTemplatesForSelection", Source).Trim();
				IncludeTemplatesForDisplay = StringUtil.ExtractParameter("IncludeTemplatesForDisplay", Source).Trim();
				ExcludeTemplatesForDisplay = StringUtil.ExtractParameter("ExcludeTemplatesForDisplay", Source).Trim();
				ExcludeItemsForDisplay = StringUtil.ExtractParameter("ExcludeItemsForDisplay", Source).Trim();
				IncludeItemsForDisplay = StringUtil.ExtractParameter("IncludeItemsForDisplay", Source).Trim();
				AllowMultipleSelection =
					string.Compare(StringUtil.ExtractParameter("AllowMultipleSelection", Source).Trim().ToLowerInvariant(), "yes",
						StringComparison.InvariantCultureIgnoreCase) == 0;
				DataSource = StringUtil.ExtractParameter("DataSource", Source).Trim().ToLowerInvariant();
				DatabaseName = StringUtil.ExtractParameter("databasename", Source).Trim().ToLowerInvariant();
			}
			else
				DataSource = Source;
		}
	}
}
