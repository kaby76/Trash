version="0.8.8"

for i in tranalyze trcombine trconvert trdelabel trdelete trdot trfold trfoldlit trgen trgroup trinsert trjson trkleene trmvsr trparse trprint trrename trsplit trsponge trst trstrip trtext trthompson trtokens trtree trunfold trungroup trwdog trxgrep trxml trxml2
do
	echo $i
	pushd $i
	rm -f asdfasdf
	cat *.csproj | sed -e "s%[<][Vv]ersion[>].*[<][/][Vv]ersion[>]%<Version\>$version</Version>%" > asdfasdf
	mv asdfasdf *.csproj	
	popd
done
for i in tragl
do
	echo $i
	pushd $i
	rm -f asdfasdf
	cat *.csproj | sed -e "s%[<][Vv]ersion[>].*[<][/][Vv]ersion[>]%<Version\>$version</Version>%" > asdfasdf
	mv asdfasdf *.csproj	
	popd
done

pushd trgen
rm -f asdfasdf
cat CGen.cs | sed -e 's%"[0123456789][.][0123456789][.][0123456789]"%"'$version'"%' > asdfasdf
mv asdfasdf CGen.cs
popd