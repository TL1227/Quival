@echo off

call :DeleteFolder "QuivalServer\bin\Debug"
call :DeleteFolder "QuivalCombatTestWPF\bin\Debug"

echo Done.
exit /b

:DeleteFolder
if exist "%~1" (
    echo Deleting %~1...
    rd /S /Q "%~1"
) else (
    echo "%~1" does not exist.
)
exit /b