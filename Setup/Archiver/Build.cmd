@echo off

echo.
echo Accord.NET Framework compressed archive builder
echo =========================================================
echo. 
echo This Windows batch file uses WinRAR to automatically
echo build the compressed archive version of the framework.
echo. 


:: Settings for complete and (libs-only) package creation
:: ---------------------------------------------------------

set version=2.7.1
set rar="C:\Program Files\WinRAR\rar"
set fullname="Accord.NET Framework-%version%.rar" 
set libsname="Accord.NET Framework-%version%-(libs only).rar"
set opts=a -m5 -s


echo  - Framework version: %version%
echo  - Complete package : %fullname%     
echo  - Lib-only package : %libsname%
echo.
echo  - WinRAR Command: %rar%
echo  - WinRAR Options: "%opts%"
echo.

pause


echo.
echo.
echo Creating Accord.NET %fullname% archive
echo ---------------------------------------------------------

timeout /T 5
set output=..\bin\%fullname%
del %output%

%rar% %opts%    %output% "..\..\Copyright.txt"
%rar% %opts%    %output% "..\..\License.txt"
%rar% %opts%    %output% "..\..\Release notes.txt"
%rar% %opts%    %output% "..\..\Docs\*.chm"        -x*\.svn*
%rar% %opts% -r %output% "..\..\Release\*"         -x*\.svn* -x*.lastcodeanalysissucceeded -x*.CodeAnalysisLog.xml -x*SlimDX.pdb
%rar% %opts% -r %output% "..\..\Sources\*"         -x*\.svn* -x*\obj  -x*\bin      -x*\TestResults -x*.sdf -x*.suo -x*.user -x*.shfbproj_* -x*.vsp
%rar% %opts% -r %output% "..\..\Samples\*"         -x*\.svn* -x*\obj  -x*\bin\x64\ -x*\bin\Debug   -x*\bin\Release -x*\bin\x86\Debug -x*.pdb -x*.user -x*.vsp
%rar% %opts% -r %output% "..\..\Externals\*"       -x*\.svn* -x*.pdb
%rar% %opts% -r %output% "..\..\Setup\*"           -x*\.svn* -x*\obj  -x*\bin -x*.user
%rar% t         %output%




echo.
echo Creating Accord.NET %libsname% archive
echo ---------------------------------------------------------

timeout /T 5
set output=..\bin\%libsname%
del %output%

%rar% %opts%    %output% "..\..\Copyright.txt"
%rar% %opts%    %output% "..\..\License.txt"
%rar% %opts%    %output% "..\..\Release notes.txt"
%rar% %opts% -r %output% "..\..\Release\*"         -x*\.svn* -x*.lastcodeanalysissucceeded -x*.CodeAnalysisLog.xml -x*SlimDX.pdb
%rar% t         %output%



echo.
echo ---------------------------------------------------------
echo Package creation has completed. Please check the above
echo commands for errors and check packages in output folder.
echo ---------------------------------------------------------
echo.

pause
