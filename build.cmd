@echo off
setlocal enabledelayedexpansion

set BatchFile=%0
set Root=%~dp0
set BuildConfiguration=Debug
set MSBuildTarget=Build
set NodeReuse=true
set DeveloperCommandPrompt=%VS150COMNTOOLS%\VsDevCmd.bat
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

if not exist "%VS150COMNTOOLS%" (
  echo To build this repository, this script needs to be run from a Visual Studio 2017 developer command prompt.
  echo.
  echo If Visual Studio is not installed, visit this page to download:
  echo.
  echo https://www.visualstudio.com/vs/
  exit /b 1
)

if "%VisualStudioVersion%" == "" (
  REM If MSBuild wasn't run from a developer command prompt, but the right VS150COMNTOOLS is set, run the bat.
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