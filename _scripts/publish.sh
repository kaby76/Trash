version=0.8.1
for i in tranalyze trcombine trconvert trdelabel trdelete trfold trfoldlit trgen trgroup trjson trkleene trmvsr trparse trprint trrename trsplit trsponge trst trstrip trtext trtokens trtree trunfold trungroup trwdog trxgrep trxml trxml2
do
	echo $i
	pushd $i
	cd bin/Debug
	dotnet nuget push $i.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json
	popd
done
