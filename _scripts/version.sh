version="0.8.3"

for i in tranalyze trcombine trconvert trdelabel trdelete trfold trfoldlit trgen trgroup trjson trkleene trmvsr trparse trprint trrename trsplit trsponge trst trstrip trtext trtokens trtree trunfold trungroup trwdog trxgrep trxml trxml2
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

version="0.8.2"
for i in tranalyze trcombine trconvert trdelabel trdelete trfold trfoldlit trgen trgroup trjson trkleene trmvsr trparse trprint trrename trsplit trsponge trst trstrip trtext trtokens trtree trunfold trungroup trwdog trxgrep trxml trxml2
do
	echo $i
	pushd $i
	rm -f asdfasdf
	touch readme.md
	cat readme.md | sed -e "s%0[.]8[.][0123456789][ ][-][-]%$version --%" > asdfasdf
	mv asdfasdf readme.md
	popd
done
for i in tragl
do
	echo $i
	pushd $i
	rm -f asdfasdf
	touch readme.md
	cat readme.md | sed -e "s%0[.]8[.][0123456789][ ][-][-]%$version --%" > asdfasdf
	mv asdfasdf readme.md
	popd
done
