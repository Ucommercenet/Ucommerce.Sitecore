[CmdletBinding()]
Param(

    [Parameter(Mandatory=$False)]
    [ValidateSet("True", "False")]
    [string]$UpdateAssemblyInfo = "True",

    [Parameter(Mandatory=$False)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",
    
    [Parameter(Mandatory=$True)]
    [string]$Version
)

function Get-ScriptDirectory { 
    Split-Path -parent $PSCommandPath 
}

function Run-It () {
    try {  
        $scriptPath = Get-ScriptDirectory
        . "$scriptPath\Deploy.Common.ps1"
    
        $src = Resolve-Path "$scriptPath\..\..\src";
        $base_dir = Resolve-Path "$scriptPath\..\.."

        Import-Module "$scriptPath\..\psake\4.7.0\psake.psm1"
        
        $properties = @{
                "configuration"="$Configuration"; 
                "UpdateAssemblyInfo"="$UpdateAssemblyInfo";
                "version"="$Version";
                "base_dir"="$base_dir";
                "src"=$src;
                "working_dir"="$env:TEMP\uCommerceTmp\8e0acd5c-f842-49db-933d-cc9e61fcff53";
                "DeploymentDirectories" = GetDeploymentDirectories;
            };

	Invoke-PSake "$scriptPath\uCommerce.build.ps1" "CreateSitecorePackage" -properties $properties
        
        if ($configuration -eq "Debug") {

            Push-Location $src

			hg revert -I "glob:**\*AssemblyInfo.cs"
			hg revert -I "glob:**\*ClientDependency*.config"
			hg revert -I "glob:**\*sc_*.txt"
			hg revert -I "glob:**\*Package*.xml"

            Pop-Location
	}
		
    } catch {  
        Write-Error $_.Exception.Message -ErrorAction Stop  
    }
}

Run-It