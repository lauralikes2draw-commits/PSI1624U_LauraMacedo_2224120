@echo off
echo Verificando linhas principais da logica de avaliacao...
findstr /S /N /C:"AtualizarMarcacoesFinalizadasParaAvaliacao" ClienteRepository.cs
findstr /S /N /C:"DECIMAL(3,2)" ClienteRepository.cs
findstr /S /N /C:"NormalizarNota" AvaliacoesCliente.cs
echo.
echo Limpando cache do Visual Studio...
for /d %%d in (.vs bin obj) do if exist "%%d" rmdir /s /q "%%d"
echo Concluido. Abra ProjetoFinal.sln e rode Clean/Rebuild.
pause
