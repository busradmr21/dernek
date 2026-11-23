@echo off
setlocal EnableDelayedExpansion
rem Betigin bulundugu klasoru proje koku olarak kullanir.
set "PROJE_KOKU=%~dp0"
set "PROJE_DOSYASI=%PROJE_KOKU%SigortaDernegi\SigortaDernegi.csproj"
set "PORT_HTTP=5044"
set "PORT_HTTPS=7002"

where dotnet >nul 2>&1
if errorlevel 1 (
    echo HATA: .NET SDK bulunamadi. Once .NET 8 SDK kurulumunu tamamlayin.
    pause
    exit /b 1
)

if not exist "%PROJE_DOSYASI%" (
    echo HATA: Proje dosyasi bulunamadi:
    echo %PROJE_DOSYASI%
    pause
    exit /b 1
)

pushd "%PROJE_KOKU%"

rem Yerel Wi-Fi / Ethernet IPv4 adresini bul
for /f "tokens=*" %%a in ('powershell -NoProfile -Command "(Get-NetIPAddress -AddressFamily IPv4 | Where-Object { $_.InterfaceAlias -notlike '*Loopback*' -and $_.IPAddress -notlike '169.254*' -and $_.InterfaceAlias -like '*Wi-Fi*' } | Select-Object -ExpandProperty IPAddress -First 1)"') do set "YEREL_IP=%%a"

if "%YEREL_IP%"=="" (
    for /f "tokens=*" %%a in ('powershell -NoProfile -Command "(Get-NetIPAddress -AddressFamily IPv4 | Where-Object { $_.InterfaceAlias -notlike '*Loopback*' -and $_.IPAddress -notlike '169.254*' } | Select-Object -ExpandProperty IPAddress -First 1)"') do set "YEREL_IP=%%a"
)

echo ======================================================================
echo                  ELSADER WEB VE YONETIM SUNUCUSU
echo ======================================================================
echo Bu bilgisayardan erisim:
echo   Ana Sayfa       : http://localhost:%PORT_HTTP%
echo   Admin Paneli    : http://localhost:%PORT_HTTP%/Admin/Dashboard
echo.
echo Ayni Wi-Fi agindaki diger cihazlar icin erisim linkleri:
echo   Ana Sayfa       : http://%YEREL_IP%:%PORT_HTTP%
echo   Admin Paneli    : http://%YEREL_IP%:%PORT_HTTP%/Admin/Dashboard
echo ======================================================================
echo.
echo Uygulamayi durdurmak icin bu pencerede Ctrl+C tuslarina basin.
echo.

rem Varsayilan tarayiciyi baslatir
start "" powershell.exe -NoProfile -WindowStyle Hidden -Command "Start-Sleep -Seconds 4; Start-Process 'http://localhost:%PORT_HTTP%'"

rem Kestrel'in dis agdan da erisilebilmesi icin 0.0.0.0 baglantisiyla baslatilir
set "ASPNETCORE_ENVIRONMENT=Development"
set "ASPNETCORE_URLS=http://0.0.0.0:%PORT_HTTP%;https://0.0.0.0:%PORT_HTTPS%"
dotnet run --configuration LocalRun --project "%PROJE_DOSYASI%" --no-launch-profile
set "CIKIS_KODU=%ERRORLEVEL%"

popd

if not "%CIKIS_KODU%"=="0" (
    echo.
    echo Uygulama bir hatayla sonlandi. Hata kodu: %CIKIS_KODU%
    pause
)

exit /b %CIKIS_KODU%
