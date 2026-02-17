@echo off
setlocal enabledelayedexpansion
cd /d "%~dp0.."

echo [1/2] Publishing Windows x64...
dotnet publish src/LicenseGenerator/LicenseGenerator.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
set WIN_PUB=src\LicenseGenerator\bin\Release\net8.0\win-x64\publish
if exist "%WIN_PUB%\LicenseGenerator.exe" (
  if not exist "dist\Windows" mkdir "dist\Windows"
  copy "%WIN_PUB%\LicenseGenerator.exe" "dist\Windows\LicenseGenerator.exe" /Y
)

echo [2/2] Publishing Linux x64...
dotnet publish src/LicenseGenerator/LicenseGenerator.csproj -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
set LINUX_PUB=src\LicenseGenerator\bin\Release\net8.0\linux-x64\publish
if exist "%LINUX_PUB%\LicenseGenerator" (
  if not exist "dist\Linux" mkdir "dist\Linux"
  copy "%LINUX_PUB%\LicenseGenerator" "dist\Linux\LicenseGenerator-linux" /Y
)

echo Done.
pause
