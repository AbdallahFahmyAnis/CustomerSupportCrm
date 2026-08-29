#Requires -Version 5.1
<#
.SYNOPSIS
  Check that CRM dev ports are listening before UAT/demo.

.DESCRIPTION
  Run after scripts/dev.ps1 (or your own service starts). Reports which
  required endpoints are up. Use http://localhost:5000 — not shell :4200 alone —
  so Native Federation remotes (portal-mfe :4202, etc.) load through the gateway.
#>
$ErrorActionPreference = "Continue"

$checks = @(
  @{ Name = "Gateway (public edge)"; Url = "http://localhost:5000/health" }
  @{ Name = "portal-mfe remote"; Url = "http://localhost:5000/mfe/portal/remoteEntry.json" }
  @{ Name = "agent-mfe remote"; Url = "http://localhost:5000/mfe/agent/remoteEntry.json" }
  @{ Name = "admin-mfe remote"; Url = "http://localhost:5000/mfe/admin/remoteEntry.json" }
)

$ok = 0
foreach ($c in $checks) {
  try {
    $r = Invoke-WebRequest -Uri $c.Url -UseBasicParsing -TimeoutSec 5
    if ($r.StatusCode -ge 200 -and $r.StatusCode -lt 400) {
      Write-Host "[OK]   $($c.Name) — $($c.Url)"
      $ok++
    } else {
      Write-Host "[FAIL] $($c.Name) — HTTP $($r.StatusCode)"
    }
  } catch {
    Write-Host "[FAIL] $($c.Name) — $($_.Exception.Message)"
  }
}

Write-Host ""
if ($ok -eq $checks.Count) {
  Write-Host "All $($checks.Count) checks passed. Open http://localhost:5000"
  exit 0
}

Write-Host "$ok / $($checks.Count) checks passed."
Write-Host "Start the full stack: .\scripts\dev.ps1"
Write-Host "If portal routes fail, ensure portal-mfe is running on :4202 (included in dev.ps1)."
exit 1
