for $ws in //labeledElement/identifier/@WS
  let $target := $ws/ancestor::labeledElement/..
  return insert node (attribute WS { string($ws) }) as first into $target
