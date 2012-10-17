; -- Mosaic.iss --

;   CURRENTLY THIS IS NOT USED  !!!!!!!!!!!!!!!!!!!!!

[Setup]
AppName=Mosaic
AppVerName=Mosaic-1740
AppVersion=1.0.1740
DefaultDirName={pf}\Gray Institute\Mosaic
OutputDir=Setup\InnoSetup
MinVersion=0,5.0
OutputBaseFilename=MosaicSetup
VersionInfoCompany=Gray Institute
VersionInfoCopyright=2011 Gray Institute, Oxford

[Files]
Source: "Setup\MosaicSetup.msi"; DestDir: "{tmp}"; Flags: ignoreversion
Source: "Setup\setup.exe"; DestDir: "{tmp}"; Flags: ignoreversion
;Source: "vcredist\vcredist_2005_x86.exe"; DestDir: "{tmp}"; Flags: ignoreversion
Source: "vcredist\vcredist_2008_x86.exe"; DestDir: "{tmp}"; Flags: ignoreversion recursesubdirs; OnlyBelowVersion: 0,6.01

[Code]
function ShouldInstallVCRedist2005: Boolean;
begin
     Result := Not RegValueExists(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\DevDiv\VC\Servicing\8.0\RED\1033', 'Install')
end;

function ShouldInstallVCRedist2008: Boolean;
begin
     Result := Not RegValueExists(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\DevDiv\VC\Servicing\9.0\RED\1033', 'Install')
end;

function ShouldInstallNet4ClientFramework: Boolean;
begin
     Result := Not RegValueExists(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client', 'Install')
end;


[Run]
;To make the MSI install quietly also you need:
;vcredist_x86.exe /Q:a /c:"msiexec.exe /qn /i vcredist.msi"

;Filename: "{tmp}\vcredist_2005_x86.exe"; Parameters: "/Q:a"; OnlyBelowVersion: 0,6.01; Check: ShouldInstallVCRedist2005;
Filename: "{tmp}\vcredist_2008_x86.exe";  Parameters: "/q:a"; OnlyBelowVersion: 0,6.01; Check: ShouldInstallVCRedist2008;

Filename: "{tmp}\setup.exe";
