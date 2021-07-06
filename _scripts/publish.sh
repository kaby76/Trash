version=0.8.5
for i in tranalyze trcombine trconvert trdelabel trdelete trdot trfold trfoldlit trgen trgroup trinsert trjson trkleene trmvsr trparse trprint trrename trsplit trsponge trst trstrip trtext trthompson trtokens trtree trunfold trungroup trwdog trxgrep trxml trxml2
do
	echo $i
	pushd $i
	cd bin/Debug
	dotnet nuget push $i.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json
	popd
done
