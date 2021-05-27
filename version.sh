version="0.8.0"

for i in tranalyze trconvert trfold trfoldlit trgen trgroup trjson trkleene trparse trprint trrename trst trstrip trtext trunfold trungroup trwdog trxgrep trxml trxml2
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
