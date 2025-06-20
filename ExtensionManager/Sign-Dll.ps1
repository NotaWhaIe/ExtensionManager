# Sign-Dll.ps1 — скрипт для подписи ExtensionManager.dll

param(
    [string]$DllPath   = "$env:USERPROFILE\source\repos\ExtensionManager\ExtensionManager\bin\Debug\ExtensionManager.dll",
    [string]$PfxPath   = "$env:USERPROFILE\source\repos\ExtensionManager\ExtensionManager\Sign-Cert.pfx",
    [string]$Password  = "YOUR_PASSWORD",
    [string]$Signtool  = "$env:USERPROFILE\source\repos\ExtensionManager\ExtensionManager\SignTool-10.0.22621.6-x64\signtool.exe",
    [string]$TimestampUrl = "http://timestamp.digicert.com"
)

# 1. Проверка наличия DLL
if (-not (Test-Path $DllPath)) {
    Write-Error "DLL не найдена по пути: $DllPath"
    exit 1
}

# 2. Проверка наличия утилиты SignTool
if (-not (Test-Path $Signtool)) {
    Write-Error "SignTool не найден по пути: $Signtool"
    exit 1
}

# 3. Подпись файла
& $Signtool sign /fd SHA256 /f $PfxPath /p $Password /tr $TimestampUrl /td SHA256 $DllPath

# 4. Проверка подписи
$signature = Get-AuthenticodeSignature -FilePath $DllPath
# Статус подписи:
Write-Host "Signature status:" $signature.Status

if ($signature.Status -ne 'Valid') {
    # Подпись невалидна:
    Write-Error "Error signature: $($signature.Status)"
    exit 1
}
