
Write-Host "Download artifacts from https://github.com/FNA-XNA/fnalibs-dailies/actions"
Write-Host "Ensure there are fnalibs-apple.zip and fnalibs.zip in the scripts/ directory!"

Remove-Item -Force -Recurse -Path scripts/temp -ErrorAction Ignore -WarningAction Ignore
New-Item -ItemType Directory -Path scripts/temp -ErrorAction Ignore -WarningAction Ignore
Remove-Item -Force -Recurse -Path nativelibs -ErrorAction Ignore -WarningAction Ignore
New-Item -ItemType Directory -Path nativelibs -ErrorAction Ignore -WarningAction Ignore

# copy win-x64 linux-x64 apple-arm64 only
Expand-Archive -Path scripts/fnalibs-apple.zip -DestinationPath scripts/temp -Force
Expand-Archive -Path scripts/fnalibs.zip -DestinationPath scripts/temp -Force

New-Item -ItemType Directory -Path "NexusCataclysm.Client/bin/Debug/net9.0/" -ErrorAction Ignore -WarningAction Ignore -Force
New-Item -ItemType Directory -Path "NexusCataclysm.Client/bin/Release/net9.0/" -ErrorAction Ignore -WarningAction Ignore -Force
Copy-Item scripts/temp/x64/* nativelibs
Copy-Item scripts/temp/lib64/* nativelibs
Copy-Item scripts/temp/osx/* nativelibs
Copy-Item nativelibs/* "NexusCataclysm.Client/bin/Debug/net9.0/"
Copy-Item nativelibs/* "NexusCataclysm.Client/bin/Release/net9.0/"

Write-Host "copy native library files to nativelibs/ and NexusCataclysm.Client/bin/[Debug|Release]/net9.0/"
