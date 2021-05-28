

/**
[The "BSD licence"]
Copyright (c) 2002-2005 Kunle Odutola
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:
1. Redistributions of source code must retain the above copyright
notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
notice, this list of conditions and the following disclaimer in the
documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


/// <summary>
/// A Parser for the C# language (including preprocessors directives).
/// </summary>
///
/// <remarks>
/// <para>
/// The Parser defined below is based on the "C# Language Specification" as documented in 
/// the ECMA-334 standard dated December 2001.
/// </para>
///
/// <para>
/// One notable feature of this parser is that it can handle input that includes "normalized"
/// C# preprocessing directives. In the simplest sense, normalized C# preprocessing directives 
/// are directives that can be safely deleted from a source file without triggering any parsing 
/// errors due to incomplete statements etc.
/// </para>
///
/// <para>
/// The Abstract Syntax Tree that this parser constructs has special nodes that represents
//  all the C# preprocessor directives defined in the ECMA-334 standard.
/// </para>
///
/// <para>
/// History
/// </para>
///
/// <para>
/// 31-May-2002 kunle	  Started work in earnest
/// 09-Feb-2003 kunle     Separated Parser from the original combined Lexer/Parser grammar file<br/>
/// 20-Oct-2003 kunle     Removed STMT_LIST from inside BLOCK nodes. A BLOCK node now directly contains
///						  a list of statements. Finding the AST nodes that correspond to a selection
///						  should now be easier.<br/>
/// </para>
///
/// </remarks>

*/
parser grammar CSharpParser;

options
{							// two tokens of lookahead
	importVocab						= CSharpLexer;
}

//=============================================================================
// Start of RULES
//=============================================================================

//
// C# LANGUAGE SPECIFICATION
//
// A.2 Syntactic grammar 
//
// The start rule for this grammar is 'compilationUnit'
//

//
// A.2.1 Basic concepts
//

nonKeywordLiterals
	:	'add'
	|	'remove'
	|	'get'
	|	'set'
	|	'assembly'
	|	'field'
	|	'method'
	|	'module'
	|	'param'
	|	'property'
	|	'type'
	;
	
identifier
	:	IDENTIFIER
	|nonKeywordLiterals
	;
	
qualifiedIdentifier
	:	identifier	
		(
			DOT qualifiedIdentifier
		)?
	;

/*
//
// A.2.2 Types
//
*/
type
	:	(
			(predefinedTypeName |qualifiedIdentifier )	// typeName
			(STAR
			)*

		|VOIDSTAR
		)rankSpecifiers
	;
	
integralType
	:	(	SBYTE
		|	BYTE
		|	SHORT
		|	USHORT
		|	INT
		|	UINT
		|	LONG
		|	ULONG
		|	CHAR
		)
	;
	
classType
	:	(	qualifiedIdentifier		// typeName
		|	OBJECT
		|	STRING
		)
	;

/*	
//
// A.2.4 Expressions
//
*/
argumentList
	:	argument ( COMMA argument )*
	;
	
argument
	:	expression
	|	REF expression
	|	OUT expression
	;

constantExpression
	:	expression
	;
	
booleanExpression
	:	expression
	;
	
expressionList
	:	expression ( COMMA expression )*
	;


/*	
//======================================
// 14.2.1 Operator precedence and associativity
//
// The following table summarizes all operators in order of precedence from lowest to highest:
//
// PRECEDENCE     SECTION  CATEGORY                     OPERATORS
//  lowest  (14)  14.13    Assignment                   = *= /= %= += -= <<= >>= &= ^= |=
//          (13)  14.12    Conditional                  ?:
//          (12)  14.11    Conditional OR               ||
//          (11)  14.11    Conditional AND              &&
//          (10)  14.10    Logical OR                   |
//          ( 9)  14.10    Logical XOR                  ^
//          ( 8)  14.10    Logical AND                  &
//          ( 7)  14.9     Equality                     == !=
//          ( 6)  14.9     Relational and type-testing  < > <= >= is as
//          ( 5)  14.8     Shift                        << >>
//          ( 4)  14.7     Additive                     +{binary} -{binary}
//          ( 3)  14.7     Multiplicative               * / %
//          ( 2)  14.6     Unary                        +{unary} -{unary} ! ~ ++x --x (T)x
//  highest ( 1)  14.5     Primary                      x.y f(x) a[x] x++ x-- new
//                                                      typeof checked unchecked
//
// NOTE: In accordance with lessons gleaned from the "java.g" file supplied with ANTLR, 
//       I have applied the following pattern to the rules for expressions:
// 
//           thisLevelExpression :
//               nextHigherPrecedenceExpression (OPERATOR nextHigherPrecedenceExpression)*
//
//       which is a standard recursive definition for a parsing an expression.
//
*/
expression
	:	assignmentExpression
	;

assignmentExpression
	:	conditionalExpression 
		(	(	ASSIGN
			|	PLUS_ASSIGN
			|	MINUS_ASSIGN
			|	STAR_ASSIGN
			|	DIV_ASSIGN
			|	MOD_ASSIGN
			|	BIN_AND_ASSIGN
			|	BIN_OR_ASSIGN
			|	BIN_XOR_ASSIGN
			|	SHIFTL_ASSIGN
			|	SHIFTR_ASSIGN
			) 
			assignmentExpression 
		)?		
	;
	
conditionalExpression
	:	conditionalOrExpression ( QUESTION assignmentExpression
	                              COLON    conditionalExpression
	                            )?	
	;
	
conditionalOrExpression
	:	conditionalAndExpression (	LOG_OR conditionalAndExpression )*
	
	;

conditionalAndExpression
	:	inclusiveOrExpression (	LOG_AND inclusiveOrExpression )*
	
	;
	
inclusiveOrExpression
	:	exclusiveOrExpression ( BIN_OR exclusiveOrExpression )*
		
	;
	
exclusiveOrExpression
	:	andExpression ( BIN_XOR andExpression )*
	;
	
andExpression
	:	equalityExpression (	BIN_AND equalityExpression )*
	;
	
equalityExpression
	:	relationalExpression ( ( EQUAL | NOT_EQUAL ) relationalExpression )*
	;
	
relationalExpression
	:	shiftExpression 
		(	( ( LTHAN | GTHAN | LTE | GTE ) shiftExpression )*
		|	( IS | AS ) type
		)
	;
	
shiftExpression
	:	additiveExpression (	( SHIFTL | SHIFTR ) additiveExpression )*
	;
	
additiveExpression
	:	multiplicativeExpression (	( PLUS | MINUS ) multiplicativeExpression )*
	;	

multiplicativeExpression
	:	unaryExpression ( ( STAR | DIV | MOD ) unaryExpression )*
	;
	
unaryExpression
	:OPEN_PAREN type CLOSE_PAREN unaryExpression
	|	// preIncrementExpression
		//
		INC unaryExpression
	|	// preDecrementExpression
		//
		DEC unaryExpression
	|PLUS unaryExpression
	|MINUS unaryExpression
	|	LOG_NOT unaryExpression
	|	BIN_NOT unaryExpression
	|STAR unaryExpression
	|BIN_AND unaryExpression
	|	primaryExpression
	;
	
basicPrimaryExpression
		// primaryNoArrayCreationExpression		
		//
	:	(	literal
		|	identifier											// simpleName
		|OPEN_PAREN assignmentExpression CLOSE_PAREN
		|	THIS												// thisAccess
		|	BASE
			(	DOT identifier									// baseAccess
			|	OPEN_BRACK expressionList CLOSE_BRACK			// baseAccess
			)			
		|	newExpression
		|!TYPEOF OPEN_PAREN
			(	{ ((LA(1) == VOID) && (LA(2) == CLOSE_PAREN)) }?voidAsType CLOSE_PAREN
			|type CLOSE_PAREN
			)
		|	SIZEOF    OPEN_PAREN qualifiedIdentifier CLOSE_PAREN	// sizeofExpression
		|	CHECKED   OPEN_PAREN expression          CLOSE_PAREN	// checkedExpression
		|	UNCHECKED OPEN_PAREN expression          CLOSE_PAREN	// uncheckedExpression
		|!predefinedTypeDOTidentifier
		)
	;

primaryExpression
	:basicPrimaryExpression
		( 
			(OPEN_PAREN (argumentList )? CLOSE_PAREN
			|OPEN_BRACKexpressionList CLOSE_BRACK
			|DOTidentifier
			|INC
			|DEC
			|DEREFidentifier
			)
		)*															
	;

newExpression
	:NEWtype
		(	// objectCreationExpression	  ::= NEW type         OPEN_PAREN ( argumentList )? CLOSE_PAREN
			// delegateCreationExpression ::= NEW delegateType OPEN_PAREN expression        CLOSE_PAREN
			// NOTE: Will ALSO match 'delegateCreationExpression'
			//
			OPEN_PAREN (argumentList )? CLOSE_PAREN
		|arrayInitializer
		|	// arrayCreationExpression 	::= NEW nonArrayType OPEN_BRACK expressionList CLOSE_BRACK ( rankSpecifiers )? ( arrayInitializer )?
			//
			OPEN_BRACKexpressionList CLOSE_BRACKrankSpecifiers
			(arrayInitializer )?
		)
	;

literal
	:	TRUE							// BOOLEAN_LITERAL
	|	FALSE							// BOOLEAN_LITERAL
	|	INT_LITERAL
	|	UINT_LITERAL
	|	LONG_LITERAL
	|	ULONG_LITERAL
	|	DECIMAL_LITERAL
	|	FLOAT_LITERAL
	|	DOUBLE_LITERAL
	|	CHAR_LITERAL
	|	STRING_LITERAL
	|	NULL							// NULL_LITERAL
	;

predefinedType
	:	(	BOOL 
		|	BYTE
		|	CHAR
		|	DECIMAL
		|	DOUBLE
		|	FLOAT
		|	INT
		|	LONG
		|	OBJECT
		|	SBYTE
		|	SHORT
		|	STRING
		|	UINT
		|	ULONG
		|	USHORT
		)
	;
	
predefinedTypeName
	:	BOOL 
	|	BYTE
	|	CHAR
	|	DECIMAL
	|	DOUBLE
	|	FLOAT
	|	INT
	|	LONG
	|	OBJECT
	|	SBYTE
	|	SHORT
	|	STRING
	|	UINT
	|	ULONG
	|	USHORT
	;
	

/*
//
// A.2.5 Statements
//
*/
statement
	:	{ (IdentifierRuleIsPredictedByLA(1) && (LA(2) == COLON)) }? labeledStatement
	|	{ ((LA(1) == CONST) && TypeRuleIsPredictedByLA(2) && IdentifierRuleIsPredictedByLA(3)) ||
		  (TypeRuleIsPredictedByLA(1) && IdentifierRuleIsPredictedByLA(2)) }? declarationStatement
	| declarationStatement
	|	embeddedStatement
	|	preprocessorDirective
	;
	
embeddedStatement
	:	block
	|SEMI
	|	expressionStatement
	|	selectionStatement
	|	iterationStatement
	|	jumpStatement
	|	tryStatement
	|	checkedStatement
	|	uncheckedStatement
	|	lockStatement
	|	usingStatement
	|	unsafeStatement
	|	fixedStatement
	;

	
body
	:	block	
	|SEMI
	;

block
	:OPEN_CURLY ( statement )* CLOSE_CURLY
	;
	
statementList
	:	(	statement )+
	;
	
labeledStatement
	:identifierCOLONstatement
	;
	
declarationStatement
	:	localVariableDeclaration SEMI
	|	localConstantDeclaration SEMI
	;
	
localVariableDeclaration
	:	type localVariableDeclarators
	;
	
localVariableDeclarators
	:	localVariableDeclarator
		( 
			COMMA localVariableDeclarator
		)*
	;
	
localVariableDeclarator
	:	identifier ( ASSIGN localVariableInitializer )?
	;
	
localVariableInitializer
	:	(	expression
		|	arrayInitializer
		)
	;
	
localConstantDeclaration
	:CONST type constantDeclarators
	;
	
constantDeclarators
	:	constantDeclarator
		(
			COMMA constantDeclarator
		)*
	;
	
constantDeclarator
	:	identifierASSIGN constantExpression
	;
	
expressionStatement
	:	statementExpressionSEMI
	;
	
statementExpression
	:assignmentExpression
/*
	:	invocationExpression
	|	objectCreationExpression
	|	assignmentExpression
	|	postIncrementExpression
	|	postDecrementExpression
	|	preIncrementExpression
	|	preDecrementExpression
*/
	;
	
selectionStatement
	:	ifStatement
	|	switchStatement
	;
	
ifStatement
	:	IF OPEN_PAREN booleanExpression CLOSE_PAREN embeddedStatement 
		( elseStatement )?
	;
	
elseStatement
	:	ELSE embeddedStatement
	;
	
switchStatement
	:	SWITCH OPEN_PAREN expression CLOSE_PAREN switchBlock
	;
	
switchBlock
	:	OPEN_CURLY ( switchSections )? CLOSE_CURLY
	;
	
switchSections
	:	( switchSection )+
	;
	
switchSection
	:switchLabelsstatementList
	;
	
switchLabels
	:	( switchLabel )+
	;
	
switchLabel
	:	CASE constantExpression COLON
	|	DEFAULT COLON
	;
	
iterationStatement
	:	whileStatement
	|	doStatement
	|	forStatement
	|	foreachStatement
	;
	
whileStatement
	:	WHILE OPEN_PAREN booleanExpression CLOSE_PAREN embeddedStatement
	;
	
doStatement
	:	DO embeddedStatement WHILE OPEN_PAREN booleanExpression CLOSE_PAREN SEMI
	;
	
forStatement
	:	FOR OPEN_PAREN forInitializer SEMI forCondition SEMI forIterator CLOSE_PAREN embeddedStatement
	;
	
forInitializer
	:	(	{ (TypeRuleIsPredictedByLA(1) && IdentifierRuleIsPredictedByLA(2)) }? localVariableDeclaration
		| localVariableDeclaration
		|	statementExpressionList
		)?
	;
	
forCondition
	:	(	booleanExpression
		)?
	;
	
forIterator
	:	(	statementExpressionList
		)?
	;
	
statementExpressionList
	:	statementExpression ( COMMA statementExpression )*
	;
	
foreachStatement
	:FOREACH OPEN_PARENtypeidentifier INexpression CLOSE_PARENembeddedStatement
	;
	
jumpStatement
	:	breakStatement
	|	continueStatement
	|	gotoStatement
	|	returnStatement
	|	throwStatement
	;
	
breakStatement
	:	BREAK SEMI
	;
	
continueStatement
	:	CONTINUE SEMI
	;
	
gotoStatement
	:	GOTO 
		(	identifier SEMI
		|	CASE constantExpression SEMI
		|	DEFAULT SEMI
		)
	;
	
returnStatement
	:	RETURN ( expression )? SEMI
	;
	
throwStatement
	:	THROW ( expression )? SEMI
	;
	
tryStatement
	:	TRY block 
		(	finallyClause
		|	catchClauses ( finallyClause )?
		)
	;
	
catchClauses
	:	( specificCatchClause )+ ( generalCatchClause )?
	|	generalCatchClause
	;
	
specificCatchClause
	:CATCH OPEN_PARENclassType (identifier )? CLOSE_PARENblock
	;
	
generalCatchClause
	:	CATCH block
	;
	
finallyClause
	:	FINALLY block
	;
	
checkedStatement
	:	CHECKED block
	;
	
uncheckedStatement
	:	UNCHECKED block
	;
	
lockStatement
	:	LOCK OPEN_PAREN expression CLOSE_PAREN embeddedStatement
	;
	
usingStatement
	:	USING OPEN_PAREN resourceAcquisition CLOSE_PAREN embeddedStatement
	;
	
unsafeStatement
	:	UNSAFE block
	;

resourceAcquisition
	:	{ (TypeRuleIsPredictedByLA(1) && IdentifierRuleIsPredictedByLA(2)) }? localVariableDeclaration
	| localVariableDeclaration
	|	expression
	;
	
compilationUnit
	:	justPreprocessorDirectives
		usingDirectives
		globalAttributes
		namespaceMemberDeclarations
	    EOF
	;
	
usingDirectives
	:	(	{ !PPDirectiveIsPredictedByLA(1) }? usingDirective 
		| 
			preprocessorDirective
		)*
	;
	
usingDirective
	:USING
		(	// UsingAliasDirective
			{ (IdentifierRuleIsPredictedByLA(1) && (LA(2) == ASSIGN)) }? identifier ASSIGN qualifiedIdentifier SEMI
		|	// UsingNamespaceDirective
			qualifiedIdentifier SEMI
		)
	;
		
namespaceMemberDeclarations
	:	(	{ PPDirectiveIsPredictedByLA(1) }? preprocessorDirective
		|	namespaceMemberDeclaration 
		)*
	;
	
namespaceMemberDeclaration
	:	namespaceDeclaration
	|attributesmodifiers typeDeclaration
	;
	
typeDeclaration
	:	classDeclaration
	|	structDeclaration
	|	interfaceDeclaration
	|	enumDeclaration
	|	delegateDeclaration
	;

namespaceDeclaration
	:	NAMESPACE qualifiedIdentifier namespaceBody ( SEMI )?
	;
	
namespaceBody
	:OPEN_CURLY
			usingDirectives 
			namespaceMemberDeclarations 
		CLOSE_CURLY
	;
	
modifiers
	:	( modifier )*
	;

modifier
	:	(	ABSTRACT
		|	NEW
		|	OVERRIDE
		|	PUBLIC
		|	PROTECTED
		|	INTERNAL
		|	PRIVATE
		|	SEALED
		|	STATIC
		|	VIRTUAL
		|	EXTERN
		|	READONLY
		|	UNSAFE
		|	VOLATILE
		)
	;


//	
// A.2.6 Classes
//

classDeclaration
	:CLASSidentifierclassBaseclassBody ( SEMI )?
	;
	
classBase
	:	(	COLON type ( COMMA type )*
		)?
	;
	
classBody
	:OPEN_CURLY classMemberDeclarations CLOSE_CURLY
	;
	
classMemberDeclarations		
	:	(	{ PPDirectiveIsPredictedByLA(1) }? preprocessorDirective
		|	classMemberDeclaration 
		)*
	;
	
classMemberDeclaration
	:attributesmodifiers
		(	destructorDeclaration
		|	typeMemberDeclaration
		)
	;
	
typeMemberDeclaration
	:!CONSTtypeconstantDeclarators SEMI
		
	|!EVENTtype 
		(	{ IdentifierRuleIsPredictedByLA(1) && (LA(2)==ASSIGN || LA(2)==SEMI ||LA(2)==COMMA) }?variableDeclarators SEMI
		|qualifiedIdentifier OPEN_CURLYeventAccessorDeclarationsCLOSE_CURLY
		)
		
	|!identifier OPEN_PAREN (formalParameterList )? CLOSE_PAREN 
		(constructorInitializer )?constructorBody

	|!	// methodDeclaration
		{ ((LA(1) == VOID) && (LA(2) != STAR)) }?voidAsTypequalifiedIdentifierOPEN_PAREN (formalParameterList )? CLOSE_PARENmethodBody
		
	|!type
		(	// unaryOperatorDeclarator or binaryOperatorDeclarator
			OPERATORoverloadableOperator OPEN_PARENfixedOperatorParameter 
				(	COMMAfixedOperatorParameter 
				)?
			CLOSE_PARENoperatorBody
		|
			// fieldDeclaration
			{ IdentifierRuleIsPredictedByLA(1) && (LA(2)==ASSIGN || LA(2)==SEMI ||LA(2)==COMMA) }?variableDeclarators SEMI
		|qualifiedIdentifier
		
			(	// propertyDeclaration
				OPEN_CURLYaccessorDeclarationsCLOSE_CURLY
				
			|	// methodDeclaration
				OPEN_PAREN (formalParameterList )? CLOSE_PARENmethodBody
				
			|	// indexerDeclaration
				DOTTHIS OPEN_BRACKformalParameterList CLOSE_BRACK
				OPEN_CURLYaccessorDeclarationsCLOSE_CURLY
			)
			
		|THIS OPEN_BRACKformalParameterList CLOSE_BRACK
			OPEN_CURLYaccessorDeclarationsCLOSE_CURLY
		)
	
	|!IMPLICIT OPERATORtype OPEN_PARENoneOperatorParameter CLOSE_PARENoperatorBody

	|!EXPLICIT OPERATORtype OPEN_PARENoneOperatorParameter CLOSE_PARENoperatorBody

	|	typeDeclaration
	;
	
variableDeclarators
	:	variableDeclarator
		( 
			COMMA variableDeclarator
		)*
	;
	
variableDeclarator
	:	identifier ( ASSIGN variableInitializer )?
	;
	
variableInitializer
	:	(	expression
		|	arrayInitializer
		|	stackallocInitializer
		)
	;
		
returnType
	:	{ ((LA(1) == VOID) && (LA(2) != STAR)) }? voidAsType
	|	type
	;
	
methodBody
	:body
	;
	
formalParameterList
	:attributes
		(	fixedParameters ( COMMAattributes parameterArray )?
		|	parameterArray
		)
	;
	
fixedParameters
	:	fixedParameter ( COMMAattributes fixedParameter )*
	;
	
fixedParameter
	:	(parameterModifier )?typeidentifier
	;
	
parameterModifier
	:	REF
	|	OUT
	;
	
parameterArray
//	:	PARAMS! arrayType identifier
	:PARAMStypeidentifier
	;
	
accessorDeclarations
	:attributes
		(getAccessorDeclaration
			(attributessetAccessorDeclaration 
			)?
		|setAccessorDeclaration
			(attributesgetAccessorDeclaration
			)?
		)
	;
	
getAccessorDeclaration
	:'get'accessorBody
	;
	
setAccessorDeclaration
	:'set'accessorBody
	;
	
accessorBody
	:	body
	;
	
eventAccessorDeclarations
	:attributes
		(addAccessorDeclarationattributesremoveAccessorDeclaration
		|removeAccessorDeclarationattributesaddAccessorDeclaration
		)
	;
	
addAccessorDeclaration
	:'add'block
	;
	
removeAccessorDeclaration
	:'remove'block
	;

overloadableOperator
		// Unary-or-Binary Operators
		//
	:	PLUS
	|	MINUS
	
		// Unary-only Operators
		//
	|	LOG_NOT
	|	BIN_NOT
	|	INC
	|	DEC
	|	TRUE		//"true"
	|	FALSE		//"false"

		// Binary-only Operators
		//
	|	STAR 
	|	DIV 
	|	MOD 
	|	BIN_AND 
	|	BIN_OR 
	|	BIN_XOR 
	|	SHIFTL 
	|	SHIFTR 
	|	EQUAL 
	|	NOT_EQUAL 
	|	GTHAN
	|	LTHAN 
	|	GTE 
	|	LTE 
	;
	
oneOperatorParameter
	:	fixedOperatorParameter
	;
	
fixedOperatorParameter
	:typeidentifier
	;
	
operatorBody
	:body
	;

constructorInitializer
	:COLON
		(	BASE OPEN_PAREN ( argumentList )? CLOSE_PAREN
		|	THIS OPEN_PAREN ( argumentList )? CLOSE_PAREN
		)
	;
	
constructorBody
	:body
	;

destructorDeclaration
	:BIN_NOTidentifier OPEN_PAREN CLOSE_PARENdestructorBody
	;
	
destructorBody
	:body
	;

	
//
// A.2.7 Structs
//

structDeclaration
	:STRUCTidentifierstructInterfacesstructBody ( SEMI )?
	;
	
structInterfaces
	:	( COLON type ( COMMA type )* )?
	;
	
structBody
	:OPEN_CURLY structMemberDeclarations CLOSE_CURLY
;
	
structMemberDeclarations
	:	(	{ PPDirectiveIsPredictedByLA(1) }? preprocessorDirective
		|	structMemberDeclaration
		)*
	;
	
structMemberDeclaration
	:attributesmodifiers typeMemberDeclaration
	;

	
//
// A.2.8 Arrays
//

nonArrayType
	:	type
	;
	
rankSpecifiers
	:	// CONFLICT:	ANTLR says this about this line:
		//						ECMA-CSharp.g:1295: warning: nondeterminism upon
		//						ECMA-CSharp.g:1295: 	k==1:OPEN_BRACK
		//						ECMA-CSharp.g:1295: 	k==2:COMMA,CLOSE_BRACK
		//						ECMA-CSharp.g:1295: 	between alt 1 and exit branch of block
		//						!FIXME! -- if possible, can't see the problem right now.
		( rankSpecifier )*
	;
	
rankSpecifier
	:OPEN_BRACK ( COMMA )* CLOSE_BRACK
	;
	
arrayInitializer
	:OPEN_CURLY
		(	CLOSE_CURLY
		|	variableInitializerList (COMMA)? CLOSE_CURLY
		)
	;
	
variableInitializerList
	:	variableInitializer ( COMMA variableInitializer )*
	;

	
// 
// A.2.9 Interfaces
//

interfaceDeclaration
	:INTERFACEidentifierinterfaceBaseinterfaceBody ( SEMI )?
	;
	
interfaceBase
	:	( COLON type ( COMMA type )* )?
	;
	
interfaceBody
	:OPEN_CURLY interfaceMemberDeclarations CLOSE_CURLY
	;
	
interfaceMemberDeclarations
	:	(	{ PPDirectiveIsPredictedByLA(1) }? preprocessorDirective
		|	interfaceMemberDeclaration
		)*
	;
	
interfaceMemberDeclaration
	:attributes (NEW )? 	
		(!	// interfaceMethodDeclaration
			{ ((LA(1) == VOID) && (LA(2) != STAR)) }?voidAsTypeidentifier OPEN_PAREN (formalParameterList )? CLOSE_PARENSEMI

		|!type 
			(THIS OPEN_BRACKformalParameterList CLOSE_BRACK 
				OPEN_CURLYinterfaceAccessorsCLOSE_CURLY
				
			|identifier
				(	// interfaceMethodDeclaration
					OPEN_PAREN (formalParameterList )? CLOSE_PARENSEMI
					
				|	// interfacePropertyDeclaration
					OPEN_CURLYinterfaceAccessorsCLOSE_CURLY
				)
			)
			
		|EVENTtypeidentifier SEMI
		)
	;
	

interfaceAccessors
	:attributes
		(getAccessorDeclaration
			(attributessetAccessorDeclaration 
			)?
		|setAccessorDeclaration
			(attributesgetAccessorDeclaration 
			)?
		)
	;
	


//
//	A.2.10 Enums
//

enumDeclaration
	:ENUMidentifierenumBaseenumBody ( SEMI )?
	;
	
enumBase
	:	(COLONintegralType )?
	;
	
enumBody
	:OPEN_CURLY ( enumMemberDeclarations ( COMMA )? )? CLOSE_CURLY
	;
	
enumMemberDeclarations
	:attributes enumMemberDeclaration 
		( 
			COMMAattributes enumMemberDeclaration 
		)*
 	;
 	
enumMemberDeclaration
	:identifier ( ASSIGNconstantExpression )?
 	;


//
// A.2.11 Delegates
//

delegateDeclaration
	:DELEGATE 
		( 	{ ((LA(1) == VOID) && IdentifierRuleIsPredictedByLA(2)) }?voidAsType
		|type
		)identifier OPEN_PAREN (formalParameterList )? CLOSE_PAREN SEMI
	;
	

//
// A.2.12 Attributes
//

globalAttributes
	:	(	{ !PPDirectiveIsPredictedByLA(1) }? globalAttributeSection
		|
			preprocessorDirective
		)*
	;
	
globalAttributeSection
	:OPEN_BRACK 
			'assembly' COLON attributeList ( COMMA )? 
		CLOSE_BRACK
	;

attributes
	:	(	{ !PPDirectiveIsPredictedByLA(1) }? attributeSection 
		|
			preprocessorDirective
		)*
	;
	
attributeSection
	:OPEN_BRACK 
			( attributeTarget COLON )? attributeList ( COMMA )? 
		CLOSE_BRACK
	;
	
attributeTarget
	:	'field'
	|	EVENT
	|	'method'
	|	'module'
	|	'param'
	|	'property'
	|	RETURN
	|	'type'
	;

attributeList
	:	attribute ( COMMA attribute )*
	;
	
attribute
	:	( predefinedTypeName | qualifiedIdentifier ) ( attributeArguments )?
	;
	
attributeArguments
	:	OPEN_PAREN
		(	CLOSE_PAREN
		|	{ (IdentifierRuleIsPredictedByLA(1) && (LA(2) == ASSIGN)) }? namedArgumentList CLOSE_PAREN
		|	positionalArgumentList ( COMMA namedArgumentList )? CLOSE_PAREN
		)
	;
	
positionalArgumentList
	:	positionalArgument 
		(	COMMA positionalArgument 
		)*
	;
	
positionalArgument
	:	attributeArgumentExpression
	;
	
namedArgumentList
	:	namedArgument ( COMMA namedArgument )*
	;
	
namedArgument
	:	identifier ASSIGN attributeArgumentExpression
	;
	
attributeArgumentExpression
	:	expression
	;

//
// A.3 Grammar extensions for unsafe code
// 

fixedStatement
//	:	FIXED^ OPEN_PAREN! pointerType fixedPointerDeclarators CLOSE_PAREN! embeddedStatement
	:	FIXED OPEN_PAREN type        fixedPointerDeclarators CLOSE_PAREN embeddedStatement
	;
	
fixedPointerDeclarators
	:	fixedPointerDeclarator ( COMMA fixedPointerDeclarator )*
	;
	
fixedPointerDeclarator
	:	identifier ASSIGN fixedPointerInitializer
	;
	
fixedPointerInitializer
	:	expression
	;	
	
stackallocInitializer
	:	STACKALLOC qualifiedIdentifier OPEN_BRACK expression CLOSE_BRACK
	;

//
// A.1.10 Pre-processing directives
// 

justPreprocessorDirectives
	:	(	{ SingleLinePPDirectiveIsPredictedByLA(1) }? singleLinePreprocessorDirective
		| 
			preprocessorDirective
		)*
	;
	
preprocessorDirective
	:	PP_DEFINE   PP_IDENT
	|	PP_UNDEFINE PP_IDENT
	|	lineDirective
	|	PP_ERROR   ppMessage
	|	PP_WARNING ppMessage
	|	regionDirective
	|	conditionalDirective
	;
	
singleLinePreprocessorDirective
	:	PP_DEFINE   PP_IDENT
	|	PP_UNDEFINE PP_IDENT
	|	lineDirective
	|	PP_ERROR   ppMessage
	|	PP_WARNING ppMessage
	;
	
lineDirective
	:	PP_LINE
		(	DEFAULT
		|	PP_NUMBER ( PP_FILENAME )?
		)
	;

regionDirective
	:PP_REGIONppMessagedirectiveBlockPP_ENDREGIONppMessage
	;

conditionalDirective
	:PP_COND_IFpreprocessExpressiondirectiveBlock
		
		(PP_COND_ELIFpreprocessExpressiondirectiveBlock
		)*
		
		(PP_COND_ELSEdirectiveBlock
		)?PP_COND_ENDIF
	;

directiveBlock
	:	
		(	preprocessorDirective
		|	{ NotExcluded(codeMask, CodeMaskEnums.UsingDirectives) }? 				usingDirective
		|	{ NotExcluded(codeMask, CodeMaskEnums.GlobalAttributes) }? 				globalAttributeSection
		|	{ NotExcluded(codeMask, CodeMaskEnums.Attributes) }? 					attributeSection
		|	{ NotExcluded(codeMask, CodeMaskEnums.NamespaceMemberDeclarations) }? 	namespaceMemberDeclaration
		|	{ NotExcluded(codeMask, CodeMaskEnums.ClassMemberDeclarations) }? 		classMemberDeclaration
		|	{ NotExcluded(codeMask, CodeMaskEnums.StructMemberDeclarations) }? 		structMemberDeclaration
		|	{ NotExcluded(codeMask, CodeMaskEnums.InterfaceMemberDeclarations) }? 	interfaceMemberDeclaration
		|	{ NotExcluded(codeMask, CodeMaskEnums.Statements) }? 					statement
		)*
	;
	
ppMessage
	:	( PP_IDENT | PP_STRING | PP_FILENAME | PP_NUMBER )*
	;
	
	
//======================================
// 14.2.1 Operator precedence and associativity
//
// The following table summarizes all PP operators in order of precedence from lowest to highest:
//
// PRECEDENCE     SECTION  CATEGORY                     OPERATORS
//  lowest  ( 4)  14.11    Conditional OR               ||
//          ( 3)  14.11    Conditional AND              &&
//          ( 2)  14.9     Equality                     == !=
//  highest ( 1)  14.5     Primary                      (x) !x
//
// NOTE: In accordance with lessons gleaned from the "java.g" file supplied with ANTLR, I have
//       applied the following pattern to the rules for expressions:
// 
//           thisLevelExpression :
//               nextHigherPrecedenceExpression (OPERATOR nextHigherPrecedenceExpression)*
//
//       which is a standard recursive definition for a parsing an expression.
//
preprocessExpression
	:	preprocessOrExpression
	;

preprocessOrExpression
	:	preprocessAndExpression (	LOG_OR preprocessAndExpression )*
	;

preprocessAndExpression
	:	preprocessEqualityExpression (	LOG_AND preprocessEqualityExpression )*
	;
	
preprocessEqualityExpression
	:	preprocessPrimaryExpression ( ( EQUAL | NOT_EQUAL ) preprocessPrimaryExpression )*
	;
	
preprocessPrimaryExpression
	:	(keywordExceptTrueAndFalse
		|	PP_IDENT
		|	TRUE
		|	FALSE
		|	LOG_NOT preprocessPrimaryExpression
		|OPEN_PAREN preprocessOrExpression CLOSE_PAREN
		)
	;

keywordExceptTrueAndFalse
	:	ABSTRACT
	|	AS
	|	BASE
	|	BOOL
	|	BREAK
	|	BYTE
	|	CASE
	|	CATCH
	|	CHAR
	|	CHECKED  | CLASS    | CONST   | CONTINUE | DECIMAL   | DEFAULT  | DELEGATE
	|	DO       | DOUBLE   | ELSE    | ENUM     | EVENT     | EXPLICIT | EXTERN    
	|	FINALLY  | FIXED    | FLOAT   |	FOR      | FOREACH   | GOTO     | IF      
	|	IMPLICIT | IN       | INT     | INTERFACE| INTERNAL  | IS       | LOCK
	|	LONG     | NAMESPACE| NEW     | NULL     | OBJECT    | OPERATOR | OUT	    
	|	OVERRIDE | PARAMS   | PRIVATE | PROTECTED| PUBLIC    | READONLY       
	|	REF      | RETURN   | SBYTE   | SEALED   | SHORT     | SIZEOF   | STACKALLOC 
	|	STATIC   | STRING   | STRUCT  | SWITCH   | THIS      | THROW    | TRY     
	|	TYPEOF   | UINT     | ULONG   | UNCHECKED| UNSAFE    | USHORT   | USING   
	|	VIRTUAL  | VOID     | VOLATILE| WHILE
	;

voidAsType
	:VOID
	;