#
for i in tranalyze trcombine trconvert trdelabel trdelete trdot trfold trfoldlit trformat trgen trgroup trinsert trjson trkleene trmove trmvsr trparse trprint trrename trreplace trrr trrup trsplit trsponge trst trstrip trtext trtokens trtree trunfold trungroup trwdog trxgrep trxml trxml2
do
	dotnet tool install -g $i
done
