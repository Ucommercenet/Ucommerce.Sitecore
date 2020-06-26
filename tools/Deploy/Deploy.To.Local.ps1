# Website root folder (website is deployed here)
$website_root = "C:\inetpub\sc9\Website"

$ucommerce_folder = "$website_root\sitecore modules\Shell\Ucommerce\"
# Temporarily change to the correct folder containing script
$scriptPath = (Get-Variable MyInvocation -Scope Script).Value.MyCommand.Path
$currentFolder = Split-Path $scriptPath
Push-Location $currentFolder

# Set src folder based on location of script location in /tools/deploy
$src = ".\..\..\src\"
$website_project = "$src\Ucommerce.Sitecore.Web"

# Exclude files and folders from deploy, usually these are
# source code files, proj files from Visual Studio, and other
# files not required at runtime
$options = @("/E", "/S", "/xf", "*.cs", "/xf", "*.??proj", "/xf", "*.user", "/xf", "*.old", "/xf", "*.vspscc", "/xf", "xsltExtensions.config", "/xf", "uCommerce.key", "/xf", "*.tmp", "/xd", "_Resharper*", "/xd", ".svn", "/xd", "_svn", "/xf", "Ucommerce.dll", "/xf", "Ucommerce.Presentation.dll", "/xf", "Ucommerce.Web.Api.dll", "Ucommerce.Infrastructure.dll", "/xf", "UCommerce.Transactions.Payments.dll", "/xf", "UCommerce.Pipelines.dll", "/xf", "ServiceStack.*", "/xf", "web.config")

# Copy all site specific files into the website
& robocopy "$website_project\Apps" "$ucommerce_folder\Apps" $options
& robocopy "$website_project\Configuration" "$ucommerce_folder\Configuration" $options
& robocopy "$website_project\Css" "$ucommerce_folder\Css" $options
& robocopy "$website_project\Pipelines" "$ucommerce_folder\Pipelines" $options
& robocopy "$website_project\Scripts" "$ucommerce_folder\Scripts" $options
& robocopy "$website_project\Shell" "$ucommerce_folder\Shell" $options

Copy-Item "$src\UCommerce.Sitecore\bin\Debug\Ucommerce.Sitecore.dll" "$website_root\bin\Ucommerce.Sitecore.dll"
Copy-Item "$src\UCommerce.Sitecore\bin\Debug\Ucommerce.Sitecore.pdb" "$website_root\bin\Ucommerce.Sitecore.pdb"

Copy-Item "$website_project\bin\Ucommerce.Sitecore.Web.dll" "$website_root\bin\Ucommerce.Sitecore.Web.dll"
Copy-Item "$website_project\bin\Ucommerce.Sitecore.Web.pdb" "$website_root\bin\Ucommerce.Sitecore.Web.pdb"

if (Test-Path "$src\Ucommerce.Sitecore.CommerceConnect\bin\Debug\Ucommerce.Sitecore.CommerceConnect.dll")
{
	Copy-Item "$src\Ucommerce.Sitecore.CommerceConnect\bin\Debug\Ucommerce.Sitecore.CommerceConnect.dll" "$website_root\bin\Ucommerce.Sitecore.CommerceConnect.dll"
	Copy-Item "$src\Ucommerce.Sitecore.CommerceConnect\bin\Debug\Ucommerce.Sitecore.CommerceConnect.pdb" "$website_root\bin\Ucommerce.Sitecore.CommerceConnect.pdb"
}

if ((Test-Path "$website_root\Sitecore modules\shell\ucommerce\apps\Sitecore92compatibility\bin") -eq $false) {
	New-Item "$website_root\Sitecore modules\shell\ucommerce\apps\Sitecore92compatibility\bin" -Type Directory -Force
}

Copy-Item "$src\Ucommerce.Sitecore92/bin/debug/Ucommerce.Sitecore92.dll" "$website_root\sitecore modules\Shell\Ucommerce\Apps\Sitecore92compatibility\bin\Ucommerce.Sitecore92.dll"
Copy-Item "$src\Ucommerce.Sitecore92/bin/debug/Ucommerce.Sitecore92.pdb" "$website_root\sitecore modules\Shell\Ucommerce\Apps\Sitecore92compatibility\bin\Ucommerce.Sitecore92.pdb"

if ((Test-Path "$website_root\Sitecore modules\shell\ucommerce\apps\Sitecore93compatibility\bin") -eq $false) {
	New-Item "$website_root\Sitecore modules\shell\ucommerce\apps\Sitecore93compatibility\bin" -Type Directory -Force
}

Copy-Item "$src\Ucommerce.Sitecore93/bin/debug/Ucommerce.Sitecore93.dll" "$website_root\sitecore modules\Shell\Ucommerce\Apps\Sitecore93compatibility\bin\Ucommerce.Sitecore93.dll"
Copy-Item "$src\Ucommerce.Sitecore93/bin/debug/Ucommerce.Sitecore93.pdb" "$website_root\sitecore modules\Shell\Ucommerce\Apps\Sitecore93compatibility\bin\Ucommerce.Sitecore93.pdb"

# Now back to original directory
Pop-Location