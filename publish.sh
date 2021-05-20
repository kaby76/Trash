version=0.7.0

pushd "tranalyze/bin/Debug/"; dotnet nuget push tranalyze.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trconvert/bin/Debug/"; dotnet nuget push trconvert.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trfold/bin/Debug/"; dotnet nuget push trfold.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trfoldlit/bin/Debug/"; dotnet nuget push trfoldlit.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trgen/bin/Debug/"; dotnet nuget push trgen.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trgroup/bin/Debug/"; dotnet nuget push trgroup.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trjson/bin/Debug/"; dotnet nuget push trjson.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trkleene/bin/Debug/"; dotnet nuget push trkleene.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trparse/bin/Debug/"; dotnet nuget push trparse.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trprint/bin/Debug/"; dotnet nuget push trprint.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trrename/bin/Debug/"; dotnet nuget push trrename.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trst/bin/Debug/"; dotnet nuget push trst.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trstrip/bin/Debug/"; dotnet nuget push trstrip.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trtext/bin/Debug/"; dotnet nuget push trtext.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trtokens/bin/Debug/"; dotnet nuget push trtokens.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trtree/bin/Debug/"; dotnet nuget push trtree.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trunfold/bin/Debug/"; dotnet nuget push trunfold.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trungroup/bin/Debug/"; dotnet nuget push trungroup.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trwdog/bin/Debug/"; dotnet nuget push trwdog.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trxgrep/bin/Debug/"; dotnet nuget push trxgrep.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trxml/bin/Debug/"; dotnet nuget push trxml.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
pushd "trxml2/bin/Debug/"; dotnet nuget push trxml2.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json; popd
