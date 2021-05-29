# $version = "--version 0.8.1"
$apps = @('tranalyze','trconvert','trdelete','trfold','trfoldlit','trgen','trgroup','trjson','trkleene','trparse','trprint','trrename','trst','trstrip','trtext','trtokens','trtree','trunfold','trungroup','trwdog','trxgrep','trxml','trxml2')
foreach ($i in $apps) {
	dotnet tool install -g $i $version
}
