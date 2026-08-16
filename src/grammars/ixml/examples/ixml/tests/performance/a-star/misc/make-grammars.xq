(: Generate various forms of the grammar S='a'*. by adding whitespace. :)

(: Declare some useful variables to fill a 63-character line and
   add a newline character. :)
declare variable $local:ws-line as xs:string := string-join(
  (for $n in 1 to 63 return ' ', '&#xA;'), 
  ''
);
declare variable $local:comment-line as xs:string := string-join(
  (for $n in 1 to 15 return '{..}', '{.}&#xA;'), 
  ''
);
declare variable $local:comment-data-line as xs:string := string-join(
  (for $n in 1 to 6 return '1234.6789 ', '123&#xA;'), 
  ''
);


(: local:generate-ws(length, style):  generate a whitespace block of 
   the requested length in the requested style. :)
declare function local:generate-ws(
  $length as xs:integer,
  $style as xs:string (: ws, lc, sc :)
) as xs:string {
  (: Figure on a padding of four characters for each style, 
     so adjust length :)
  let $len := max(($length - 4, 0))
  let $line-count := $len idiv 64,
      $remainder  := $len mod 64
  
  return if ($len eq 0)
         then ''
         else if ($style eq 'ws') (: whitespace string of length $length :)
         then string-join(
                (' &#xA;',
                 for $n in 1 to $line-count 
                 return $local:ws-line,
                 substring($local:ws-line, 1, $remainder),
                 ' &#xA;'
                ), '')
         else if ($style eq 'lc') (: single comment of length $length :)
         then string-join(
                ('{&#xA;', 
                 for $n in 1 to $line-count 
                 return $local:comment-data-line,
                 substring($local:comment-data-line, 1, $remainder),
                 '}&#xA;'
                ), '')
         else (: $style eq 'sc' :) (: $length characters' worth of {..} :)
              string-join(
                (for $n in 1 to $line-count 
                 return $local:comment-line,
                 for $n in 1 to ($remainder idiv 4)
                 return "{..}",
                 "{.",
                 for $n in 1 to ($remainder mod 4)
                 return '.',
                 '}&#xA;'
                ), '')
      
};

let $g := ('', 'S', ' = ', "'a'*", '.&#xA;', '')

let $dir := 'file:///home/cmsmcq/2022/github/ixml-tests/tests-wood/sizes-0'

let $exponents := 1 to 10 (: 1 to 4 for decimal :),
    $block-styles := ('s', 'm') (: single, multiple :),
    $ws-styles := ('ws', 'lc', 'sc') (: white space, long comment, short :)

for $exponent in $exponents
for $block-style in $block-styles
for $ws-style in $ws-styles

let $filename := 'G.b' (: "G.d" for decimal :) 
                 || $exponent 
                 || '.' || $block-style 
                 || '.' || $ws-style 
                 || '.ixml'
                 
(: let $target := round(math:pow(10, $exponent)), :)

let $target := round(10 * math:pow(2, $exponent)),
    $deficit := xs:integer($target - 10)

let $grammar := if ($block-style eq 's')
                then let $injection := local:generate-ws($deficit, $ws-style)
                     return string-join( ($injection, $g), '')
                else (: block-style eq 'm' :)
                     let $injection := local:generate-ws(
                                          $deficit idiv 5, 
                                          $ws-style
                                        )
                     return string-join($g, $injection)
                     
return file:write($dir || '/' || $filename, $grammar)