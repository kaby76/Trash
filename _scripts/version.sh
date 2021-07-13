for i in tranalyze trcombine trconvert trdelabel trdelete trdot trfold trfoldlit trgen trgroup trinsert trjson trkleene trmvsr trparse trprint trrename trsplit trsponge trst trstrip trtext trthompson trtokens trtree trunfold trungroup trwdog trxgrep trxml trxml2
do
	echo $i
	$i --version
done
for i in tragl
do
	echo $i
	$i --version
done
