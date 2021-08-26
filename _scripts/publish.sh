version=0.8.9
for i in tranalyze trcombine trconvert trdelabel trdelete trdot trfold trfoldlit trformat trgen trgroup trinsert trjson trkleene trmvsr trparse trprint trrename trrup trsplit trsponge trst trstrip trtext trthompson trtokens trtree trunfold trungroup trwdog trxgrep trxml trxml2
do
	echo $i
	pushd $i
	cd bin/Debug
	dotnet nuget push $i.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json
	popd
done
