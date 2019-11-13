@echo off
::可以通过环境变量来设置路径\
rem 先关闭掉unity进程
taskkill /F /IM unity* /T 

rem 注意变量和=之间不要有空格，很坑，搞了好久
set UNITY_PATH="D:\app\UnityPackage\2018.3.14f1\Editor\Unity.exe"  
set UNITY_LOG=%cd%\unity_pc_log.txt
set PROJECT_PATH="E:\unity_project\AssetTool\AssetTool"
set EXPORT_PATH=%cd%\ExportPath rem 导出的项目路径，用来打apk
set BUILD_NAME=AssetToolDemo.exe
set BUILD_DIR=E:\unity_project\AssetTool\AssetTool\Build
set BUILD_FILE=%BUILD_DIR%\%BUILD_NAME%
set PYTHON_FILE=%cd%\pythonFile

set WINDOW_AB_PATH=E:\unity_project\AssetTool\AssetTool\AssetBundles\Window

echo 开始导出项目
echo -------------

python %PYTHON_FILE%\build_postprocess.py -package %BUILD_FILE% -platform window -updatePath %WINDOW_AB_PATH%
pause 
python %PYTHON_FILE%\ExportProj.py -srcProj %PROJECT_PATH% -destProj %EXPORT_PATH% -platform window

echo 导出项目完成
echo -------------
pause

rem 清理导出目录
if exist %BUILD_DIR% rd /s /q %BUILD_DIR%
md %BUILD_DIR% 

echo 开始打包
%UNITY_PATH% -batchMode -nographics -quit -projectPath %EXPORT_PATH% -logFile %UNITY_LOG% -executeMethod BuildTool.BuildPCPackage BUILD_PATH=%BUILD_FILE%
if not "%ERRORLEVEL%" == "0" goto FAIL rem 需要-quit，命令才结束
echo success

echo 拷贝ab文件 
echo ------------
python %PYTHON_FILE%\build_postprocess.py -package %BUILD_FILE% -platform window -updatePath %WINDOW_AB_PATH%
goto end 

:FAIL 
echo 打包失败

:end 
echo 结束
pause