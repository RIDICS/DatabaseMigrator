#!/usr/bin/env pwsh

    [CmdletBinding()]
Param(
    [Parameter(Mandatory = $true)]
    [String]$apiKey,

    [Parameter(Mandatory = $true)]
    [String]$packageVersion,

    [Parameter()]
    [Switch]$disableRestore = $false
)

$currentPath = (Get-Location -PSProvider FileSystem).ProviderPath

$packages = New-Object System.Collections.ArrayList
$packages.Add("Ridics.DatabaseMigrator.Core") > $null
$packages.Add("Ridics.DatabaseMigrator.QueryBuilder") > $null
$packages.Add("Ridics.DatabaseMigrator.Shared") > $null

if ($packageVersion[0] -eq 'v')
{
    $packageVersion = $packageVersion.Substring(1)
}

Set-Location Solution

if (!$disableRestore)
{
    dotnet restore
}

# `dotnet build` must run before `dotnet publish` because GeneratePackageOnBuild in csproj forces not to build when running `dotnet publish` command
# https://github.com/dotnet/core/issues/1778
dotnet build --no-restore -c Release "/property:Version=${packageVersion}"

dotnet publish --no-restore -c Release --no-build

Set-Location $currentPath
Set-Location build

foreach ($package in $packages)
{
    $packageFullName = "${package}.${packageVersion}.nupkg"

    Write-Host "Publishing ${packageFullName}"
    dotnet nuget push --source nuget.org -k "${apiKey}" "${packageFullName}"
}

Set-Location $currentPath
