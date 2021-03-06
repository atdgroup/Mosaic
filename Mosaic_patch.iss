; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Mosaic_Patch"
#define MyAppVersion "1.7"
#define MyAppPublisher "UCL CI"
#define MyAppExeName "Mosaic.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{52605101-F5D4-428F-AC75-639D464A06C0}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\Gray Institute\Mosaic
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
LicenseFile=C:\Users\Paul\Documents\Devel\ATD\Mosaic\GPL License.txt
OutputDir=C:\Users\Paul\Documents\Devel\ATD\Mosaic
OutputBaseFilename=Mosaic_patch
SetupIconFile=C:\Users\Paul\Documents\Devel\ATD\Mosaic\Resources\Mosaic.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "C:\Users\Paul\Documents\Devel\ATD\Mosaic\bin\Mosaic.exe"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

