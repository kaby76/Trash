# $version = "--version 0.8.1"
$apps = @('tranalyze','trcombine','trconvert','trdelabel','trdelete','trdot','trfold','trfoldlit','trformat','trgen','trgroup','trinsert','trjson','trkleene','trmvsr','trparse','trprint','trrename','trrup','trst','trstrip','trtext','trthompson','trtokens','trtree','trunfold','trungroup','trwdog','trxgrep','trxml','trxml2')
foreach ($i in $apps) {
	dotnet tool install -g $i $version
}
