# Danucite360 — deploy to Azure (App Service F1 + Azure SQL free offer).
# Usage:  .\deploy\azure-deploy.ps1            (first run creates everything, later runs only redeploy code)
# Requires: az login, .NET SDK. Secrets are generated here and stored only in Azure + a local file outside the repo.
param(
    [string]$ResourceGroup = "rg-danucite360",
    [string[]]$Locations = @("swedencentral", "germanywestcentral", "polandcentral", "francecentral", "italynorth", "uksouth", "westeurope", "northeurope"),
    [string]$AppName = "danucite360",
    [string]$Sku = "F1"
)
# Continue: az writes warnings to stderr, which PS 5.1 would treat as fatal; every call checks $LASTEXITCODE instead.
$ErrorActionPreference = "Continue"
$repo = Split-Path $PSScriptRoot -Parent
$secretsFile = Join-Path $env:USERPROFILE "danucite360-azure-secrets.txt"

function New-Password { -join ((48..57) + (65..90) + (97..122) | Get-Random -Count 20 | ForEach-Object { [char]$_ }) + "!aA9" }
# Never echo arguments in errors: they can contain generated passwords.
function Invoke-Az { $out = & az @args; if ($LASTEXITCODE -ne 0) { throw "az $($args[0]) $($args[1]) $($args[2]) failed" }; $out }

$appExists = (az webapp list -g $ResourceGroup --query "[?name=='$AppName'] | length(@)" -o tsv 2>$null) -eq "1"

if (-not $appExists) {
    Write-Host "== Creating SQL server + free database"
    $suffix = -join ((97..122) | Get-Random -Count 5 | ForEach-Object { [char]$_ })
    $sqlServer = "danucite360-sql-$suffix"
    $sqlUser = "danuciteadmin"
    $sqlPassword = New-Password
    $adminPassword = New-Password

    # New subscriptions are often blocked from some regions; use the first EU region that accepts a SQL server.
    $Location = $null
    foreach ($loc in $Locations) {
        & az sql server create -g $ResourceGroup -n $sqlServer -l $loc -u $sqlUser -p $sqlPassword --only-show-errors -o none 2>$null
        if ($LASTEXITCODE -eq 0) { $Location = $loc; break }
        Write-Host "   $loc not available for SQL, trying next"
    }
    if (-not $Location) { throw "No region accepted a SQL server" }
    Write-Host "   SQL server created in $Location"
    Invoke-Az sql server firewall-rule create -g $ResourceGroup -s $sqlServer -n AllowAzureServices --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0 -o none
    Invoke-Az sql db create -g $ResourceGroup -s $sqlServer -n Danucite360 -e GeneralPurpose -f Gen5 -c 2 --compute-model Serverless `
        --use-free-limit --free-limit-exhaustion-behavior AutoPause --backup-storage-redundancy Local -o none

    Write-Host "== Creating App Service plan ($Sku) + web app"
    Invoke-Az appservice plan create -g $ResourceGroup -n "$AppName-plan" --sku $Sku --is-linux -l $Location -o none
    Invoke-Az webapp create -g $ResourceGroup -p "$AppName-plan" -n $AppName --runtime "DOTNETCORE:10.0" -o none

    $conn = "Server=tcp:$sqlServer.database.windows.net,1433;Database=Danucite360;User ID=$sqlUser;Password=$sqlPassword;Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;"
    Invoke-Az webapp config connection-string set -g $ResourceGroup -n $AppName -t SQLAzure --settings "DefaultConnection=$conn" -o none
    Invoke-Az webapp config appsettings set -g $ResourceGroup -n $AppName --settings "Seed__AdminPassword=$adminPassword" "ASPNETCORE_ENVIRONMENT=Production" -o none
    Invoke-Az webapp update -g $ResourceGroup -n $AppName --https-only true -o none

    @"
Danucite360 Azure secrets (created $(Get-Date -Format s)) — keep private, do not commit.
Site admin login:  admin@danucite360.local / $adminPassword
SQL server:        $sqlServer.database.windows.net  user $sqlUser / $sqlPassword
"@ | Set-Content -Path $secretsFile -Encoding utf8
    Write-Host "Secrets saved to $secretsFile"
}

# Linux .NET image needs to be told which assembly to run.
Invoke-Az webapp config set -g $ResourceGroup -n $AppName --startup-file "dotnet Danucite360.Web.dll" -o none

Write-Host "== Publishing app"
$out = Join-Path $env:TEMP "danucite360-publish"
$zip = Join-Path $env:TEMP "danucite360.zip"
Remove-Item $out, $zip -Recurse -Force -ErrorAction SilentlyContinue
dotnet publish (Join-Path $repo "Danucite360\Danucite360.Web.csproj") -c Release -o $out
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed" }
# Production gets its connection string from Azure, not from the local appsettings.json.
Remove-Item (Join-Path $out "appsettings.Development.json") -ErrorAction SilentlyContinue
# Keep the upload small (slow links reset big uploads): no source maps, debug symbols or Windows-only native libs on a Linux host.
Get-ChildItem $out -Recurse -File -Include *.map, *.map.gz, *.map.br, *.pdb | Remove-Item -Force
Get-ChildItem (Join-Path $out "runtimes") -Directory -Filter "win*" -ErrorAction SilentlyContinue | Remove-Item -Recurse -Force
# Windows tar writes forward-slash zip entries; PS 5.1 Compress-Archive writes backslashes that Linux Kudu rejects.
tar -a -c -f $zip -C $out .
if ($LASTEXITCODE -ne 0) { throw "zip failed" }
Invoke-Az webapp deploy -g $ResourceGroup -n $AppName --src-path $zip --type zip -o none

Write-Host "== Done: https://$AppName.azurewebsites.net"
