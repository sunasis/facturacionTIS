@echo off

SET CSCPATH=%SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319

if not exist ".\nuget.exe" powershell -Command "(new-object System.Net.WebClient).DownloadFile('https://dist.nuget.org/win-x86-commandline/latest/nuget.exe', '.\nuget.exe')"
.\nuget.exe pack FacturacionElectronica.GeneradorXml/FacturacionElectronica.GeneradorXml.csproj -properties Configuration=Release
.\nuget.exe pack FacturacionElectronica.Homologacion/FacturacionElectronica.Homologacion.csproj -properties Configuration=Release
.\nuget.exe pack FacturacionElectronica.Validador/FacturacionElectronica.Validador.csproj -properties Configuration=Release
.\nuget.exe pack Gs.Ubl/Gs.Ubl.csproj -properties Configuration=Release