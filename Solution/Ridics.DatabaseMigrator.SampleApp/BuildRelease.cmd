@ECHO OFF

SET PROJ_DIR=%~dp0
echo Using project directory: %PROJ_DIR%

RD /S /Q "%PROJ_DIR%build\Migrator-build"

dotnet publish "%PROJ_DIR%Ridics.DatabaseMigrator.SampleApp.csproj" --configuration Release --output "%PROJ_DIR%/build/Migrator-build/"
