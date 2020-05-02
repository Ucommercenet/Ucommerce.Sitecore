task CreateSitecorePackage -depends SetVersionNumberFromClientNugetPackage,CleanSitecoreWorkingDirectory,NuGetRestore, Rebuild,CopySitecoreFiles, CreateFakeAssembliesForUpgrade, AddCompatibilityApp, CleanPackageForOtherCmsDependencies, CreateSitecoreZipFile {

}

task AddCompatibilityApp {
	if ((Test-Path "$working_dir\Files\Sitecore modules\shell\ucommerce\apps\Sitecore92compatibility.disabled\bin") -eq $false) {
		New-Item "$working_dir\Files\Sitecore modules\shell\ucommerce\apps\Sitecore92compatibility.disabled\bin" -Type Directory -Force
	}

	Copy-Item "$src\Ucommerce.Sitecore92\bin\$configuration\Ucommerce.Sitecore92.dll" "$working_dir\Files\Sitecore modules\shell\ucommerce\apps\Sitecore92compatibility.disabled\bin" -Force
}

task SetVersionNumberFromClientNugetPackage {
	Push-Location "$src\packages"

	$folderItem = Get-ChildItem uCommerce.client.*

	if("System.Object[]" -eq $folderItem.GetType())
	{
		#return the last element in an array:
		#$myArray[-1]
		$folderItem = $folderItem[-1];
	}

	Push-Location "$folderItem\lib\net45"

	$info = Get-ChildItem -Filter Ucommerce.Admin.dll -Recurse | Select-Object -ExpandProperty VersionInfo

	# Removing build part from the Ucommerce packages.
	$version = $info.FileVersion.SubString(0,$info.FileVersion.LastIndexOf('.'))

	# Adding build number from date.
	$script:versionDateNumberPart = (Get-Date).Year.ToString().Substring(2) + "" + (Get-Date).DayOfYear.ToString("000");
	$script:version = "$version." + $script:versionDateNumberPart
	Pop-Location
}

task NuGetRestore {
	Push-Location "$src\..\tools\Nuget"
	.\nuget restore ..\..\src
	Pop-Location
}

task SetSitecoreVars -description "Since path are different from Deploy.To.Local or Deploy.To.Package, we need to set them differently." {
    if($CreatePackage)
    {
        $script:hash.ucommerce_dir = "$working_dir\files\sitecore modules\Shell\Ucommerce"
        $script:hash.bin_dir = "$working_dir\files\bin"
        $script:hash.files_root_dir = "$working_dir\files"
    }
    else
    {
        $script:hash.ucommerce_dir = "$working_dir\sitecore modules\Shell\Ucommerce"
        $script:hash.bin_dir = "$working_dir\bin"
        $script:hash.files_root_dir = "$working_dir"
    }
}

task CreateFakeAssembliesForUpgrade {
	# Create empty assemblies to enable upgrades from v8 -> v9
	$bin_dir = $script:hash["bin_dir"]
	New-Item -Path "$bin_dir" -Name "Ucommerce.Admin.dll" -ItemType File
	New-Item -Path "$bin_dir" -Name "Ucommerce.SystemWeb.dll" -ItemType File
}

task CleanPackageForOtherCmsDependencies {
	$items = Get-ChildItem $working_dir -Recurse | Where-Object { $_.FullName.ToLower().Contains("umbraco") -or $_.FullName.ToLower().Contains("sitefinity") -or $_.FullName.ToLower().Contains("kentico")}

	foreach($item in $items) {
		if (Test-Path $item.FullName) {
			Write-Host "Removing " + $item.FullName
			Remove-Item $item.FullName -Force -Recurse
		}
        else {
            Write-Host "test-path failed for " + $item.FullName
        }
	}
}

task CopySitecoreFiles -description "Copy all the sitecore files needs for a deployment" {
	$ucommerce_dir = $script:hash["ucommerce_dir"]
	$base_dir = $script:hash["base_dir"]
    $bin_dir = $script:hash["bin_dir"]
    $files_root = $script:hash["files_root_dir"]

	# First let's copy the package data for the zip
	&robocopy "$src\Ucommerce.Sitecore.Installer\package\installer" "$working_dir\installer" /is /it /e /NFL /NDL
	&robocopy "$src\Ucommerce.Sitecore.Installer\package\metadata" "$working_dir\metadata" /is /it /e /NFL /NDL
	&robocopy "$src\Ucommerce.Sitecore.Installer\package\Files" "$working_dir\Files" /is /it /e /NFL /NDL
	&robocopy "$src\Ucommerce.Sitecore.Installer\SpeakSerialization" "$working_dir\Files\Sitecore modules\shell\ucommerce\install\SpeakSerialization" /is /it /e /NFL /NDL

	# Copy binaries needed for the actual bootstrapping
	Copy-Item "$src\Ucommerce.SiteCore.Installer\bin\$configuration\Ucommerce.Installer.dll" "$working_dir\files\bin\Ucommerce.Installer.dll" -Force
	Copy-Item "$src\Ucommerce.SiteCore.Installer\bin\$configuration\Ucommerce.Sitecore.Installer.dll" "$working_dir\files\bin\Ucommerce.Sitecore.Installer.dll" -Force
	Copy-Item "$src\..\lib\XmlTransform\Microsoft.Web.XmlTransform.dll" "$working_dir\files\bin\Microsoft.Web.XmlTransform.dll" -Force

	# Binaries for the site to run

	# Only copy commerce connect if exists (exluded from sln per default).
	if (Test-Path "$src\Ucommerce.Sitecore.CommerceConnect\bin\$configuration\Ucommerce.Sitecore.CommerceConnect.dll") {
		Copy-Item "$src\Ucommerce.Sitecore.CommerceConnect\bin\$configuration\Ucommerce.Sitecore.CommerceConnect.dll" "$working_dir\Files\Sitecore modules\shell\ucommerce\install\binaries" -Force
	}

	Copy-Item "$src\Ucommerce.Sitecore.Web\bin\Ucommerce.Sitecore.Web.dll" "$working_dir\Files\Sitecore modules\shell\ucommerce\install\binaries" -Force

	#Lets delete the Ucommerce folder that Client nuget package copies over - we'll just grab from the package location
	if (Test-Path "$src\Ucommerce.Sitecore.Web\ucommerce") {
		Remove-Item "$src\Ucommerce.Sitecore.Web\ucommerce" -Force -Recurse
	}

	# copy client resources from client nuget package. Even though it copies the files to a ucommerce folder, this will only happen during installatino not restore
   	Push-Location "$src\packages"
    $path = Get-ChildItem -Include uCommerce.client* -name

	if("System.Object[]" -eq $path.GetType())
	{
		#return the last element in an array:
		#$myArray[-1]
		$path = $path[-1];
	}

    Pop-Location

	&robocopy "$src\packages\$path\uCommerceFiles" "$working_dir\files\sitecore modules\Shell" /is /it /e /NFL /NDL

	# Start overriding CMS specific things to the package
	##Shell specific
	Copy-Item "$src\Ucommerce.Sitecore.Web\Shell\CatalogManager.aspx" "$working_dir\files\sitecore modules\Shell\ucommerce\shell\CatalogManager.aspx" -Force
	Copy-Item "$src\Ucommerce.Sitecore.Web\Shell\OrderManager.aspx" "$working_dir\files\sitecore modules\Shell\ucommerce\shell\OrderManager.aspx" -Force
	Copy-Item "$src\Ucommerce.Sitecore.Web\Shell\PromotionManager.aspx" "$working_dir\files\sitecore modules\Shell\ucommerce\shell\PromotionManager.aspx" -Force
	Copy-Item "$src\Ucommerce.Sitecore.Web\Shell\SettingsManager.aspx" "$working_dir\files\sitecore modules\Shell\ucommerce\shell\SettingsManager.aspx" -Force
	Copy-Item "$src\Ucommerce.Sitecore.Web\Shell\Scripts\Sitecore.js" "$working_dir\files\sitecore modules\Shell\ucommerce\shell\app\constants.js" -Force

	#Configuration specific
	&robocopy "$src\Ucommerce.Sitecore.Web\Apps" "$working_dir\files\sitecore modules\Shell\ucommerce\Apps" /is /it /e /NFL /NDL
	Copy-Item "$src\Ucommerce.Sitecore.Web\Configuration\Shell.config.default" "$working_dir\files\sitecore modules\Shell\ucommerce\Configuration\Shell.config.default" -Force

	New-Item "$working_dir\Files\Sitecore modules\shell\ucommerce\install\configinclude" -ItemType Directory
	&robocopy "$src\Ucommerce.SiteCore.Installer\ConfigurationTransformations\ConfigIncludes" "$working_dir\Files\Sitecore modules\shell\ucommerce\install\configinclude" * /NFL /NDL
	&robocopy "$src\Ucommerce.SiteCore.Installer\ConfigurationTransformations" "$working_dir\Files\Sitecore modules\shell\ucommerce\install" *.config /NFL /NDL

	#Copy sql scripts to install folder
	&robocopy "$src\..\database" "$working_dir\files\sitecore modules\Shell\ucommerce\install" *.sql /NFL /NDL

	#css
	&robocopy "$src\Ucommerce.Sitecore.Web\Css" "$working_dir\files\sitecore modules\Shell\ucommerce\Css" * /is /it /e /NFL /NDL

	#binaries
	&robocopy "$src\Ucommerce.Sitecore\bin\$configuration" "$working_dir\files\sitecore modules\Shell\ucommerce\install\binaries" Ucommerce.* /is /it /e /NFL /NDL

    $dependencies = @("castle.core.dll", "castle.windsor.dll", "clientdependency.core.dll","Castle.Facilities.AspNet.SystemWeb.dll" , "csvhelper.dll", "epplus.dll", "fluentnhibernate.dll", "iesi.collections.dll", "infralution.licensing.dll", "log4net.dll", "lucene.net.dll", "microsoft.web.xmltransform.dll". "newtonsoft.json.dll", "nhibernate.caches.syscache.dll", "nhibernate.dll", "Antlr3.Runtime.dll", "Remotion.Linq.dll","Remotion.Linq.EagerFetching.dll", "FluentValidation.dll", "Slugify.Core.dll", "Dapper.dll")
	CopyFiles "$src\Ucommerce.Sitecore\bin\$configuration" "$working_dir\files\sitecore modules\Shell\ucommerce\install\binaries" $dependencies

	# Commerce Connect app
	New-Item "$working_dir\Files\Sitecore modules\shell\ucommerce\apps\sitecore commerce connect.disabled" -ItemType Directory
	&robocopy "$src\Ucommerce.Sitecore.CommerceConnect\Configuration" "$working_dir\Files\Sitecore modules\shell\ucommerce\apps\sitecore commerce connect.disabled" * /NFL /NDL

	Move-Item "$working_dir\Files\Sitecore modules\shell\ucommerce\configuration\settings\settings.sitecore.config.default" "$working_dir\Files\Sitecore modules\shell\ucommerce\configuration\settings\settings.config.default" -Force

	Remove-Item "$working_dir\Files\Sitecore modules\shell\ucommerce\css\speak\*.less" -Force

	&robocopy "$src\Ucommerce.Sitecore.Web\Pipelines" "$working_dir\Files\Sitecore modules\shell\ucommerce\Pipelines" * /is /it /e /NFL /NDL

	# Other files that are part of the client package that should not be there
	Remove-Item "$working_dir\Files\Sitecore modules\shell\ucommerce\scripts\ucommerce6.js" -Force
	Remove-Item "$working_dir\Files\Sitecore modules\shell\ucommerce\pipelines\baskets.addaddress.sitecore.config.default" -Force
	Remove-Item "$working_dir\Files\Sitecore modules\shell\ucommerce\shell\settingsmanager.aspx" -Force
	Remove-Item "$working_dir\Files\Sitecore modules\shell\ucommerce\css\sitecore\sitecore.less" -Force
	Remove-Item "$working_dir\Files\Sitecore modules\shell\ucommerce\install\binaries\ucommerce.sitecore.dll.config" -Force
	Remove-Item "$working_dir\*.orig" -Force -Recurse

	if ($configuration -eq "Release") {
		Remove-Item "$working_dir\Files\Sitecore modules\shell\ucommerce\install\binaries\*.pdb" -Force
		Remove-Item "$working_dir\Files\Sitecore modules\shell\ucommerce\install\binaries\*.xml" -Force
	}

	##### Special version of Lucene.net #####
	Copy-Item "$src\..\lib\Lucene.net\Lucene.net.dll" "$working_dir\files\Sitecore modules\shell\ucommerce\install\binaries\Lucene.net.dll" -Force

	if (Test-Path "$working_dir\files\sitecore modules\Shell\Ucommerce\Apps\Acquire%20and%20Cancel%20Payments.disabled") {
		Rename-Item -Path "$working_dir\files\sitecore modules\Shell\Ucommerce\Apps\Acquire%20and%20Cancel%20Payments.disabled" -NewName "Acquire and Cancel Payments.disabled" -Force
	}

	if (Test-Path "$working_dir\files\sitecore modules\shell\ucommerce\install\binaries\Ucommerce.Installer.dll"){
		Remove-Item "$working_dir\files\sitecore modules\shell\ucommerce\install\binaries\Ucommerce.Installer.dll" -Force
	}
}

task CleanSitecoreWorkingDirectory -description "Cleans the sitecore working directory. This should NOT be used when using Deploy.To.Local" -depends SetSitecoreVars{
    # Create directories
    if(Test-Path $working_dir)
    {
        Remove-Item -Recurse "$working_dir\*" -Force
    }
    else
    {
        New-Item "$working_dir" -Force -ItemType Directory
    }

    New-Item "$working_dir\files\bin\uCommerce" -Force -ItemType Directory
	New-Item "$working_dir\files\sitecore modules\Shell" -Force -ItemType Directory
	New-Item "$working_dir\files\sitecore modules\Shell\Ucommerce" -Force -ItemType Directory
	New-Item "$working_dir\files\sitecore modules\Shell\Ucommerce\Install" -Force -ItemType Directory
	New-Item "$working_dir\files\sitecore modules\Shell\Ucommerce\Install\Binaries" -Force -ItemType Directory
	New-Item "$working_dir\installer" -Force -ItemType Directory
    New-Item "$working_dir\metadata" -Force -ItemType Directory
}

task CreateSitecoreZipFile -description "Creates the Sitecore Zip fil" {
    Assert($script:version -ne $null) "'version' cannot be null."
    Assert($zipDestinationFolder -ne $null) "'zipDestinationFolder' cannot be null."
    Assert($zipFileName -ne $null) "'zipFileName' cannot be null."

    # Create the filename.
    $newFileName = GetZipFilename

    $packageZipFullName = "$zipDestinationFolder\package.zip"
    if(Test-Path $packageZipFullName)
    {
        Remove-Item $packageZipFullName
    }

    # Create a zip file from the working_dir.
    Exec { Invoke-Expression "& '$base_dir\tools\7zip\7z.exe' a -r -tZip -mx9 $packageZipFullName '$working_dir\*'" } | out-null

    Exec { Invoke-Expression "& '$base_dir\tools\7zip\7z.exe' a -tZip -mx9 $newFileName '$packageZipFullName'" }

    del $packageZipFullName
}

task UpdateSitecorePackageInfo -description "Updates the Sitecore package information file" {
    if($UpdateAssemblyInfo -eq "True") {
        $version = $script:version
        echo "$version" > "$src\Ucommerce.Sitecore.Installer\package\metadata\sc_version.txt"
        echo "Ucommerce $version" > "$src\Ucommerce.Sitecore.Installer\package\metadata\sc_name.txt"
    }
}
