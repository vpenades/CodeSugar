@echo off

:: ============================================= Define Version suffix

set GETTIMEKEY=powershell get-date -format "{yyyyMMdd-HHmm}"
for /f %%i in ('%GETTIMEKEY%') do set TIMEKEY=%%i

set VERSIONSUFFIX=Preview-%TIMEKEY%

echo Building 1.0.0-%VERSIONSUFFIX%

:: ============================================= DOTNET builder

dotnet build -c:Release --version-suffix %VERSIONSUFFIX% ..\CodeSugar.sln
// dotnet pack -c:Release --version-suffix %VERSIONSUFFIX% ..\CodeSugar.sln

:: ============================================= Copy output

md bin

for /r %%i in (*.*nupkg) do move %%i bin

pause
exit /b