rmdir package /s /q
mkdir package

xcopy /I /Y *.exe package
xcopy /I /Y *.dll package
xcopy /I /Y /S x64 "package\x64"
xcopy /I /Y /S x86 "package\x86"
del ".\package\VersionInfo.exe"

VersionInfo.exe FMStudio.exe > VERSION.txt
set /P VERSION=<VERSION.txt

nuget pack FMStudio.nuspec -Version %VERSION% -Properties Configuration=Release -OutputDirectory . -BasePath .
..\..\packages\squirrel.windows.1.4.0\tools\squirrel --releasify FMStudio.%VERSION%.nupkg -r installer