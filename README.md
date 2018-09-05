# Ucommerce Sitecore Integration #

### Community Developed and maintained integration between Sitecore and Ucommerce ###
Be a part of developing the awesome Ucommerce integration with Sitecore. We accept pull requests.

### Building the package for testing ###

After downloading the repo, building the installation package is really easy. 
Just follow these few simple steps below:

1. Open a Powershell tool.
2. Navigate the path to the root of the repository.
	Further navigate to "\tools\deploy"
3. Execute the Powershell command: ".\Deploy.To.Package.ps1"
	The package will be generated under "c:\tmp"
4. Install the package through the Sitecore package installer.

### Deploying local changes for debug ###

Once the package is installed you can debug the integration by building the VS solution and then attaching a debugger. 
There's a powershell script that runs after building the solution. To build to the right location, find the file called "Deploy.To.Local.ps1". It can be found under the deploy folder in the visual studio solution.

Modify the first line in the file, where there is a variable called $website_root

$website_root = "C:\inetpub\sc8\Website"

The value needs to match the website root of your Sitecore instance where Ucommerce is installed.

### Commerce Connect ###

Per default the Ucommerce.CommerceConnect project is not loaded. If you want to build a version for Sitecore 8 with Commerce Connect, simply include the project and run the commands above. Then it will automatically be included in the package. 

### Who do I talk to? ###

* https://eureka.ucommerce.net