rmdir package /s /q
mkdir package

xcopy /I /Y *.exe package
xcopy /I /Y *.dll package
xcopy /I /Y /S x64 "package\x64"
xcopy /I /Y /S x86 "package\x86"