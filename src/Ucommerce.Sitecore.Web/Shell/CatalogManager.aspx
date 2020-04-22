<%@ Page Title="" Language="C#" MasterPageFile="Masterpages/MasterPageShell.Master" AutoEventWireup="true" CodeBehind="CatalogManager.aspx.cs" 
	Inherits="Ucommerce.Sitecore.Web.Shell.CatalogManager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlacerHolder" runat="server">
	<ucommerce-shell content-picker-type="CatalogApp" tree-indetion-size="31" fixed-left-size="300px" disable-resize="true" start-page="../Catalog/StoresStartPage.aspx?IsSpeak=true" stylesheet="<%= string.Equals(Request.QueryString["IsSpeak"], bool.TrueString, StringComparison.OrdinalIgnoreCase) ? "/sitecore modules/Shell/ucommerce/css/Speak/ucommerce-speak.css" : string.Empty %>" script="<%= string.Equals(Request.QueryString["IsSpeak"], bool.TrueString, StringComparison.OrdinalIgnoreCase) ? "/sitecore modules/Shell/ucommerce/scripts/speak.js" : string.Empty %>"></ucommerce-shell>        
</asp:Content>
