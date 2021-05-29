

/** Parse an entire html file, firing events to a single listener
 *  for each image and href link encountered.  All tokens are
 *  defined to skip so the lexer will continue scarfing until EOF.
 */
lexer grammar LinkExtractor;
options {
	filter=SCARF;
}

AHREF
	:	'<a' WS (ATTR)+ '>'
	;

IMG	:	'<img' WS (ATTR)+ '>'
	;
ATTR
	:WORD '='
		(STRING
		|WORD
		)
	;
WORD:	(	'a'..'z' | '0'..'9' | '/' | '.' | '#' | '_'
		)+
	;
STRING
	:	'"' (~'"')* '"'
	|	'\'' (~'\'')* '\''
	;
WS	:	(	' '
		|	'\t'
		|	'\f'
		|	(	'\r\n'  // DOS
			|	'\r'    // Macintosh
			|	'\n'    // Unix (the right way)
			)
		)
	;
SCARF
	:	WS	// track line numbers while you scarf
	|	.
	;