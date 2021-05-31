version="0.8.1"

for i in tranalyze trcombine trconvert trdelete trfold trfoldlit trgen trgroup trjson trkleene trparse trprint trrename trsplit trsponge trst trstrip trtext trtokens trtree trunfold trungroup trwdog trxgrep trxml trxml2
do
	echo $i
	if [[ ! -d $i ]]
	then
		continue
	fi
	pushd $i
	count=`ls *.csproj | wc | awk '{print $1}'`
	if [[ $count != 1 ]]
	then
		continue
	fi
	rm -f asdfasdf
	cat *.csproj | sed -e "s%[<][Vv]ersion[>].*[<][/][Vv]ersion[>]%<Version\>$version</Version>%" > asdfasdf
	mv asdfasdf *.csproj	
	popd
done
