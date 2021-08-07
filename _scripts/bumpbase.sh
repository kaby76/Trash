version="1.9.0"

for i in tranalyze trcombine trconvert trdelabel trdelete trdot trfold trfoldlit trgen trgroup trinsert trjson trkleene trmvsr trparse trprint trrename trsplit trsponge trst trstrip trtext trthompson trtokens trtree trunfold trungroup trwdog trxgrep trxml trxml2
do
	pushd $i
	rm -f asdfasdf
	cat *.csproj | sed -e "s%\"Domemtech.TrashBase\" Version=\".*\"%\"Domemtech.TrashBase\" Version=\"$version\"%" > asdfasdf
	mv asdfasdf *.csproj	
	cat *.csproj | sed -e "s%\"AntlrTreeEditing\" Version=\".*\"%\"AntlrTreeEditing\" Version=\"2.9.0\"%" > asdfasdf
	mv asdfasdf *.csproj	
	popd
done
for i in tragl Docs AntlrJson
do
	pushd $i
	rm -f asdfasdf
	cat *.csproj | sed -e "s%\"Domemtech.TrashBase\" Version=\".*\"%\"Domemtech.TrashBase\" Version=\"$version\"%" > asdfasdf
	mv asdfasdf *.csproj	
	cat *.csproj | sed -e "s%\"AntlrTreeEditing\" Version=\".*\"%\"AntlrTreeEditing\" Version=\"2.9.0\"%" > asdfasdf
	mv asdfasdf *.csproj	
	popd
done
