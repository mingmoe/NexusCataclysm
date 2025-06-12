
git submodule update --init --recursive
if($? -eq $false){ return }
dotnet build --configuration Debug
if($? -eq $false){ return }
dotnet build --configuration Release
if($? -eq $false){ return }
