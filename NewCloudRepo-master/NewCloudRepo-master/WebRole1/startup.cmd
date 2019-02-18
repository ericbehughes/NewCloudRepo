
REM ***** Exit script if running in Emulator *****
if "%ComputeEmulatorRunning%"=="true" goto exit

echo Installing "IPv4 Address and Domain Restrictions" feature 
powershell -ExecutionPolicy Unrestricted -command "Install-WindowsFeature Web-IP-Security"
echo Unlocking configuration for "IPv4 Address and Domain Restrictions" feature 
%windir%\system32\inetsrv\AppCmd.exe unlock config -section:system.webServer/security/ipSecurity

icacls %windir%\System32\inetsrv\Config\redirection.config /grant IIS_IUSRS:F
icacls %windir%\System32\inetsrv\Config\administration.config /grant IIS_IUSRS:F
icacls %windir%	\System32\inetsrv\Config\applicationHost.config /grant IIS_IUSRS:F
icacls Web.Config /grant IIS_IUSR:F

EXIT /B 0