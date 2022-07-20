lexer grammar TemplateLexer;

tokens { Code }

CodeStart : '<' -> more, pushMode(code) ;
Any : ~'<'+ ;

mode code;
CodeEnd : '>' -> type(Code), popMode;
AnyCode : . -> more;

