@echo off
::����ͨ����������������·��\
rem �ȹرյ�unity����
taskkill /F /IM unity* /T 

rem ע�������=֮�䲻Ҫ�пո񣬺ܿӣ����˺þ�
set UNITY_PATH="D:\app\UnityPackage\2018.3.14f1\Editor\Unity.exe"  
set UNITY_LOG=%cd%\unity_pc_log.txt
set PROJECT_PATH="E:\unity_project\AssetTool\AssetTool"
set EXPORT_PATH=%cd%\ExprotPath rem ��������Ŀ·����������apk
set BUILD_NAME=AssetToolDemo.exe
set BUILD_DIR=E:\unity_project\AssetTool\AssetTool\Build
set BUILD_FILE=%BUILD_DIR%\%BUILD_NAME%
set PYTHON_FILE=%cd%\pythonFile
rem ������Ŀ¼
if exist %BUILD_DIR% rd /s /q %BUILD_DIR%
md %BUILD_DIR% 

echo ��ʼ������Ŀ
echo -------------

python %PYTHON_FILE%\ExportProj.py -srcProj %PROJECT_PATH% -destProj %EXPORT_PATH% -platform window

echo ������Ŀ���
echo -------------
pause
echo ��ʼ���
%UNITY_PATH% -batchMode -nographics -quit -projectPath %EXPORT_PATH% -logFile %UNITY_LOG% -executeMethod BuildTool.BuildPCPackage BUILD_PATH=%BUILD_FILE%
if not "%ERRORLEVEL%" == "0" goto FAIL rem ��Ҫ-quit������Ž���
echo success
goto end

:FAIL 
echo ���ʧ��
goto end

:end 
echo over
pause