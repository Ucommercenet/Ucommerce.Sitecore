---Update Sitecore Items for Speak Applications---

After editting and updating the items in the Sitecore content tree for the Core database. You will need to export those Sitecore items by using Unicorn. 

Export items from Sitecore: 
# Update the predicate section in Serialization.config to point at that part of the tree you want to export items from. By default: 
		<include database="core" path="/sitecore/templates/uCommerce"/>
		<include database="core" path="/sitecore/layout/Renderings/uCommerce Speak Frame"/>
		<include database="core" path="/sitecore/client/uCommerce Applications"/>
		<include database="core" path="/sitecore/client/Applications/Launchpad/PageSettings/Buttons/Commerce"/>

1. Copy Unicorn.dll, Ninject.dll and Kamsar.WebConsole.dll to your main project in whatever fashion you wish (project reference, as binary references, etc)
2. Copy Serialization.config to your App_Config\Include folder
3. Hit $yoursite/unicorn.aspx to perform initial serialization of your configured predicate
4. Items is places in \~\App_Data\serialization\core
5. Update the modified items in the uCommerce solution and create a new uCommerce-Sitecore package. 
---Update Sitecore Items for Speak Applications---

After editting and updating the items in the Sitecore content tree for the Core database. You will need to export those Sitecore items by using Unicorn. 

