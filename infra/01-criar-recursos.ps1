[CmdletBinding()]
param(
 [string]$Location = "brazilsouth",
 [ValidateSet("F1", "B1")][string]$PlanSku = "F1",
 [string]$SubscriptionId,
 [string]$ClientIp
)
$ErrorActionPreference = "Stop"
function Az {
 param([Parameter(ValueFromRemainingArguments=$true)][string[]]$Arguments)
 $result = & az @Arguments
 if ($LASTEXITCODE -ne 0) { throw "Comando Azure CLI falhou. Corrija a mensagem anterior antes de continuar." }
 return $result
}
if (-not (Get-Command az -ErrorAction SilentlyContinue)) { throw "Instale Azure CLI e reabra o PowerShell: https://aka.ms/installazurecliwindows" }
Az account show --output none
if ($SubscriptionId) { Az account set --subscription $SubscriptionId }
$configPath = Join-Path $PSScriptRoot "recursos.local.json"
if (Test-Path $configPath) {
 $cfg = Get-Content $configPath -Raw | ConvertFrom-Json
 Write-Host "Reutilizando nomes de recursos do arquivo local."
} else {
 $suffix = [Guid]::NewGuid().ToString('N').Substring(0,8)
 $cfg = [pscustomobject]@{
  resourceGroup = "rg-dimdim-555125-$suffix"; location = $Location
  sqlServer = "sql-dimdim-555125-$suffix"; database = "dimdimdb"; sqlUser = "dimdimadmin"
  plan = "plan-dimdim-555125-$suffix"; webApp = "app-dimdim-555125-$suffix"
 }
 $cfg | ConvertTo-Json | Set-Content $configPath -Encoding UTF8
}
Write-Host "Assinatura:"
Az account show --query '{name:name,id:id}' --output table
Write-Host "Serao criados Azure SQL Basic (pago) e App Service $PlanSku na regiao $($cfg.location)."
Write-Host "Acompanhe o custo no Azure; B1 tambem e pago. O script nao altera recursos antigos."
$passwordSecure = Read-Host "Crie senha SQL forte (16+ caracteres, maiuscula/minuscula/numero/simbolo; guarde para o Query Editor)" -AsSecureString
$ptr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($passwordSecure)
try {
 $password = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($ptr)
 if ($password.Length -lt 16) { throw "Use senha SQL de pelo menos 16 caracteres." }
 Az group create --name $cfg.resourceGroup --location $cfg.location --output none
 Az sql server create --name $cfg.sqlServer --resource-group $cfg.resourceGroup --location $cfg.location --admin-user $cfg.sqlUser --admin-password $password --output none
 Az sql db create --resource-group $cfg.resourceGroup --server $cfg.sqlServer --name $cfg.database --service-objective Basic --backup-storage-redundancy Local --output none
 Az appservice plan create --name $cfg.plan --resource-group $cfg.resourceGroup --location $cfg.location --sku $PlanSku --is-linux --output none
 Az webapp create --name $cfg.webApp --resource-group $cfg.resourceGroup --plan $cfg.plan --runtime 'DOTNETCORE:8.0' --output none
 Az webapp update --name $cfg.webApp --resource-group $cfg.resourceGroup --https-only true --output none
 Az webapp config set --name $cfg.webApp --resource-group $cfg.resourceGroup --startup-file 'dotnet DimDim.Api.dll' --output none
 $builder = New-Object System.Data.SqlClient.SqlConnectionStringBuilder
 $builder['Data Source'] = "tcp:$($cfg.sqlServer).database.windows.net,1433"
 $builder['Initial Catalog'] = $cfg.database
 $builder['User ID'] = $cfg.sqlUser
 $builder['Password'] = $password
 $builder['Encrypt'] = $true
 $builder['TrustServerCertificate'] = $false
 $builder['Connect Timeout'] = 30
 $connection = $builder.ConnectionString
 Az webapp config appsettings set --name $cfg.webApp --resource-group $cfg.resourceGroup --settings "ConnectionStrings__DimDim=$connection" 'DatabaseProvider=SqlServer' 'ASPNETCORE_ENVIRONMENT=Production' --output none
 $outbound = Az webapp show --name $cfg.webApp --resource-group $cfg.resourceGroup --query outboundIpAddresses --output tsv
 if (-not $outbound) { throw "Nao foi possivel obter IPs de saida do App Service." }
 $index = 1
 foreach ($ip in ($outbound.Trim().Split(','))) {
  Az sql server firewall-rule create --resource-group $cfg.resourceGroup --server $cfg.sqlServer --name "AppService-$index" --start-ip-address $ip --end-ip-address $ip --output none
  $index++
 }
 if (-not $ClientIp) { $ClientIp = (Invoke-RestMethod 'https://api.ipify.org').Trim() }
 $parsedIp = $null
 if (-not [Net.IPAddress]::TryParse($ClientIp, [ref]$parsedIp)) { throw "IP invalido. Rode novamente com -ClientIp SEU_IP_PUBLICO." }
 Az sql server firewall-rule create --resource-group $cfg.resourceGroup --server $cfg.sqlServer --name SeuComputador --start-ip-address $ClientIp --end-ip-address $ClientIp --output none
 Write-Host "Infraestrutura criada. Ainda falta publicar a API pela Release."
 Write-Host "Web App: $($cfg.webApp)"
 Write-Host "URL: https://$($cfg.webApp).azurewebsites.net"
 Write-Host "SQL: $($cfg.sqlServer).database.windows.net / $($cfg.database) / $($cfg.sqlUser)"
 Write-Host "Nomes salvos em infra/recursos.local.json, sem a senha. Este arquivo e ignorado pelo Git."
} finally {
 [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($ptr)
 $password = $null; $connection = $null; $builder = $null
}
