@echo off
REM ==== Lanzador de MisFinanzas: construye el front, lo copia y corre el backend ====

REM 1) Pararse en la carpeta de este script (la raiz del repo)
cd /d "%~dp0"

echo.
echo === 1/3  Construyendo el frontend (ng build) ===
cd frontend
call ng build
if errorlevel 1 ( echo. & echo Fallo el ng build. Revisa el error de arriba. & pause & exit /b 1 )
cd ..

echo.
echo === 2/3  Copiando el build a wwwroot ===
if exist "src\MisFinanzas.API\wwwroot" rmdir /s /q "src\MisFinanzas.API\wwwroot"
mkdir "src\MisFinanzas.API\wwwroot"
xcopy /e /i /y "frontend\dist\frontend\browser\*" "src\MisFinanzas.API\wwwroot\" >nul

echo.
echo === 3/3  Iniciando el backend  (Ctrl+C para detener) ===
echo Abre en el navegador la URL que diga "Now listening on:"
dotnet run --project src\MisFinanzas.API