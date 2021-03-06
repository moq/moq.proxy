@echo off
setlocal enabledelayedexpansion

set BatchFile=%0
set Root=%~dp0
set BuildConfiguration=Debug
set MSBuildTarget=Build
set NodeReuse=true
set MultiProcessor=/m
set RunTests=true

:ParseArguments
if "%1" == "" goto :DoneParsing
if /I "%1" == "/?" call :Usage && exit /b 1
if /I "%1" == "/debug" set BuildConfiguration=Debug&&shift&& goto :ParseArguments
if /I "%1" == "/release" set BuildConfiguration=Release&&shift&& goto :ParseArguments
if /I "%1" == "/rebuild" set MSBuildTarget=Rebuild&&shift&& goto :ParseArguments
if /I "%1" == "/restore" set MSBuildTarget=Restore&&shift&& goto :ParseArguments
if /I "%1" == "/skiptests" set RunTests=false&&shift&& goto :ParseArguments
if /I "%1" == "/no-node-reuse" set NodeReuse=false&&shift&& goto :ParseArguments
if /I "%1" == "/no-multi-proc" set MultiProcessor=&&shift&& goto :ParseArguments
MSBuildAdditionalArguments="%1 %MSBuildAdditionalArguments"%&&shift&& goto :ParseArguments
:DoneParsing

:: Detect if MSBuild is in the path
for /f "delims=" %%i in ('where msbuild') do set "MSBuildPath=%%i" & goto :MSBuildPathDone
:MSBuildPathDone

if not exist "%MSBuildPath%" (
  call :PrintColor Red "To build this repository, MSBuild.exe must be in the PATH."
  echo MSBuild is included with Visual Studio 2017 or later.
  echo.
  echo If Visual Studio is not installed, visit this page to download:
  echo.
  echo https://www.visualstudio.com/vs/
  echo.
  exit /b 1
)

:: Detect MSBuild version >= 15
for /f "delims=" %%i in ('msbuild -nologo -version') do set MSBuildFullVersion=%%i
for /f "delims=. tokens=1" %%a in ("%MSBuildFullVersion%") do (
  set MSBuildMajorVersion=%%a
)

if %MSBuildMajorVersion% LSS 15 (
  call :PrintColor Red "To build this repository, the MSBuild.exe in the PATH needs to be 15.0 or higher."
  echo MSBuild 15.0 is included with Visual Studio 2017 or later.
  echo.
  echo If Visual Studio is not installed, visit this page to download:
  echo.
  echo https://www.visualstudio.com/vs/
  echo.
  echo Located MSBuild in the PATH was "%MSBuildPath%".
  exit /b 1
)

:: Ensure developer command prompt variables are set
if "%VisualStudioVersion%" == "" (
  for /f "delims=" %%i in ('msbuild -nologo /t:GetVsInstallRoot') do set "VsInstallRoot=%%i" & goto :VsInstallRootDone
:VsInstallRootDone
  for /f "tokens=* delims= " %%i in ("%VsInstallRoot%") do set "VsInstallRoot=%%i"
  set "DeveloperCommandPrompt=%VsInstallRoot%\Common7\Tools\VsDevCmd.bat"
  if not exist "%DeveloperCommandPrompt%" (
    call :PrintColor Red "Failed to locate 'Common7\Tools\VsDevCmd.bat' under the reported Visual Studio installation root '%VsInstallRoot%'."
    echo.
    echo If Visual Studio is not installed, visit this page to download:
    echo.
    echo https://www.visualstudio.com/vs/
    echo.
    exit /b 1  
  )
  call "%DeveloperCommandPrompt%" || goto :BuildFailed
)

set BinariesDirectory=%Root%bin\%BuildConfiguration%\
if not exist "%BinariesDirectory%" mkdir "%BinariesDirectory%" || goto :BuildFailed

msbuild /nologo /nodeReuse:%NodeReuse% /t:"%MSBuildTarget%" /p:Configuration="%BuildConfiguration%" /p:RunTests="%RunTests%" "%Root%corebuild.proj" %MSBuildAdditionalArguments%
if ERRORLEVEL 1 (
    echo.
    call :PrintColor Red "Build failed, for full log see msbuild.log."
    exit /b 1
)

echo.
call :PrintColor Green "Build completed successfully, for full log see msbuild.log"
exit /b 0

:Usage
echo Usage: %BatchFile% [/rebuild^|/restore^] [/debug^|/release] [/no-node-reuse] [/no-multi-proc] [/skiptests]
echo.
echo   Build targets:
echo     /rebuild                 Perform a clean, then build
echo     /restore                 Only restore NuGet packages
echo     /skiptests               Don't run unit tests
echo.
echo   Build options:
echo     /debug                   Perform debug build (default)
echo     /release                 Perform release build
echo     /no-node-reuse           Prevents MSBuild from reusing existing MSBuild instances,
echo                              useful for avoiding unexpected behavior on build machines
echo     /no-multi-proc           No multi-proc build, useful for diagnosing build logs
goto :eof

:BuildFailed
call :PrintColor Red "Build failed with ERRORLEVEL %ERRORLEVEL%"
exit /b 1

:PrintColor
"%Windir%\System32\WindowsPowerShell\v1.0\Powershell.exe" -noprofile write-host -foregroundcolor %1 "'%2'"