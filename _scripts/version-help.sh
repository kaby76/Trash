
version="0.8.9"
for i in tranalyze trcombine trconvert trdelabel trdelete trdot trfold trfoldlit trformat trgen trgroup trinsert trjson trkleene trmvsr trparse trprint trrename trsplit trsponge trst trstrip trtext trthompson trtokens trtree trunfold trungroup trwdog trxgrep trxml trxml2
do
	echo $i
	pushd $i
	rm -f asdfasdf
	touch readme.md
	cat readme.md | sed -e 's%0[.]8[.][0123456789][ ][-][-].*$'"%$version -- Updated all app, but especially for tranalyze, trconvert, trformat, trgen, trrename.%" > asdfasdf
	mv asdfasdf readme.md
	popd
done
for i in tragl
do
	echo $i
	pushd $i
	rm -f asdfasdf
	touch readme.md
	cat readme.md | sed -e 's%0[.]8[.][0123456789][ ][-][-].*$'"%$version -- Updated all app, but especially for tranalyze, trconvert, trformat, trgen, trrename.%" > asdfasdf
	mv asdfasdf readme.md
	popd
done
