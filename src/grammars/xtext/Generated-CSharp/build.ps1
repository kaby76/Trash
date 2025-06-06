# Generated from trgen 0.23.12
if (Test-Path -Path transformGrammar.py -PathType Leaf) {
    $(& python3 transformGrammar.py ) 2>&1 | Write-Host
}

$version = dotnet trxml2 .\Other.csproj `
    | Where-Object { $_ -match 'PackageReference/@Version' } `
    | ForEach-Object {
        ($_ -split '=')[1].Trim()
    }

$(& antlr4 -v $version xtext.g4 -encoding utf-8 -Dlanguage=CSharp  ; $compile_exit_code = $LASTEXITCODE) | Write-Host
if($compile_exit_code -ne 0){
    exit $compile_exit_code
}


$(& dotnet build Test.csproj; $compile_exit_code = $LASTEXITCODE) | Write-Host
exit $compile_exit_code
