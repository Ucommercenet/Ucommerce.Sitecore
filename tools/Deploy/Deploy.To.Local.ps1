# Website root folder (website is deployed here)
$website_root = "C:\inetpub\sc8\Website"

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
$options = @("/E", "/S", "/xf", "*.cs", "/xf", "*.??proj", "/xf", "*.user", "/xf", "*.old", "/xf", "*.vspscc", "/xf", "xsltExtensions.config", "/xf", "uCommerce.key", "/xf", "*.tmp", "/xd", "_Resharper*", "/xd", ".svn", "/xd", "_svn", "/xf", "UCommerce.dll", "/xf", "UCommerce.Presentation.dll", "/xf", "UCommerce.Web.Api.dll", "UCommerce.Infrastructure.dll", "/xf", "UCommerce.Transactions.Payments.dll", "/xf", "UCommerce.Pipelines.dll", "/xf", "ServiceStack.*")

# Copy all site specific files into the website
& robocopy "$website_project\Apps" "$ucommerce_folder\Apps" $options
& robocopy "$website_project\Configuration" "$ucommerce_folder\Configuration" $options
& robocopy "$website_project\Css" "$ucommerce_folder\Css" $options
& robocopy "$website_project\Pipelines" "$ucommerce_folder\Pipelines" $options
& robocopy "$website_project\Scripts" "$ucommerce_folder\Scripts" $options
& robocopy "$website_project\Shell" "$ucommerce_folder\Shell" $options

Copy-Item "$src\UCommerce.Sitecore\bin\Debug\UCommerce.Sitecore.dll" "$website_root\bin\UCommerce.Sitecore.dll"
Copy-Item "$src\UCommerce.Sitecore\bin\Debug\UCommerce.Sitecore.pdb" "$website_root\bin\UCommerce.Sitecore.pdb"

Copy-Item "$website_project\bin\UCommerce.Sitecore.Web.dll" "$website_root\bin\UCommerce.Sitecore.Web.dll"
Copy-Item "$website_project\bin\UCommerce.Sitecore.Web.pdb" "$website_root\bin\UCommerce.Sitecore.Web.pdb"

if (Test-Path "$src\UCommerce.Sitecore.CommerceConnect\bin\Debug\UCommerce.Sitecore.CommerceConnect.dll")
{
	Copy-Item "$src\UCommerce.Sitecore.CommerceConnect\bin\Debug\UCommerce.Sitecore.CommerceConnect.dll" "$website_root\bin\UCommerce.Sitecore.CommerceConnect.dll"
	Copy-Item "$src\UCommerce.Sitecore.CommerceConnect\bin\Debug\UCommerce.Sitecore.CommerceConnect.pdb" "$website_root\bin\UCommerce.Sitecore.CommerceConnect.pdb"
}

# Now back to original directory
Pop-Location