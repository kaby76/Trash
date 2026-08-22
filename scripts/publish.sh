#!/usr/bin/bash
version=2.6.0
cd src
dotnet nuget push trash/bin/Release/trash.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json
