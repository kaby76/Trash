
version="0.11.0"
directories=`find . -maxdepth 1 -type d`
cwd=`pwd`
for i in $directories
do
	if [ "$i" == "." ]
	then
		continue
	fi
	cd $cwd/$i
	if [[ "$?" != "0" ]]
	then
		continue
	fi
	csproj=`find . -maxdepth 1 -name '*.csproj'`
	if [[ "$csproj" == "" ]]
	then
		continue
	fi
	if [[ ! -f "$i.csproj" ]]
	then
		continue
	fi
	if [[ ! -f "readme.md" ]]
	then
		continue;
	fi
	echo $i
	rm -f asdfasdf2
	touch readme.md
	cat readme.md | sed -e 's%^0[.][0-9]*[.][0-9]*[ ]*[-][-].*$'"%$version -- Updated trkleen. Added trreplace.%" > asdfasdf2
	mv asdfasdf2 readme.md
done
