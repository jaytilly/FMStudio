$nuget = ".\.nuget\NuGet.exe"
$squirrel = "packages\squirrel.windows.0.99.1.1\tools\squirrel.exe"

& $nuget pack FMStudio.nuspec

$nupkg = (gci *.nupkg)[0]

& $squirrel --releasify $nupkg
