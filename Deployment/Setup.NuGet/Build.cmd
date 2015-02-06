:: The script was taken from: Accord.NET project and modified
@echo off

echo.
echo Accord.NET Extensions Framework NuGet packages builder
echo =========================================================
echo. 
echo This Windows batch file uses NuGet to automatically
echo build the NuGet packages versions of the framework.
echo. 

timeout /T 5

:: Set version info
set version=1.2.1-rc
set output=bin\

:: Create output directory
IF NOT EXIST %output%\nul (
    mkdir %output%
)

:: Remove old files
:forfiles /p %output% /m *.nupkg /c "cmd /c del @file"


echo.
echo Creating packages...

forfiles /m *.nuspec /c "cmd /c nuget.exe pack @File -Version %version% -OutputDirectory %output%"

:eof