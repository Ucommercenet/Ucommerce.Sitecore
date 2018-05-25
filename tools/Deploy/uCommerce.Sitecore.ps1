task CreateSitecorePackage -depends ValidateSetup,CleanSitecoreWorkingDirectory,Rebuild,CopySitecoreFiles, CreateSitecoreZipFile {

}

task SetSitecoreVars -description "Since path are different from Deploy.To.Local or Deploy.To.Package, we need to set them differently." {
    if($CreatePackage)
    {
        $script:hash.ucommerce_dir = "$working_dir\files\sitecore modules\Shell\uCommerce"
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

task CopySitecoreFiles -description "Copy all the sitecore files needs for a deployment" -depends CopyUCommerceFiles {

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
    New-Item "$working_dir\files\sitecore modules\Shell\uCommerce\install" -Force -ItemType Directory
    New-Item "$working_dir\files\sitecore modules\Shell\uCommerce\install\configInclude" -Force -ItemType Directory
    New-Item "$working_dir\files\sitecore modules\Shell\uCommerce\shell" -Force -ItemType Directory
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
