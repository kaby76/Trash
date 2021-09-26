version="0.11.2"

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
	rm -f asdfasdf
	cat *.csproj | sed -e "s%[<][Vv]ersion[>].*[<][/][Vv]ersion[>]%<Version\>$version</Version>%" > asdfasdf
	mv asdfasdf *.csproj	
	cd ..
done

pushd trgen
rm -f asdfasdf
cat CGen.cs | sed -e 's%"[0123456789][.][0123456789][.][0123456789]"%"'$version'"%' > asdfasdf
mv asdfasdf CGen.cs
popd
