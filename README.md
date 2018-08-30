# Ucommerce Sitecore Integration #

### Community Developed and maintained integration between Sitecore and Ucommerce ###
Be a part of developing the awesome Ucommerce integration with Sitecore. We accept pull requests.

### Building the package for testing ###

After downloading the repo, building the installation package is really easy. 
Just follow theese few simple steps below:

1. Open a power shell tool.
2. Navigate the path to the repo root.
	Further navigate to "\tools\deploy"
3. Execute the powershell command: ".\Deploy.To.Package.ps1"
	The package will be located under "c:\tmp"
4. Install the package through the sitecore package installer UI.

### Commerce Connect ###

Per default the Ucommerce.CommerceConnect project is not loaded. If you want to build a version for Sitecore 8 with commerce connect, simply include the project and run the commands above. Then it will automatically be included in the package. 

### Who do I talk to? ###

* https://eureka.ucommerce.net