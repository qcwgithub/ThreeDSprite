rmdir .\gameConfig /Q/S
mkdir .\gameConfig
xcopy ..\Assets\Resources\Imported .\gameConfig\Imported\ /E/H

xcopy ..\Assets\Configs .\gameConfig\ /E/H