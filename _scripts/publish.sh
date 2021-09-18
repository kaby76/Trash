version=0.10.0
directories=`find . -maxdepth 1 -type d`
for i in $directories
do
	if [ "$i" == "." ]
	then
		continue
	fi
	cd $i
	csproj=`find . -maxdepth 1 -name '*.csproj'`
	if [[ "$csproj" == "" ]]
	then
		cd ..
		continue
	fi
	if [[ ! -f "$i.csproj" ]]
	then
		echo $i
		echo nope
		exit 1
	fi
	echo $i
	pushd .
	cd bin/Debug
	tool=${i##*/}
	dotnet nuget push $tool.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json
	popd
	cd ..
done
