
version="0.10.0"
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
	pushd .
	rm -f asdfasdf
	touch readme.md
	cat readme.md | sed -e 's%0[.][89][.][0123456789][ ][-][-].*$'"%$version -- Updated trparse, trsplit, trstrip, trtree. Add trrup, trrr.%" > asdfasdf
	mv asdfasdf readme.md
	popd
	cd ..
done
