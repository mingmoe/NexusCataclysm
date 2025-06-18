
New-Item -ItemType Directory -Path "NexusCataclysm.Client/bin/Debug/net9.0/" -ErrorAction Ignore -WarningAction Ignore -Force
New-Item -ItemType Directory -Path "NexusCataclysm.Client/bin/Release/net9.0/" -ErrorAction Ignore -WarningAction Ignore -Force

$TEMP_DEBUG = Resolve-Path "NexusCataclysm.Client/bin/Debug/net9.0/"
$TEMP_RELEASE = Resolve-Path "NexusCataclysm.Client/bin/Release/net9.0/"
$TEMP_RESOURCE = Resolve-Path "resources"

New-Item -Path "$TEMP_DEBUG/resources" -ItemType SymbolicLink -Value $TEMP_RESOURCE -ErrorAction Ignore -WarningAction Ignore -Force
New-Item -Path "$TEMP_RELEASE/resources" -ItemType SymbolicLink -Value $TEMP_RESOURCE -ErrorAction Ignore -WarningAction Ignore -Force
