[Setup]
AppName=IMU Mocap Viewer
AppPublisher=x-io Technologies Limited
AppVerName=IMU Mocap Viewer
AppVersion=1.0.0
DefaultDirName={autopf64}\IMU Mocap Viewer
DefaultGroupName=IMU Mocap Viewer
DisableProgramGroupPage=yes
OutputBaseFilename=IMU-Mocap-Viewer-Setup
UninstallDisplayIcon={app}\IMU Mocap Viewer.exe
WizardStyle=modern

[Files]
Source: "files/*"; destdir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{autodesktop}\IMU Mocap Viewer"; Filename: "{app}\IMU Mocap Viewer.exe"
Name: "{group}\IMU Mocap Viewer"; Filename: "{app}\IMU Mocap Viewer.exe"

[Run]
Filename: {app}\IMU Mocap Viewer.exe; Flags: postinstall
