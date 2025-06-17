@echo off

echo Generating executable.

dotnet publish -c Release -r win-x64 ^
-p:PublishSingleFile=true ^
--self-contained true ^
-p:IncludeNativeLibrariesForSelfExtract=false ^
-p:EnableCompressionInSingleFile=true ^
-p:DebugType=None -p:DebugSymbols=false ^
-o "%CD%\bin"

echo Completed.
echo The exectuable is on: %CD%\bin

pause
