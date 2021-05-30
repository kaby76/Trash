version="1.2.0"

for i in tranalyze trcombine trconvert trdelete trfold trfoldlit trgen trgroup trjson trkleene trparse trprint trrename trst trstrip trtext trtokens trtree trunfold trungroup trwdog trxgrep trxml trxml2
do
	pushd $i
	rm -f asdfasdf
	cat *.csproj | sed -e "s%\"Domemtech.TrashBase\" Version=\".*\"%\"Domemtech.TrashBase\" Version=\"$version\"%" > asdfasdf
	mv asdfasdf *.csproj	
	popd
done
