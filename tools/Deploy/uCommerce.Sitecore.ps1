task CreateSitecorePackage -depends SetVersionNumberFromClientNugetPackage,ValidateSetup,CleanSitecoreWorkingDirectory,NuGetRestore, Rebuild,CopySitecoreFiles, CleanPackageForOtherCmsDependencies, CreateSitecoreZipFile {

}

task SetVersionNumberFromClientNugetPackage {
	Push-Location "$src\packages"

	$folderItem = Get-ChildItem uCommerce.client.* 

	Push-Location "$folderItem\lib\net45"

	$info = Get-ChildItem -Filter UCommerce.Admin.dll -Recurse | Select-Object -ExpandProperty VersionInfo

	$script:version = $info.FileVersion

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
        $script:hash.ucommerce_dir = "$working_dir\sitecore modules\Shell\uCommerce"
        $script:hash.bin_dir = "$working_dir\bin"
        $script:hash.files_root_dir = "$working_dir"
    }
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
	&robocopy "$src\Ucommerce.SiteCore.Installer\package\installer" "$working_dir\installer" /is /it /e /NFL /NDL
	&robocopy "$src\Ucommerce.SiteCore.Installer\package\metadata" "$working_dir\metadata" /is /it /e /NFL /NDL
	&robocopy "$src\Ucommerce.SiteCore.Installer\package\Files" "$working_dir\Files" /is /it /e /NFL /NDL
	&robocopy "$src\Ucommerce.SiteCore.Installer\SpeakSerialization" "$working_dir\Files\Sitecore modules\shell\ucommerce\install\SpeakSerialization" /is /it /e /NFL /NDL

	# Copy binaries needed for the actual bootstrapping
	Copy-Item "$src\UCommerce.SiteCore.Installer\bin\$configuration\UCommerce.Installer.dll" "$working_dir\files\bin\UCommerce.Installer.dll" -Force
	Copy-Item "$src\UCommerce.SiteCore.Installer\bin\$configuration\UCommerce.Sitecore.Installer.dll" "$working_dir\files\bin\UCommerce.Sitecore.Installer.dll" -Force
	Copy-Item "$src\..\lib\XmlTransform\Microsoft.Web.XmlTransform.dll" "$working_dir\files\bin\Microsoft.Web.XmlTransform.dll" -Force

	# Binaries for the site to run
	Copy-Item "$src\UCommerce.Sitecore.CommerceConnect\bin\$configuration\UCommerce.Sitecore.CommerceConnect.dll" "$working_dir\Files\Sitecore modules\shell\ucommerce\install\binaries" -Force

	#Lets delete the Ucommerce folder that Client nuget package copies over - we'll just grab from the package location
	if (Test-Path "$src\Ucommerce.Sitecore.Web\ucommerce") {
		Remove-Item "$src\Ucommerce.Sitecore.Web\ucommerce" -Force -Recurse
	}
 
	# copy client resources from client nuget package. Even though it copies the files to a ucommerce folder, this will only happen during installatino not restore
   	Push-Location "$src\packages"
    $path = Get-ChildItem -Include uCommerce.client* -name
    Pop-Location

	&robocopy "$src\packages\$path\uCommerceFiles" "$working_dir\files\sitecore modules\Shell" /is /it /e /NFL /NDL

	# Start overriding CMS specific things to the package
	##Shell specific
	Copy-Item "$src\UCommerce.Sitecore.Web\Shell\CatalogManager.aspx" "$working_dir\files\sitecore modules\Shell\ucommerce\shell\CatalogManager.aspx" -Force
	Copy-Item "$src\UCommerce.Sitecore.Web\Shell\OrderManager.aspx" "$working_dir\files\sitecore modules\Shell\ucommerce\shell\OrderManager.aspx" -Force
	Copy-Item "$src\UCommerce.Sitecore.Web\Shell\PromotionManager.aspx" "$working_dir\files\sitecore modules\Shell\ucommerce\shell\PromotionManager.aspx" -Force
	Copy-Item "$src\UCommerce.Sitecore.Web\Shell\SettingsManager.aspx" "$working_dir\files\sitecore modules\Shell\ucommerce\shell\SettingsManager.aspx" -Force
	Copy-Item "$src\UCommerce.Sitecore.Web\Shell\Scripts\Sitecore.js" "$working_dir\files\sitecore modules\Shell\ucommerce\shell\app\constants.js" -Force

	#Configuration specific
	&robocopy "$src\UCommerce.Sitecore.Web\Apps" "$working_dir\files\sitecore modules\Shell\ucommerce\Apps" /is /it /e /NFL /NDL
	Copy-Item "$src\UCommerce.Sitecore.Web\Configuration\Shell.config.default" "$working_dir\files\sitecore modules\Shell\ucommerce\Configuration\Shell.config.default" -Force

	New-Item "$working_dir\Files\Sitecore modules\shell\ucommerce\install\configinclude" -ItemType Directory
	&robocopy "$src\UCommerce.SiteCore.Installer\ConfigurationTransformations\ConfigIncludes" "$working_dir\Files\Sitecore modules\shell\ucommerce\install\configinclude" * /NFL /NDL
	&robocopy "$src\UCommerce.SiteCore.Installer\ConfigurationTransformations" "$working_dir\Files\Sitecore modules\shell\ucommerce\install" *.config /NFL /NDL

	#Copy sql scripts to install folder
	&robocopy "$src\..\database" "$working_dir\files\sitecore modules\Shell\ucommerce\install" *.sql /NFL /NDL

	#css
	&robocopy "$src\UCommerce.Sitecore.Web\Css" "$working_dir\files\sitecore modules\Shell\ucommerce\Css" * /is /it /e /NFL /NDL

	#binaries
	&robocopy "$src\UCommerce.Sitecore\bin\$configuration" "$working_dir\files\sitecore modules\Shell\ucommerce\install\binaries" UCommerce.* /is /it /e /NFL /NDL
	
    $dependencies = @("AuthorizeNet.dll", "braintree-2.22.0.dll", "castle.core.dll", "castle.windsor.dll", "clientdependency.core.dll", "csvhelper.dll", "epplus.dll", "fluentnhibernate.dll", "iesi.collections.dll", "infralution.licensing.dll", "log4net.dll", "lucene.net.dll", "microsoft.web.xmltransform.dll". "newtonsoft.json.dll", "nhibernate.caches.syscache.dll", "nhibernate.dll", "paypal_base.dll", "system.net.http.formatting.dll", "system.web.http.dll", "system.web.http.webhost.dll")
	CopyFiles "$src\UCommerce.Sitecore\bin\$configuration" "$working_dir\files\sitecore modules\Shell\ucommerce\install\binaries" $dependencies

	# Commerce Connect app
	New-Item "$working_dir\Files\Sitecore modules\shell\ucommerce\apps\sitecore commerce connect.disabled" -ItemType Directory
	&robocopy "$src\UCommerce.Sitecore.CommerceConnect\Configuration" "$working_dir\Files\Sitecore modules\shell\ucommerce\apps\sitecore commerce connect.disabled" * /NFL /NDL

	Move-Item "$working_dir\Files\Sitecore modules\shell\ucommerce\configuration\settings\settings.sitecore.config.default" "$working_dir\Files\Sitecore modules\shell\ucommerce\configuration\settings\settings.config.default" -Force

	# Raven Lucene.net assembly not used in Sitecore and can make weired errors. Don't copy it over.
	Remove-Item "$working_dir\Files\Sitecore modules\shell\ucommerce\apps\RavenDB25.disabled\bin\Lucene.net.dll" -Force
	
	Remove-Item "$working_dir\Files\Sitecore modules\shell\ucommerce\css\speak\*.less" -Force
	
	&robocopy "$src\UCommerce.Sitecore.Web\Pipelines" "$working_dir\Files\Sitecore modules\shell\ucommerce\Pipelines" * /is /it /e /NFL /NDL

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


	##### ServiceStack ######
	Copy-Item "$src\..\lib\ServiceStack\3.9.55\ServiceStack.Common.3.9.55\lib\net35\ServiceStack.Common.dll" "$working_dir\files\bin\ServiceStack.Common.dll" -Force
	Copy-Item "$src\..\lib\ServiceStack\3.9.55\ServiceStack.Common.3.9.55\lib\net35\ServiceStack.interfaces.dll" "$working_dir\files\bin\ServiceStack.interfaces.dll" -Force
	Copy-Item "$src\..\lib\ServiceStack\3.9.55\ServiceStack.3.9.55\lib\net35\ServiceStack.dll" "$working_dir\files\bin\ServiceStack.dll" -Force
	Copy-Item "$src\..\lib\ServiceStack\3.9.55\ServiceStack.3.9.55\lib\net35\ServiceStack.ServiceInterface.dll" "$working_dir\files\bin\ServiceStack.ServiceInterface.dll" -Force
	Copy-Item "$src\..\lib\ServiceStack\3.9.55\ServiceStack.Text.3.9.55\lib\net35\ServiceStack.Text.dll" "$working_dir\files\bin\ServiceStack.Text.dll" -Force

	if (Test-Path "$working_dir\files\sitecore modules\Shell\Ucommerce\Apps\Acquire%20and%20Cancel%20Payments.disabled") {
		Rename-Item -Path "$working_dir\files\sitecore modules\Shell\Ucommerce\Apps\Acquire%20and%20Cancel%20Payments.disabled" -NewName "Acquire and Cancel Payments.disabled" -Force
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

task UpdateSitecorePackageInfo -description "Updates the Sitecore package information file" -precondition { return ($target.ToUpper().Equals("sitecore".ToUpper()))}{
    if($UpdateAssemblyInfo -eq "True") {
        $version = $script:version
        Get-Content "$src\UCommerce.Installer\PackageInformation\Sitecore.txt" > "$src\UCommerce.Sitecore.Installer\package\metadata\sc_readme.txt"
        "$version" > "$src\UCommerce.Sitecore.Installer\package\metadata\sc_version.txt"
        "uCommerce $version" > "$src\UCommerce.Sitecore.Installer\package\metadata\sc_name.txt"
    }
}
