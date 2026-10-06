@echo off
setlocal
rem Betigin bulundugu klasoru proje koku olarak kullanir.
set "PROJE_KOKU=%~dp0"
set "PROJE_DOSYASI=%PROJE_KOKU%SigortaDernegi\SigortaDernegi.csproj"
set "UYGULAMA_ADRESI=https://localhost:7002"

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

echo Sigorta Dernegi uygulamasi baslatiliyor...
echo Tarayici adresi: %UYGULAMA_ADRESI%
echo Uygulamayi durdurmak icin bu pencerede Ctrl+C tuslarina basin.
echo.

rem Sunucunun acilmasi icin kisa sure bekleyip varsayilan tarayiciyi baslatir.
start "" powershell.exe -NoProfile -WindowStyle Hidden -Command "Start-Sleep -Seconds 5; Start-Process '%UYGULAMA_ADRESI%'"

dotnet run --project "%PROJE_DOSYASI%" --launch-profile https
set "CIKIS_KODU=%ERRORLEVEL%"

popd

if not "%CIKIS_KODU%"=="0" (
    echo.
    echo Uygulama bir hatayla sonlandi. Hata kodu: %CIKIS_KODU%
    pause
)

exit /b %CIKIS_KODU%
