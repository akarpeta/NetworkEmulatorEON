@echo off

cd "C:\Users\CableCloud\bin\Debug"
start CableCloud.exe
TIMEOUT /T 2
cd "C:\Users\Node\bin\Debug"
start Node.exe
TIMEOUT /T 3
cd "C:\Users\SubNetwork\bin\Debug"
start SubNetwork.exe
TIMEOUT /T 3
cd "C:\Users\EON20\NCC\bin\Debug"
start NCC.exe
TIMEOUT /T 3
cd "C:\Users\Host\bin\Debug"
start Host.exe
TIMEOUT /T 3
