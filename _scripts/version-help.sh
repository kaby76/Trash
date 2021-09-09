
version="0.10.0"
for i in tranalyze trcombine trconvert trdelabel trdelete trdot trfold trfoldlit trformat trgen trgroup trinsert trjson trkleene trmvsr trparse trprint trrename trrr trrup trsplit trsponge trst trstrip trtext trthompson trtokens trtree trunfold trungroup trwdog trxgrep trxml trxml2
do
	echo $i
	pushd $i
	rm -f asdfasdf
	touch readme.md
	cat readme.md | sed -e 's%0[.][89][.][0123456789][ ][-][-].*$'"%$version -- Updated trparse, trsplit, trstrip, trtree. Add trrup, trrr.%" > asdfasdf
	mv asdfasdf readme.md
	popd
done
for i in tragl
do
	echo $i
	pushd $i
	rm -f asdfasdf
	touch readme.md
	cat readme.md | sed -e 's%0[.][89][.][0123456789][ ][-][-].*$'"%$version -- Updated trparse, trsplit, trstrip, trtree. Add trrup, trrr.%" > asdfasdf
	mv asdfasdf readme.md
	popd
done
