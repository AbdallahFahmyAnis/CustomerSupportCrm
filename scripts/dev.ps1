#Requires -Version 5.1
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$npm = if (Test-Path "$env:ProgramFiles\nodejs\npm.cmd") { "$env:ProgramFiles\nodejs\npm.cmd" } else { "npm.cmd" }

function Start-LoggedProcess([string]$Title, [string]$File, [string]$Arguments, [string]$WorkingDirectory) {
  Write-Host "Starting $Title"
  Start-Process -FilePath $File -ArgumentList $Arguments -WorkingDirectory $WorkingDirectory -WindowStyle Minimized
}

# .NET APIs
Start-LoggedProcess "Identity :5101" "dotnet" "run --project src\services\identity\Crm.Identity.Api\Crm.Identity.Api.csproj --urls http://localhost:5101" $root
Start-LoggedProcess "Customers :5102" "dotnet" "run --project src\services\customers\Crm.Customers.Api\Crm.Customers.Api.csproj --urls http://localhost:5102" $root
Start-LoggedProcess "Tickets :5103" "dotnet" "run --project src\services\tickets\Crm.Tickets.Api\Crm.Tickets.Api.csproj --urls http://localhost:5103" $root
Start-LoggedProcess "Knowledge :5104" "dotnet" "run --project src\services\knowledge\Crm.Knowledge.Api\Crm.Knowledge.Api.csproj --urls http://localhost:5104" $root
Start-LoggedProcess "SLA :5105" "dotnet" "run --project src\services\sla\Crm.Sla.Api\Crm.Sla.Api.csproj --urls http://localhost:5105" $root

# NestJS
Start-LoggedProcess "Channels :5201" $npm "start" (Join-Path $root "src\services\channels")
Start-LoggedProcess "Notifications :5202" $npm "start" (Join-Path $root "src\services\notifications")
Start-LoggedProcess "AI :5203" $npm "start" (Join-Path $root "src\services\ai")

# Angular MFEs + shell
Start-LoggedProcess "agent-mfe :4201" $npm "run start:agent" (Join-Path $root "src\frontend")
Start-LoggedProcess "portal-mfe :4202" $npm "run start:portal" (Join-Path $root "src\frontend")
Start-LoggedProcess "admin-mfe :4203" $npm "run start:admin" (Join-Path $root "src\frontend")
Start-LoggedProcess "knowledge-mfe :4204" $npm "run start:knowledge" (Join-Path $root "src\frontend")
Start-LoggedProcess "shell :4200" $npm "start" (Join-Path $root "src\frontend")

Start-Sleep -Seconds 12
Start-LoggedProcess "Gateway :5000" "dotnet" "run --project src\gateway\Crm.Gateway\Crm.Gateway.csproj --urls http://localhost:5000" $root

Write-Host ""
Write-Host "Customer Support CRM is starting (full stack for UAT)."
Write-Host "Open http://localhost:5000"
Write-Host "Agent:  agent@crm.local / Crm!123"
Write-Host "Admin:  admin@crm.local / Crm!123"
Write-Host "Health: http://localhost:5000/health"
Write-Host "UAT:    docs/uat-scenario.md"
