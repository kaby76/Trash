let $name := string(./IDENTIFIER)
return
  if ($name = substring($input, $start + 1, $length)
      and exists($tree//decl[IDENTIFIER = $name]))
  then $name
  else ()
