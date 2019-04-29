rem
rem This file should be run on Windows, in the directory
rem
rem Double-clicking on the file while in Windows Explorer should
rem be sufficient

@echo off
echo.
echo This batch file will copy the missing textures from the inaccessable
echo Squad/zDeprecated directory 
echo.
echo.
pause

mkdir Squad
cd Squad
mkdir linearRCS_v1
copy  ..\..\Squad\zDeprecated\Parts\Utility\linearRCS_v1\model.mu linearRCS_v1
copy  ..\..\Squad\zDeprecated\Parts\Utility\linearRCS_v1\rcs.dds linearRCS_v1




echo.
echo.
echo The files have been copied
echo.
pause
