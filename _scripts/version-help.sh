
version="0.8.6"
for i in tranalyze trcombine trconvert trdelabel trdelete trdot trfold trfoldlit trgen trgroup trinsert trjson trkleene trmvsr trparse trprint trrename trsplit trsponge trst trstrip trtext trthompson trtokens trtree trunfold trungroup trwdog trxgrep trxml trxml2
do
	echo $i
	pushd $i
	rm -f asdfasdf
	touch readme.md
	cat readme.md | sed -e 's%0[.]8[.][0123456789][ ][-][-].*$'"%$version -- Updated trinsert, trconvert (Issue #1).%" > asdfasdf
	mv asdfasdf readme.md
	popd
done
for i in tragl
do
	echo $i
	pushd $i
	rm -f asdfasdf
	touch readme.md
	cat readme.md | sed -e 's%0[.]8[.][0123456789][ ][-][-].*$'"%$version -- Updated trinsert, trconvert (Issue #1).%" > asdfasdf
	mv asdfasdf readme.md
	popd
done
