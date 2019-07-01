#!/usr/bin/env pwsh

    [CmdletBinding()]
Param(
    [Parameter(Mandatory = $true)]
    [String]$Environment
)

$CurrentPath = Split-Path -parent $MyInvocation.MyCommand.Definition

Write-Host $CurrentPath

$env:ASPNETCORE_ENVIRONMENT = "${Environment}"

Write-Host "Start migration"

dotnet (Join-Path $CurrentPath Ridics.DatabaseMigrator.SampleApp.dll) --no-interactive

Write-Host "Migration finished"


