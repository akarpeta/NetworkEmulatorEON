@echo off
cls
taskkill /IM Node.exe /F
taskkill /IM SubNetwork.exe /F
taskkill /IM Host.exe /F
taskkill /IM NCC.exe /F
taskkill /IM CableCloud.exe /F
exit

