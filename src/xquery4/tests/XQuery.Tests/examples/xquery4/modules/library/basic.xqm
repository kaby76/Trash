module namespace local = 'http://example.com/lib'; declare function local:greet($name as xs:string) as xs:string { concat('Hello, ', $name) };
