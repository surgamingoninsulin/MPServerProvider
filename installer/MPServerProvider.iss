; Inno Setup script for MP Server Provider.
; Built by build-installer.bat, which publishes the app to installer\publish and passes
; /DAppVersion=<version from MPServerProvider.App.csproj>.
;
; Safety:
;  - The app keeps its servers/settings in a "data" folder next to the exe and runs without
;    admin rights, so it installs per-user into a writable folder by default.
;  - The uninstaller only removes files this installer put there; the "data" folder (servers,
;    mods, settings) is always left alone.
;  - Setup refuses to install into a folder that contains a source checkout (.git, *.sln or
;    _MPServerProvider) so an uninstall can never take the dev repo with it.

#ifndef AppVersion
  #define AppVersion "0.0.0"
#endif

#define AppName "MP Server Provider"
#define AppExe "MPServerProvider.App.exe"
#define PublishDir "publish"

[Setup]
AppId={{D369D6C4-C79F-499B-9CF1-46EC140318CE}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppName}
VersionInfoVersion={#AppVersion}
DefaultDirName={localappdata}\Programs\{#AppName}
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
OutputDir=Output
OutputBaseFilename=MPServerProvider Installer {#AppVersion}
SetupIconFile=..\assets\app_icon.ico
UninstallDisplayIcon={app}\{#AppExe}
UninstallDisplayName={#AppName}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
CloseApplications=yes
RestartApplications=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExe}"
Name: "{group}\Uninstall {#AppName}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExe}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExe}"; Description: "{cm:LaunchProgram,{#AppName}}"; Flags: nowait postinstall skipifsilent

[Code]
function IsSourceCheckout(Dir: String): Boolean;
var
  FindRec: TFindRec;
begin
  Result := DirExists(AddBackslash(Dir) + '.git') or
            DirExists(AddBackslash(Dir) + '_MPServerProvider') or
            DirExists(AddBackslash(Dir) + 'dotnet\src');
  if (not Result) and FindFirst(AddBackslash(Dir) + '*.sln', FindRec) then
  begin
    Result := True;
    FindClose(FindRec);
  end;
end;

function SourceCheckoutMessage(Dir: String): String;
begin
  Result := 'The folder "' + Dir + '" contains MP Server Provider source code (a dev checkout).' + #13#10 +
            'Installing there would let an uninstall delete it. Please choose another folder.';
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  if (CurPageID = wpSelectDir) and IsSourceCheckout(WizardDirValue) then
  begin
    MsgBox(SourceCheckoutMessage(WizardDirValue), mbError, MB_OK);
    Result := False;
  end;
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
begin
  // Also covers silent installs, which skip the folder page.
  if IsSourceCheckout(ExpandConstant('{app}')) then
    Result := SourceCheckoutMessage(ExpandConstant('{app}'))
  else
    Result := '';
end;
