
Write-Host "Download artifacts from https://github.com/FNA-XNA/fnalibs-dailies/actions"
Write-Host "Ensure there are fnalibs-apple.zip and fnalibs.zip in the Script directory!"

Remove-Item -Force -Recurse -Path Script/temp -ErrorAction Ignore -WarningAction Ignore
New-Item -ItemType Directory -Path Script/temp -ErrorAction Ignore -WarningAction Ignore
Remove-Item -Force -Recurse -Path nativelibs -ErrorAction Ignore -WarningAction Ignore
New-Item -ItemType Directory -Path nativelibs -ErrorAction Ignore -WarningAction Ignore

# copy win-x64 linux-x64 apple-arm64 only
Expand-Archive -Path Script/fnalibs-apple.zip -DestinationPath Script/temp -Force
Expand-Archive -Path Script/fnalibs.zip -DestinationPath Script/temp -Force

New-Item -ItemType Directory -Path "NexusCataclysm.Client/bin/Debug/net9.0/" -ErrorAction Ignore -WarningAction Ignore -Force
New-Item -ItemType Directory -Path "NexusCataclysm.Client/bin/Release/net9.0/" -ErrorAction Ignore -WarningAction Ignore -Force
Copy-Item Script/temp/x64/* nativelibs
Copy-Item Script/temp/lib64/* nativelibs
Copy-Item Script/temp/osx/* nativelibs
Copy-Item nativelibs/* "NexusCataclysm.Client/bin/Debug/net9.0/"
Copy-Item nativelibs/* "NexusCataclysm.Client/bin/Release/net9.0/"

Write-Host "copy native library files to nativelibs/ and NexusCataclysm.Client/bin/[Debug|Release]/net9.0/"
