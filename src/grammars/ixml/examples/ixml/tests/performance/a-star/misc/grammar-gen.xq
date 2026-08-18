let $g := ('S', ' = ', "'a'*", '.&#xA;')

let $dir := 'file:///home/cmsmcq/2022/github/ixml-tests/test-wood/size-0'

let $be1 := '          ',
    $b9  := '         ',
    $be2 := concat($be1, $be1, $be1, $be1, $b9, '&#xA;',
                   $be1, $be1, $be1, $be1, $b9, '&#xA;'),
    $be3 := concat($be2, $be2, $be2, $be2, $be2,
                   $be2, $be2, $be2, $be2, $be2),
    $be4 := concat($be3, $be3, $be3, $be3, $be3,
                   $be3, $be3, $be3, $be3, $be3),
    
    $de1 := '1234.6789 ',
    $d9  := '1234.6789',
    $de2 := concat($de1, $de1, $de1, $de1, $d9, '&#xA;',
                   $de1, $de1, $de1, $de1, $d9, '&#xA;'),
    $de3 := concat($de2, $de2, $de2, $de2, $de2,
                   $de2, $de2, $de2, $de2, $de2),
    $de4 := concat($de3, $de3, $de3, $de3, $de3,
                   $de3, $de3, $de3, $de3, $de3)  

for $exponent in (1, 2, 3, 4)
for $block-style in ('s', 'm') (: single, multiple :)
for $ws-style in ('ws', 'lc', 'sc') (: ws, long comment, short comments :)

let $filename := 'G' || $exponent 
                 || '.' || $block-style 
                 || '.' || $ws-style 
                 || '.ixml'
                 
let $target := math:pow(10, $exponent),
    $makeup := $target - 10

let $grammar := if ($block-style eq 's')
                then if ($ws-style eq 'ws')
                     then string-join(
                             ( substring($be4, 1, $makeup),
                               $g ),
                             ''
                          )
                     else if ($ws-style eq 'lc')
                     then string-join( (
                             '{', 
                             substring($de4, 2, $makeup - 3),
                             '}&#xA;',
                             $g ),
                             ''                           
                          )
                     else (: $ws-style eq 'sc' :)
                          let $comment := concat(
                                  '{',
                                  substring($de2, 2, 22),
                                  '}&#xA;'
                              ),
                              $comment-count := $makeup idiv 25,
                              $remainder := $makeup mod 25
                          return string-join(
                              ( for $i in 1 to $comment-count
                                return $comment,
                                if ($remainder lt 3)
                                then '{}&#xA;'
                                else ('{',
                                   substring($de2, 2, $remainder - 3),
                                   '}'),
                                $g
                              ),
                              ''
                          )
                else (: block-style eq 'm' :)
                     if ($ws-style eq 'ws')
                     then let $block-size := round($makeup div 5),
                              $block := substring($be4, 
                                           1, 
                                           $block-size - 1) || '&#xA;'
                          return concat(
                                   $block,
                                   string-join($g, $block),
                                   $block
                          )
                     else if ($ws-style eq 'lc')
                     then ()
                     else (: $ws-style eq 'sc' :)
                          let $comment-length := $makeup div 5,
                              $comment := concat('{',
                                             substring($de4, 
                                                2, 
                                                $comment-length - 3
                                             ),
                                             '}&#xA;' 
                                          )
                          return concat($comment,
                             string-join($g, $comment),
                             $comment
                          )
                () 
return file:write($dir || '/' || $filename, $grammar)