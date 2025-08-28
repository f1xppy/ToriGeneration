# deploy.ps1 - PowerShell script for IIS deployment
param(
    [string]$SourcePath = "Web/dist/web",
    [string]$DestinationPath = "C:\inetpub\wwwroot"
)

Write-Host "🚀 Starting IIS deployment..." -ForegroundColor Green

# Check if source directory exists
if (-not (Test-Path $SourcePath)) {
    Write-Host "❌ Source directory not found: $SourcePath" -ForegroundColor Red
    Write-Host "💡 Please run 'npm run build:iis' first" -ForegroundColor Yellow
    exit 1
}

# Check if destination directory exists
if (-not (Test-Path $DestinationPath)) {
    Write-Host "❌ IIS directory not found: $DestinationPath" -ForegroundColor Red
    Write-Host "💡 Please check IIS installation or specify correct path" -ForegroundColor Yellow
    exit 1
}

try {
    # Clean destination directory
    Write-Host "🧹 Cleaning destination directory..." -ForegroundColor Yellow
    Remove-Item "$DestinationPath\*" -Recurse -Force -ErrorAction SilentlyContinue
    
    # Copy files
    Write-Host "📦 Copying files to IIS..." -ForegroundColor Yellow
    Copy-Item "$SourcePath\*" -Destination $DestinationPath -Recurse -Force
    
    Write-Host "✅ Deployment completed successfully!" -ForegroundColor Green
    Write-Host "🌐 Open: http://localhost/" -ForegroundColor Cyan
}
catch {
    Write-Host "❌ Deployment failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
