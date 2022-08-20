# $version = "--version 0.16.6"
$apps = @('tranalyze','trcombine','trconvert','trdelabel','trdelete','trdot','trfold','trfoldlit','trformat','trgen','trgroup','trinsert','tritext','trjson','trkleene','trmove','trparse','trperf','trpiggy','trprint','trrename','trreplace','trrr','trrup','trsort','trsplit','trsponge','trst','trstrip','trtext','trtokens','trtree','trull','trunfold','trungroup','trwdog','trxgrep','trxml','trxml2')
foreach ($i in $apps) {
	dotnet tool install -g $i $version
}
