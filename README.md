# Ucommerce Sitecore Integration #

### Community Developed and maintained integration between Sitecore and Ucommerce ###
Be a part of developing the awesome Ucommerce integration with Sitecore. We accept pull requests.

### Building the package for testing ###

After downloading the repo, building the installation package is really easy. 
Just follow these few simple steps below:

0. Copy the Sitecore assemblies required for the project to compile from your website's bin folder, and place them under a lib/Sitecore folder in the solution folder. 
1. Update the references in the Ucommerce.Sitecore and Ucommerce.Sitecore.Installer projects to use these assemblies.
2. Open a Powershell tool.
3. Navigate the path to the root of the repository.
	Further navigate to "\tools\deploy"
4. Execute the Powershell command: ".\Deploy.To.Package.ps1"
	The package will be generated under "c:\tmp"
5. Install the package through the Sitecore package installer.

### Deploying local changes for debug ###

Once the package is installed you can debug the integration by building the VS solution and then attaching a debugger. 
There's a powershell script that runs after building the solution. To build to the right location, find the file called "Deploy.To.Local.ps1". It can be found under the deploy folder in the visual studio solution.

Modify the first line in the file, where there is a variable called $website_root

$website_root = "C:\inetpub\sc8\Website"

The value needs to match the website root of your Sitecore instance where Ucommerce is installed.

### Troubleshooting ###

- If the projects don't compile after adding the Sitecore assemblies as references, please make sure that the "Target framework" is set to at least the version that the Sitecore assemblies have been compiled against. If this is the issue, you will get a warning in Visual Studio regarding a mismatch of framework versions.

### Commerce Connect ###

Per default the Ucommerce.CommerceConnect project is not loaded. If you want to build a version for Sitecore 8 with Commerce Connect, simply include the project and run the commands above. Then it will automatically be included in the package. 

Then follow the steps from [this article to set up Commerce Connect]("https://docs.ucommerce.net/ucommerce/v7.18/sitecore/Commerce-Connect/Installation.html").

### Who do I talk to? ###

* https://eureka.ucommerce.net