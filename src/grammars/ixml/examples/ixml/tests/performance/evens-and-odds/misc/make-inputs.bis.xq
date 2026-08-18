(: make-inputs for sizes-1 tests :)

let $dir := 'file:///home/cmsmcq/2022/github/ixml-tests/tests-wood/sizes-1/'

let $exponents := 1 to 14
for $e in $exponents
let $base-length := math:pow(2, $e)
for $polarity in ('P', 'N')
for $tag in ('e', 'o')
let $L := if (($polarity eq 'P') and ($tag eq 'e'))
          then $base-length 
          else if (($polarity eq 'P') and ($tag eq 'o'))
          then $base-length + 1
          else if (($polarity eq 'N') and ($tag eq 'e'))
          then $base-length + 1
          else if (($polarity eq 'N') and ($tag eq 'o'))
          then $base-length
          else error()

let $filename := $polarity || format-number($L,'00000') || $tag || ".txt"
let $data := string-join(
                 (for $n in 1 to xs:integer($L) return 'a',
                  $tag),
                 '')

return file:write($dir || $filename, $data)