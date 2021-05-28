/** Java 1.3 Recognizer
 *
 * Run 'java Main [-showtree] directory-full-of-java-files'
 *
 * [The -showtree option pops up a Swing frame that shows
 *  the AST constructed from the parser.]
 *
 * Run 'java Main <directory full of java files>'
 *
 * Contributing authors:
 *		John Mitchell		johnm@non.net
 *		Terence Parr		parrt@magelang.com
 *		John Lilley			jlilley@empathy.com
 *		Scott Stanchfield	thetick@magelang.com
 *		Markus Mohnen       mohnen@informatik.rwth-aachen.de
 *      Peter Williams      pete.williams@sun.com
 *      Allan Jacobs        Allan.Jacobs@eng.sun.com
 *      Steve Messick       messick@redhills.com
 *      John Pybus			john@pybus.org
 *
 * Version 1.00 December 9, 1997 -- initial release
 * Version 1.01 December 10, 1997
 *		fixed bug in octal def (0..7 not 0..8)
 * Version 1.10 August 1998 (parrt)
 *		added tree construction
 *		fixed definition of WS,comments for mac,pc,unix newlines
 *		added unary plus
 * Version 1.11 (Nov 20, 1998)
 *		Added "shutup" option to turn off last ambig warning.
 *		Fixed inner class def to allow named class defs as statements
 *		synchronized requires compound not simple statement
 *		add [] after builtInType DOT class in primaryExpression
 *		"const" is reserved but not valid..removed from modifiers
 * Version 1.12 (Feb 2, 1999)
 *		Changed LITERAL_xxx to xxx in tree grammar.
 *		Updated java.g to use tokens {...} now for 2.6.0 (new feature).
 *
 * Version 1.13 (Apr 23, 1999)
 *		Didn't have (stat)? for else clause in tree parser.
 *		Didn't gen ASTs for interface extends.  Updated tree parser too.
 *		Updated to 2.6.0.
 * Version 1.14 (Jun 20, 1999)
 *		Allowed final/abstract on local classes.
 *		Removed local interfaces from methods
 *		Put instanceof precedence where it belongs...in relationalExpr
 *			It also had expr not type as arg; fixed it.
 *		Missing ! on SEMI in classBlock
 *		fixed: (expr) + "string" was parsed incorrectly (+ as unary plus).
 *		fixed: didn't like Object[].class in parser or tree parser
 * Version 1.15 (Jun 26, 1999)
 *		Screwed up rule with instanceof in it. :(  Fixed.
 *		Tree parser didn't like (expr).something; fixed.
 *		Allowed multiple inheritance in tree grammar. oops.
 * Version 1.16 (August 22, 1999)
 *		Extending an interface built a wacky tree: had extra EXTENDS.
 *		Tree grammar didn't allow multiple superinterfaces.
 *		Tree grammar didn't allow empty var initializer: {}
 * Version 1.17 (October 12, 1999)
 *		ESC lexer rule allowed 399 max not 377 max.
 *		java.tree.g didn't handle the expression of synchronized
 *		statements.
 * Version 1.18 (August 12, 2001)
 *      	Terence updated to Java 2 Version 1.3 by
 *		observing/combining work of Allan Jacobs and Steve
 *		Messick.  Handles 1.3 src.  Summary:
 *		o  primary didn't include boolean.class kind of thing
 *      	o  constructor calls parsed explicitly now:
 * 		   see explicitConstructorInvocation
 *		o  add strictfp modifier
 *      	o  missing objBlock after new expression in tree grammar
 *		o  merged local class definition alternatives, moved after declaration
 *		o  fixed problem with ClassName.super.field
 *      	o  reordered some alternatives to make things more efficient
 *		o  long and double constants were not differentiated from int/float
 *		o  whitespace rule was inefficient: matched only one char
 *		o  add an examples directory with some nasty 1.3 cases
 *		o  made Main.java use buffered IO and a Reader for Unicode support
 *		o  supports UNICODE?
 *		   Using Unicode charVocabulay makes code file big, but only
 *		   in the bitsets at the end. I need to make ANTLR generate
 *		   unicode bitsets more efficiently.
 * Version 1.19 (April 25, 2002)
 *		Terence added in nice fixes by John Pybus concerning floating
 *		constants and problems with super() calls.  John did a nice
 *		reorg of the primary/postfix expression stuff to read better
 *		and makes f.g.super() parse properly (it was METHOD_CALL not
 *		a SUPER_CTOR_CALL).  Also:
 *
 *		o  "finally" clause was a root...made it a child of "try"
 *		o  Added stuff for asserts too for Java 1.4, but *commented out*
 *		   as it is not backward compatible.
 *
 * Version 1.20 (October 27, 2002)
 *
 *      Terence ended up reorging John Pybus' stuff to
 *      remove some nondeterminisms and some syntactic predicates.
 *      Note that the grammar is stricter now; e.g., this(...) must
 *	be the first statement.
 *
 *      Trinary ?: operator wasn't working as array name:
 *          (isBig ? bigDigits : digits)[i];
 *
 *      Checked parser/tree parser on source for
 *          Resin-2.0.5, jive-2.1.1, jdk 1.3.1, Lucene, antlr 2.7.2a4,
 *	    and the 110k-line jGuru server source.
 *
 * Version 1.21 (October 17, 2003)
 *	Fixed lots of problems including:
 *	Ray Waldin: add typeDefinition to interfaceBlock in java.tree.g
 *  He found a problem/fix with floating point that start with 0
 *  Ray also fixed problem that (int.class) was not recognized.
 *  Thorsten van Ellen noticed that \n are allowed incorrectly in strings.
 *  TJP fixed CHAR_LITERAL analogously.
 *
 * This grammar is in the PUBLIC DOMAIN
 */
grammar JavaRecognizer;

// Compilation Unit: In Java, this is a single file.  This is the start
//   rule for this parser
compilationUnit
	:	// A compilation unit starts with an optional package definition
		(	packageDefinition
		|	/* nothing */
		)

		// Next we have a series of zero or more import statements
		( importDefinition )*

		// Wrapping things up with any number of class or interface
		//    definitions
		( typeDefinition )*

		EOF
	;


// Package statement: "package" followed by an identifier.
packageDefinition // let ANTLR handle errors
	:'package' identifier SEMI
	;


// Import statement: import followed by a package or class name
importDefinition
	:'import' identifierStar SEMI
	;

// A type definition in a file is either a class or interface definition.
typeDefinition
	:modifiers
		( classDefinition
		| interfaceDefinition
		)
	|	SEMI
	;

/** A declaration is the creation of a reference or primitive-type variable
 *  Create a separate Type/Var tree for each var in the var list.
 */
declaration
	:modifierstypeSpecvariableDefinitions
	;

// A type specification is a type name with possible brackets afterwards
//   (which would make it an array type).
typeSpec
	: classTypeSpec
	| builtInTypeSpec
	;

// A class type specification is a class type with possible brackets afterwards
//   (which would make it an array type).
classTypeSpec
	:	identifier (LBRACK RBRACK)*
	;

// A builtin type specification is a builtin type with possible brackets
// afterwards (which would make it an array type).
builtInTypeSpec
	:	builtInType (LBRACK RBRACK)*
	;

// A type name. which is either a (possibly qualified) class name or
//   a primitive (builtin) type
type
	:	identifier
	|	builtInType
	;

// The primitive types.
builtInType
	:	'void'
	|	'boolean'
	|	'byte'
	|	'char'
	|	'short'
	|	'int'
	|	'float'
	|	'long'
	|	'double'
	;

// A (possibly-qualified) java identifier.  We start with the first IDENT
//   and expand its name by adding dots and following IDENTS
identifier
	:	IDENT  ( DOT IDENT )*
	;

identifierStar
	:	IDENT
		( DOT IDENT )*
		( DOT STAR  )?
	;

// A list of zero or more modifiers.  We could have used (modifier)* in
//   place of a call to modifiers, but I thought it was a good idea to keep
//   this rule separate so they can easily be collected in a Vector if
//   someone so desires
modifiers
	:	( modifier )*
	;

// modifiers for Java classes, interfaces, class/instance vars and methods
modifier
	:	'private'
	|	'public'
	|	'protected'
	|	'static'
	|	'transient'
	|	'final'
	|	'abstract'
	|	'native'
	|	'threadsafe'
	|	'synchronized'
//	|	"const"			// reserved word, but not valid
	|	'volatile'
	|	'strictfp'
	;

// Definition of a Java class
classDefinition
	:	'class' IDENTsuperClassClauseimplementsClauseclassBlock
	;

superClassClause
	:	( 'extends'identifier )?
	;

// Definition of a Java Interface
interfaceDefinition
	:	'interface' IDENTinterfaceExtendsclassBlock
	;


// This is the body of a class.  You can have fields and extra semicolons,
// That's about it (until you see what a field is...)
classBlock
	:	LCURLY
			( field | SEMI )*
		RCURLY
	;

// An interface can extend several other interfaces...
interfaceExtends
	:	('extends'
		identifier ( COMMA identifier )*
		)?
	;

// A class can implement several interfaces...
implementsClause
	:	('implements' identifier ( COMMA identifier )*
		)?
	;

// Now the various things that can be defined inside a class or interface...
// Note that not all of these are really valid in an interface (constructors,
//   for example), and if this grammar were used for a compiler there would
//   need to be some semantic checks to make sure we're doing the right thing...
field
	:modifiers
		(ctorHeadconstructorBody

		|classDefinition

		|interfaceDefinition

		|typeSpec  // method or variable declaration(s)
			(	IDENT  // the name of the method

				// parse the formal parameter declarations.
				LPARENparameterDeclarationList RPARENdeclaratorBrackets

				// get the list of exceptions that this method is
				// declared to throw
				(throwsClause)?

				(compoundStatement | SEMI )
			|variableDefinitions SEMI
			)
		)

    // "static { ... }" class initializer
	|	'static'compoundStatement

    // "{ ... }" instance initializer
	|compoundStatement
	;

constructorBody
    :LCURLY
            ( explicitConstructorInvocation)?
            (statement)*
        RCURLY
    ;

/** Catch obvious constructor calls, but not the expr.super(...) calls */
explicitConstructorInvocation
    :   'this'LPAREN argList RPAREN SEMI
    |   'super'LPAREN argList RPAREN SEMI
    ;

variableDefinitions
	:	variableDeclarator
		(	COMMA
			variableDeclarator
		)*
	;

/** Declaration of a variable.  This can be a class/instance variable,
 *   or a local variable in a method
 * It can also include possible initialization.
 */
variableDeclarator
	:IDENTdeclaratorBracketsvarInitializer
	;

declaratorBrackets
	:
		(LBRACK RBRACK)*
	;

varInitializer
	:	( ASSIGN initializer )?
	;

// This is an initializer used to set up an array.
arrayInitializer
	:LCURLY
			(	initializer
				(
					COMMA initializer
				)*
				(COMMA)?
			)?
		RCURLY
	;


// The two "things" that can initialize an array element are an expression
//   and another (nested) array initializer.
initializer
	:	expression
	|	arrayInitializer
	;

// This is the header of a method.  It includes the name and parameters
//   for the method.
//   This also watches for a list of exception classes in a "throws" clause.
ctorHead
	:	IDENT  // the name of the method

		// parse the formal parameter declarations.
		LPAREN parameterDeclarationList RPAREN

		// get the list of exceptions that this method is declared to throw
		(throwsClause)?
	;

// This is a list of exception classes that the method is declared to throw
throwsClause
	:	'throws' identifier ( COMMA identifier )*
	;


// A list of formal parameters
parameterDeclarationList
	:	( parameterDeclaration ( COMMA parameterDeclaration )* )?
	;

// A formal parameter.
parameterDeclaration
	:parameterModifiertypeSpecIDENTdeclaratorBrackets
	;

parameterModifier
	:	('final')?
	;

// Compound statement.  This is used in many contexts:
//   Inside a class definition prefixed with "static":
//      it is a class initializer
//   Inside a class definition without "static":
//      it is an instance initializer
//   As the body of a method
//   As a completely indepdent braced block of code inside a method
//      it starts a new scope for variable definitions

compoundStatement
	:LCURLY
			// include the (possibly-empty) list of statements
			(statement)*
		RCURLY
	;


statement
	// A list of statements in curly braces -- start a new scope!
	:	compoundStatement

	// declarations are ambiguous with "ID DOT" relative to expression
	// statements.  Must backtrack to be sure.  Could use a semantic
	// predicate to test symbol table to see what the type was coming
	// up, but that's pretty hard without a symbol table ;)
	| declaration SEMI

	// An expression statement.  This could be a method call,
	// assignment statement, or any other expression evaluated for
	// side-effects.
	|	expression SEMI

	// class definition
	|modifiers classDefinition

	// Attach a label to the front of a statement
	|	IDENTCOLON statement

	// If-else statement
	|	'if' LPAREN expression RPAREN statement
		(
			'else' statement
		)?

	// For statement
	|	'for'
			LPAREN
				forInit SEMI   // initializer
				forCond	SEMI   // condition test
				forIter         // updater
			RPAREN
			statement                     // statement to loop over

	// While statement
	|	'while' LPAREN expression RPAREN statement

	// do-while statement
	|	'do' statement 'while' LPAREN expression RPAREN SEMI

	// get out of a loop (or switch)
	|	'break' (IDENT)? SEMI

	// do next iteration of a loop
	|	'continue' (IDENT)? SEMI

	// Return an expression
	|	'return' (expression)? SEMI

	// switch/case statement
	|	'switch' LPAREN expression RPAREN LCURLY
			( casesGroup )*
		RCURLY

	// exception try-catch block
	|	tryBlock

	// throw an exception
	|	'throw' expression SEMI

	// synchronize a statement
	|	'synchronized' LPAREN expression RPAREN compoundStatement

	// asserts (uncomment if you want 1.4 compatibility)
	// |	"assert"^ expression ( COLON! expression )? SEMI!

	// empty statement
	|SEMI
	;

casesGroup
	:	(
			aCase
		)+
		caseSList
	;

aCase
	:	('case' expression | 'default') COLON
	;

caseSList
	:	(statement)*
	;

// The initializer for a for loop
forInit
		// if it looks like a declaration, it is
	:	( declaration
		// otherwise it could be an expression list...
		|	expressionList
		)?
	;

forCond
	:	(expression)?
	;

forIter
	:	(expressionList)?
	;

// an exception handler try/catch block
tryBlock
	:	'try' compoundStatement
		(handler)*
		( finallyClause )?
	;

finallyClause
	:	'finally' compoundStatement
	;

// an exception handler
handler
	:	'catch' LPAREN parameterDeclaration RPAREN compoundStatement
	;


// expressions
// Note that most of these expressions follow the pattern
//   thisLevelExpression :
//       nextHigherPrecedenceExpression
//           (OPERATOR nextHigherPrecedenceExpression)*
// which is a standard recursive definition for a parsing an expression.
// The operators in java have the following precedences:
//    lowest  (13)  = *= /= %= += -= <<= >>= >>>= &= ^= |=
//            (12)  ?:
//            (11)  ||
//            (10)  &&
//            ( 9)  |
//            ( 8)  ^
//            ( 7)  &
//            ( 6)  == !=
//            ( 5)  < <= > >=
//            ( 4)  << >>
//            ( 3)  +(binary) -(binary)
//            ( 2)  * / %
//            ( 1)  ++ -- +(unary) -(unary)  ~  !  (type)
//                  []   () (method call)  . (dot -- identifier qualification)
//                  new   ()  (explicit parenthesis)
//
// the last two are not usually on a precedence chart; I put them in
// to point out that new has a higher precedence than '.', so you
// can validy use
//     new Frame().show()
//
// Note that the above precedence levels map to the rules below...
// Once you have a precedence chart, writing the appropriate rules as below
//   is usually very straightfoward



// the mother of all expressions
expression
	:	assignmentExpression
	;


// This is a list of expressions.
expressionList
	:	expression (COMMA expression)*
	;


// assignment expression (level 13)
assignmentExpression
	:	conditionalExpression
		(	(	ASSIGN
            |   PLUS_ASSIGN
            |   MINUS_ASSIGN
            |   STAR_ASSIGN
            |   DIV_ASSIGN
            |   MOD_ASSIGN
            |   SR_ASSIGN
            |   BSR_ASSIGN
            |   SL_ASSIGN
            |   BAND_ASSIGN
            |   BXOR_ASSIGN
            |   BOR_ASSIGN
            )
			assignmentExpression
		)?
	;


// conditional test (level 12)
conditionalExpression
	:	logicalOrExpression
		( QUESTION assignmentExpression COLON conditionalExpression )?
	;


// logical or (||)  (level 11)
logicalOrExpression
	:	logicalAndExpression (LOR logicalAndExpression)*
	;


// logical and (&&)  (level 10)
logicalAndExpression
	:	inclusiveOrExpression (LAND inclusiveOrExpression)*
	;


// bitwise or non-short-circuiting or (|)  (level 9)
inclusiveOrExpression
	:	exclusiveOrExpression (BOR exclusiveOrExpression)*
	;


// exclusive or (^)  (level 8)
exclusiveOrExpression
	:	andExpression (BXOR andExpression)*
	;


// bitwise or non-short-circuiting and (&)  (level 7)
andExpression
	:	equalityExpression (BAND equalityExpression)*
	;


// equality/inequality (==/!=) (level 6)
equalityExpression
	:	relationalExpression ((NOT_EQUAL | EQUAL) relationalExpression)*
	;


// boolean relational expressions (level 5)
relationalExpression
	:	shiftExpression
		(	(	(	LT
				|	GT
				|	LE
				|	GE
				)
				shiftExpression
			)*
		|	'instanceof' typeSpec
		)
	;


// bit shift expressions (level 4)
shiftExpression
	:	additiveExpression ((SL | SR | BSR) additiveExpression)*
	;


// binary addition/subtraction (level 3)
additiveExpression
	:	multiplicativeExpression ((PLUS | MINUS) multiplicativeExpression)*
	;


// multiplication/division/modulo (level 2)
multiplicativeExpression
	:	unaryExpression ((STAR | DIV | MOD ) unaryExpression)*
	;

unaryExpression
	:	INC unaryExpression
	|	DEC unaryExpression
	|	MINUS unaryExpression
	|	PLUS unaryExpression
	|	unaryExpressionNotPlusMinus
	;

unaryExpressionNotPlusMinus
	:	BNOT unaryExpression
	|	LNOT unaryExpression

		// use predicate to skip cases like: (int.class)
    |LPAREN builtInTypeSpec RPAREN
        unaryExpression

        // Have to backtrack to see if operator follows.  If no operator
        // follows, it's a typecast.  No semantic checking needed to parse.
        // if it _looks_ like a cast, it _is_ a cast; else it's a "(expr)"
    |LPAREN classTypeSpec RPAREN
        unaryExpressionNotPlusMinus

    |	postfixExpression
	;

// qualified names, array expressions, method invocation, post inc/dec
postfixExpression
	:
    /*
    "this"! lp1:LPAREN^ argList RPAREN!
		{#lp1.setType(CTOR_CALL);}

    |   "super"! lp2:LPAREN^ argList RPAREN!
		{#lp2.setType(SUPER_CTOR_CALL);}
    |
    */
        primaryExpression

		(
            /*
            options {
				// the use of postfixExpression in SUPER_CTOR_CALL adds DOT
				// to the lookahead set, and gives loads of false non-det
				// warnings.
				// shut them off.
				generateAmbigWarnings=false;
			}
		:	*/
            DOT IDENT
			(LPAREN
				argList
				RPAREN
			)?
		|	DOT 'this'

		|	DOT 'super'
            (LPAREN argList RPAREN
			|   DOT IDENT
                (LPAREN
                    argList
                    RPAREN
                )?
            )
		|	DOT newExpression
		|LBRACK expression RBRACK
		)*

		(INC
	 	|DEC
		)?
 	;

// the basic element of an expression
primaryExpression
	:	identPrimary ( DOT 'class' )?
    |   constant
	|	'true'
	|	'false'
	|	'null'
    |   newExpression
	|	'this'
	|	'super'
	|	LPAREN assignmentExpression RPAREN
		// look for int.class and int[].class
	|	builtInType
		(LBRACK RBRACK )*
		DOT 'class'
	;

/** Match a, a.b.c refs, a.b.c(...) refs, a.b.c[], a.b.c[].class,
 *  and a.b.c.class refs.  Also this(...) and super(...).  Match
 *  this or super.
 */
identPrimary
	:	IDENT
		(	DOT IDENT
		)*
		(   (LPAREN argList RPAREN )
		|	(LBRACK RBRACK
            )+
		)?
    ;

/** object instantiation.
 *  Trees are built as illustrated by the following input/tree pairs:
 *
 *  new T()
 *
 *  new
 *   |
 *   T --  ELIST
 *           |
 *          arg1 -- arg2 -- .. -- argn
 *
 *  new int[]
 *
 *  new
 *   |
 *  int -- ARRAY_DECLARATOR
 *
 *  new int[] {1,2}
 *
 *  new
 *   |
 *  int -- ARRAY_DECLARATOR -- ARRAY_INIT
 *                                  |
 *                                EXPR -- EXPR
 *                                  |      |
 *                                  1      2
 *
 *  new int[3]
 *  new
 *   |
 *  int -- ARRAY_DECLARATOR
 *                |
 *              EXPR
 *                |
 *                3
 *
 *  new int[1][2]
 *
 *  new
 *   |
 *  int -- ARRAY_DECLARATOR
 *               |
 *         ARRAY_DECLARATOR -- EXPR
 *               |              |
 *             EXPR             1
 *               |
 *               2
 *
 */
newExpression
	:	'new' type
		(	LPAREN argList RPAREN (classBlock)?

			//java 1.1
			// Note: This will allow bad constructs like
			//    new int[4][][3] {exp,exp}.
			//    There needs to be a semantic check here...
			// to make sure:
			//   a) [ expr ] and [ ] are not mixed
			//   b) [ expr ] and an init are not used together

		|	newArrayDeclarator (arrayInitializer)?
		)
	;

argList
	:	(	expressionList
		|
		)
	;

newArrayDeclarator
	:	(LBRACK
				(expression)?
			RBRACK
		)+
	;

constant
	:	NUM_INT
	|	CHAR_LITERAL
	|	STRING_LITERAL
	|	NUM_FLOAT
	|	NUM_LONG
	|	NUM_DOUBLE
	; 
FINAL : 'final';
 
ABSTRACT : 'abstract';
 
STRICTFP : 'strictfp';




// OPERATORS
QUESTION		:	'?'		;
LPAREN			:	'('		;
RPAREN			:	')'		;
LBRACK			:	'['		;
RBRACK			:	']'		;
LCURLY			:	'{'		;
RCURLY			:	'}'		;
COLON			:	':'		;
COMMA			:	','		;
//DOT			:	'.'		;
ASSIGN			:	'='		;
EQUAL			:	'=='	;
LNOT			:	'!'		;
BNOT			:	'~'		;
NOT_EQUAL		:	'!='	;
DIV				:	'/'		;
DIV_ASSIGN		:	'/='	;
PLUS			:	'+'		;
PLUS_ASSIGN		:	'+='	;
INC				:	'++'	;
MINUS			:	'-'		;
MINUS_ASSIGN	:	'-='	;
DEC				:	'--'	;
STAR			:	'*'		;
STAR_ASSIGN		:	'*='	;
MOD				:	'%'		;
MOD_ASSIGN		:	'%='	;
SR				:	'>>'	;
SR_ASSIGN		:	'>>='	;
BSR				:	'>>>'	;
BSR_ASSIGN		:	'>>>='	;
GE				:	'>='	;
GT				:	'>'		;
SL				:	'<<'	;
SL_ASSIGN		:	'<<='	;
LE				:	'<='	;
LT				:	'<'		;
BXOR			:	'^'		;
BXOR_ASSIGN		:	'^='	;
BOR				:	'|'		;
BOR_ASSIGN		:	'|='	;
LOR				:	'||'	;
BAND			:	'&'		;
BAND_ASSIGN		:	'&='	;
LAND			:	'&&'	;
SEMI			:	';'		;


// Whitespace -- ignored
WS	:	(	' '
		|	'\t'
		|	'\f'
			// handle newlines
		|	(	'\r\n'  // Evil DOS
			|	'\r'    // Macintosh
			|	'\n'    // Unix (the right way)
			)
		)+
	;

// Single-line comments
SL_COMMENT
	:	'//'
		(~('\n'|'\r'))* ('\n'|'\r'('\n')?)
	;

// multiple-line comments
ML_COMMENT
	:	'/*'
		(
			{ LA(2)!='/' }? '*'
		|	'\r' '\n'
		|	'\r'
		|	'\n'
		|	~('*'|'\n'|'\r')
		)*
		'*/'
	;


// character literals
CHAR_LITERAL
	:	'\'' ( ESC | ~('\''|'\n'|'\r'|'\\') ) '\''
	;

// string literals
STRING_LITERAL
	:	'"' (ESC|~('"'|'\\'|'\n'|'\r'))* '"'
	;
ESC
	:	'\\'
		(	'n'
		|	'r'
		|	't'
		|	'b'
		|	'f'
		|	'"'
		|	'\''
		|	'\\'
		|	('u')+ HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT
		|	'0'..'3'
			(	'0'..'7'
				(	'0'..'7'
				)?
			)?
		|	'4'..'7'
			(	'0'..'7'
			)?
		)
	;
HEX_DIGIT
	:	('0'..'9'|'A'..'F'|'a'..'f')
	;
VOCAB
	:	'\3'..'\377'
	;


// an identifier.  Note that testLiterals is set to true!  This means
// that after we match the rule, we look in the literals table to see
// if it's a literal or really an identifer
IDENT
	:	('a'..'z'|'A'..'Z'|'_'|'$') ('a'..'z'|'A'..'Z'|'_'|'0'..'9'|'$')*
	;


// a numeric literal
NUM_INT
    :   '.'
            (	('0'..'9')+ (EXPONENT)? (FLOAT_SUFFIX)?
            )?

	|	(	'0' // special case for just '0'
			(	('x'|'X')
				(	HEX_DIGIT
				)+

			| ('0'..'9')+

			|	('0'..'7')+									// octal
			)?
		|	('1'..'9') ('0'..'9')*		// non-zero decimal
		)
		(	('l'|'L')

		// only check to see if it's a float if looks like decimal so far
		|	{isDecimal}?
            (   '.' ('0'..'9')* (EXPONENT)? (FLOAT_SUFFIX)?
            |   EXPONENT (FLOAT_SUFFIX)?
            |FLOAT_SUFFIX
            )
        )?
	;
EXPONENT
	:	('e'|'E') ('+'|'-')? ('0'..'9')+
	;
FLOAT_SUFFIX
	:	'f'|'F'|'d'|'D'
	;
