@echo off
chcp 65001 >nul
echo ===============================================
echo VERIFICACAO DA V5 - FORMS NOVA MARCACAO
echo ===============================================
echo.
echo Pasta atual:
echo %cd%
echo.
echo Procurando a linha antiga que causava erro:
findstr /S /N /C:"fechar.Click += delegate" NovaMarcacao*.cs NovaMarcacao*.Designer.cs
if %errorlevel%==0 (
  echo.
  echo [ATENCAO] A linha antiga ainda existe nesta pasta.
  echo Voce nao esta usando a V5 limpa ou algum arquivo antigo foi misturado.
) else (
  echo.
  echo [OK] A linha antiga NAO existe nos forms NovaMarcacao da V5.
)
echo.
echo Limpando cache local bin/obj/.vs, se existir...
if exist bin rmdir /s /q bin
if exist obj rmdir /s /q obj
if exist .vs rmdir /s /q .vs
echo.
echo Agora abra o arquivo ProjetoFinal.sln desta mesma pasta.
pause
