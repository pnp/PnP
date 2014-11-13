param($installPath, $toolsPath, $package, $project)

try {
    $references = $project.Object.References
    
    $oldref = $references | ? { $_.name -eq "Microsoft.SharePoint.Client"}
    $oldref.remove()
    
    $oldref = $references | ? { $_.Name -eq "Microsoft.SharePoint.Client.Runtime" }
    $oldref.remove()
    
    $references.Add("Microsoft.SharePoint.Client, Version=16.0.0.0")
    $references.Add("Microsoft.SharePoint.Client.Runtime, Version=16.0.0.0")
    
    $project.Save()

} catch {

    Write-Host "Error while installing package: " + $_.Exception -ForegroundColor Red
    exit
}