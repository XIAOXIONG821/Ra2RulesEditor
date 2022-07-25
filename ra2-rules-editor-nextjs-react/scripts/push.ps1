function Run{
    $outFolderName="out"
    $outPath="./"+$outFolderName
    $csPath="../Ra2RulesEditorAPI/Ra2RulesEditorAPI.Web.Entry/"
    $rootPath=$csPath+"wwwroot"
    
    # build next.js , export HTML
    npx next build
    npx next export
    
    # delete wwwroot
    if (test-path $rootPath) {
      Remove-item $rootPath -force -recurse
    }
	
    # copy out -> wwwroot
    Copy-Item $outPath $csPath -Force -Recurse
    rename-Item $csPath$outFolderName wwwroot
}

# ********************************************
# try catch template
# ********************************************
$eap = $ErrorActionPreference
Try{
    $ErrorActionPreference = 'Stop'

    Run
}
Catch{    
    # show error
    Write-Output $PSItem
}
Finally{
    $ErrorActionPreference = $eap
}