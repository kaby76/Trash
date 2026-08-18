(: make-inputs for sizes-1 tests :)

let $dir := 'file:///home/cmsmcq/2022/github/ixml-tests/tests-wood' 
             || '/a-star/input/'

let $exponents := 0 to 14
for $e in $exponents
for $polarity in ('P', 'N')

let $suffix := if ($polarity eq 'P') then '' else 'b'
let $filename := $polarity || 'b' || format-number($e, '00') || ".txt"
let $data := string-join(
                 (for $n in 1 to xs:integer(math:pow(2, $e)) return 'a',
                  $suffix),
                 '')

return file:write($dir || $filename, $data)