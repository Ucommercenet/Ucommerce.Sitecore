task CreateSitecorePackage -depends ValidateSetup,CleanSitecoreWorkingDirectory,Rebuild,CopySitecoreFiles, CreateSitecoreZipFile {

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

	# copy client resources
	&robocopy "$src\UCommerce.Sitecore.Web\ucommerce" "$working_dir\files\sitecore modules\Shell\ucommerce" /is /it /e /NFL /NDL

	# Start overriding CMS specific things to the package
	##Shell specific
	Copy-Item "$src\UCommerce.Sitecore.Web\Shell\CatalogManager.aspx" "$working_dir\files\sitecore modules\Shell\ucommerce\shell\CatalogManager.aspx" -Force
	Copy-Item "$src\UCommerce.Sitecore.Web\Shell\OrderManager.aspx" "$working_dir\files\sitecore modules\Shell\ucommerce\shell\OrderManager.aspx" -Force
	Copy-Item "$src\UCommerce.Sitecore.Web\Shell\PromotionManager.aspx" "$working_dir\files\sitecore modules\Shell\ucommerce\shell\PromotionManager.aspx" -Force
	Copy-Item "$src\UCommerce.Sitecore.Web\Shell\SettingsManager.aspx" "$working_dir\files\sitecore modules\Shell\ucommerce\shell\SettingsManager.aspx" -Force

	Copy-Item "$src\UCommerce.Sitecore.Web\Shell\Scripts\Sitecore.js" "$working_dir\files\sitecore modules\Shell\ucommerce\shell\app\constants.js" -Force

	#Configuration specific
	&robocopy "$src\UCommerce.Sitecore.Web\Apps\Speak" "$working_dir\files\sitecore modules\Shell\ucommerce\Apps\Speak" /is /it /e /NFL /NDL
	Copy-Item "$src\UCommerce.Sitecore.Web\Configuration\Shell.config.default" "$working_dir\files\sitecore modules\Shell\ucommerce\Configuration\Shell.config.default" -Force

	#Copy sql scripts to install folder
	&robocopy "$src\..\database" "$working_dir\files\sitecore modules\Shell\ucommerce\install" *.sql /NFL /NDL
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
	New-Item "$working_dir\files\sitecore modules\Shell\" -Force -ItemType Directory
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
