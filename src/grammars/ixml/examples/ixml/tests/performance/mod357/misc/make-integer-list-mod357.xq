let $outdir := 'file:///home/cmsmcq/2022/github/cmsmcq-ixml' 
               || '/tests/performance/mod357/input'
let $maxpow := 10
for $n in (for $p in 1 to $maxpow return xs:integer(math:pow(2, $p)))
let $integers := 
    for $m in 1 to $n
    let $r := random:integer(1000000000),
        $r2 := random:integer(2),
        $i := (3, 5, 7)[($r2 + 1)] * $r
    return $i,
$fn := $outdir || '/numbers.' || format-number($n,'9999') || '.txt'
    
return ($n, file:write($fn, $integers))