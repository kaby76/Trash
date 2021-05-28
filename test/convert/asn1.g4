


//*************************************************************************
//**********		PARSER DEFINITIONS
//*************************************************************************


grammar ASNParser;

// Grammar Definitions

module_definition
	:	( module_identifier)
		DEFINITIONS_KW 
		((EXPLICIT_KW
		  |IMPLICIT_KW
		  |AUTOMATIC_KW
		 ) TAGS_KW |) 
		(EXTENSIBILITY_KW IMPLIED_KW | )
		ASSIGN_OP 
		BEGIN_KW 
		module_body 
		END_KW
	; 

module_identifier
	:	((UPPER) 
		 (( obj_id_comp_lst)|) 
		)
	;

obj_id_comp_lst
	:	L_BRACE 
		( defined_value)?
		( obj_id_component)+  
		R_BRACE 
	;

obj_id_component
	: 	((NUMBER)
	|((LOWER) 
		( L_PAREN 
		 (NUMBER)
		R_PAREN ) ? )
	|( defined_value))
	;
		
tag_default
	:	(EXPLICIT_KW)
	|	(IMPLICIT_KW)
	|	(AUTOMATIC_KW)
	;
		
module_body 
//	:	(exports[module])? (imports[module])? (assignment[module])+
	:	(exports|) (imports|) ((assignment)+ |)
	;

exports
//	:	(EXPORTS_KW {module.exported=true;})   (s = symbol { module.exportList.add(s) ; })
//		(COMMA (s = symbol {module.exportList.add(s) ;} ) )*  SEMI
//	;
	:	EXPORTS_KW 
		(( symbol_list |)
		|ALL_KW) 
		SEMI
	;	
		
imports
	:	(IMPORTS_KW (((symbols_from_module)+)|)  SEMI)
	;

symbols_from_module

	:	(( symbol_list) FROM_KW	
	    (UPPER
	     ( obj_id_comp_lst
	     |( defined_value)|)))
	;
	
symbol_list
	:	(( symbol)
		(COMMA ( symbol))*) 
	; 

symbol			
	:UPPER
	|LOWER
	|macroName   	//To solve the matching of Macro Name with Keyword
	;

macroName
	:	OPERATION_KW
	|	ERROR_KW
 	|	'BIND'
 	|	'UNBIND'
 	|	'APPLICATION-SERVICE-ELEMENT'
 	|	'APPLICATION-CONTEXT'
 	|	'EXTENSION'
 	|	'EXTENSIONS'
 	|	'EXTENSION-ATTRIBUTE'
 	|	'TOKEN'
 	|	'TOKEN-DATA'
 	|	'SECURITY-CATEGORY'
 	|	'OBJECT'
 	|	'PORT'
 	|	'REFINE'
 	|	'ABSTRACT-BIND'
 	|	'ABSTRACT-UNBIND'
 	|	'ABSTRACT-OPERATION'
 	|	'ABSTRACT-ERROR'
 	|	'ALGORITHM'
 	|	'ENCRYPTED'
 	|	'SIGNED'
 	|	'SIGNATURE'
 	|	'PROTECTED'
 	|	'OBJECT-TYPE'
 	;

assignment

// Type Assignment Definition
	:	(UPPER ASSIGN_OP	(type))

// Value Assignment definition	
	|	(LOWER ( type ) ASSIGN_OP ( value))
// Definition of Macro type. Consume the definitions . No Actions

	|  UPPER 'MACRO' ASSIGN_OP BEGIN_KW (~(END_KW))* END_KW
// ***************************************************
// Define the following
// ***************************************************
//	|XMLValueAssignment 
//	|ValueSetTypeAssignment 
//	|ObjectClassAssignment 
//	|ObjectAssignment 
//	|ObjectSetAssignment 
//	|ParameterizedAssignment
	;

/*TYPES===============================================================*/

type
	:	( built_in_type)		// Builtin Type
	|	( defined_type)		// Referenced Type
//	|	(obj = useful_type)			// Referenced Type OK contained in Character_Str type
	|	( selection_type ) 		// Referenced Type	// Grammar ok Frames to be created
//	|	(obj = type_from_Object )		// Referenced Type
//	|	(obj = value_set_from_objects)	// Referenced Type
//	|   (obj = type_and_constraint )		// Constrained Type Causing Infinit Recursion. In case of
// Parsing errors this needs to be built in the assignment itself
//	|	(obj = type_with_constraint)	// Constrained Type OK in built in types
	|	( macros_type)
	;

built_in_type
	:	( any_type)
	|	( bit_string_type)
	|	( boolean_type )
	| ( character_str_type) // Not Correct
	|	( choice_type)
	|	( embedded_type) EMBEDDED_KW  PDV_KW		
	|	( enum_type) 
	|	( external_type) 
//	|	INSTANCE_KW OF_KW		
	|	( integer_type )
	|	( null_type )
//	|	ObjectClassFieldType OBJECT_DESCRIPTOR_KW
	|	( object_identifier_type)
	|	( octetString_type)
	|	( real_type )
	|	( relativeOid_type)
	|	( sequence_type)
	|	( sequenceof_type)
	|	( set_type)
	|	( setof_type)
	|	( tagged_type)
	;
			
any_type
	: (	ANY_KW  ( DEFINED_KW BY_KWLOWER)? )
	;
	
bit_string_type
	:	(BIT_KW STRING_KW (namedNumber_list)? 
		( constraint )? )
	;

// Includes Useful types as well
character_str_type
	:	((CHARACTER_KW STRING_KW)
	|	( character_set 
		( constraint)? ))
	;
		
character_set
	:	(BMP_STR_KW)
	|	(GENERALIZED_TIME_KW)
	|	(GENERAL_STR_KW)
	|	(GRAPHIC_STR_KW)
	|	(IA5_STRING_KW)
	|	(ISO646_STR_KW)
	|	(NUMERIC_STR_KW)
	|	(PRINTABLE_STR_KW)
	|	(TELETEX_STR_KW)
	|	(T61_STR_KW)
	|	(UNIVERSAL_STR_KW)
	|	(UTF8_STR_KW)
	|	(UTC_TIME_KW)
	|	(VIDEOTEX_STR_KW)
	|	(VISIBLE_STR_KW)
	;		
boolean_type
	: BOOLEAN_KW
	;
				
choice_type
	: (	CHOICE_KW L_BRACE ( elementType_list) R_BRACE )
	;

embedded_type
	:	(EMBEDDED_KW  PDV_KW)
	;
enum_type
	: ( ENUMERATED_KW ( namedNumber_list) )	
	;
		
external_type
	: EXTERNAL_KW
	;

integer_type
	: (	INTEGER_KW ( namedNumber_list
		| constraint)? )
	;
		
null_type
	: NULL_KW
	;

object_identifier_type
	: OBJECT_KW IDENTIFIER_KW	
	; 
	
octetString_type
	: (	OCTET_KW STRING_KW ( constraint)? )
	;

real_type
	: REAL_KW		
	;

relativeOid_type
	: RELATIVE_KW MINUS OID_KW
	;
		
sequence_type
	:  ( SEQUENCE_KW 
	    L_BRACE 
	   ( elementType_list)? 
	    R_BRACE )
	;
		
sequenceof_type
	:  ( SEQUENCE_KW
	        (SIZE_KW( constraint))? OF_KW 
		( type) )		
	;
	
set_type
	:  ( SET_KW L_BRACE (  elementType_list)? R_BRACE )
	;
		
setof_type
	:	(SET_KW	
	(SIZE_KW( constraint))? OF_KW 
		( type) ) 		
	;

tagged_type
	:	(( tag) 
		( tag_default)? 
		( type))
	;


tag 
	:	(L_BRACKET ( clazz)? ( class_NUMBER) R_BRACKET )
	;
	
clazz			
	:	(UNIVERSAL_KW)
	|	(APPLICATION_KW)
	|	(PRIVATE_KW)
	;

class_NUMBER
	:	((NUMBER)
	|	(LOWER) )
		
	;

// Useful types
defined_type
	:	((UPPER 
			DOT )? 
		(UPPER)
		( constraint)? )
	;
// Referenced Type
selection_type
	:	((LOWER)
	 	LESS
	 	(type))
	;

//Constrained Type Causes Infinite Recursion
// Resolved by parsing the constraints along with the types using them.
//type_and_constraint returns [Object obj]
//{AsnTypeAndConstraint tpcns = new AsnTypeAndConstraint();
//obj=null;Object obj1;AsnConstraint cnstrnt;}
//	:	((obj1 = type { tpcns.type = obj1;})
//		(cnstrnt = constraint {tpcns.constraint = cnstrnt;}))
//		{obj=tpcns; tpcns=null;}
//	;

// Macros Types	
macros_type
		:	( operation_macro)
		|	( error_macro)
		|	( objecttype_macro)
		;

operation_macro
	: (	'OPERATION'
		(ARGUMENT_KW (LOWER)?
		(( type)
		))?
		(RESULT_KW (SEMI|(LOWER)?
		(type
		)|))?
		(ERRORS_KW L_BRACE (operation_errorlist|) R_BRACE )?
		(LINKED_KW L_BRACE (linkedOp_list)?	 R_BRACE )? )
	;

operation_errorlist
	: typeorvalue
		(COMMA ( typeorvalue))*
	;
	
linkedOp_list
	: typeorvalue
		(COMMA ( typeorvalue))*
	;
	
error_macro
	:  ( ERROR_KW  (PARAMETER_KW
		((LOWER)? 
		( type) ) )? )
	;
	
objecttype_macro 
	: ('OBJECT-TYPE' 'SYNTAX' ( type )
		('ACCESS'LOWER)
	  ('STATUS'LOWER) 
	  ('DESCRIPTION' CHARACTER_KW STRING_KW)?
	  ('REFERENCE' CHARACTER_KW STRING_KW)? 
	  ('INDEX' L_BRACE (typeorvaluelist) R_BRACE)? 
	  ('DEFVAL' L_BRACE ( value) R_BRACE )? )	
	;

typeorvaluelist
	: (( typeorvalue)
	   (COMMA (typeorvalue)* ))
	;

typeorvalue
	: (( type) | value)
	;

elementType_list
	:	( elementType
	    (COMMA ( elementType))*)
	;

elementType
	: (	(LOWER 
		( tag)? 
		( tag_default)? 
		( type) ( (OPTIONAL_KW) 
		| (DEFAULT_KW value ))? )
	|	COMPONENTS_KW OF_KW( type ))
	;
		
namedNumber_list	
	: (	L_BRACE ( namedNumber)
	   (COMMA ( namedNumber) )*  R_BRACE )
	;

namedNumber
	:	(LOWER L_PAREN 
		( signed_number
		| ( defined_value)) R_PAREN	)
	;
	
// Updated Grammar for ASN constraints

constraint
	: L_PAREN 
		(element_set_specs)? 
		(exception_spec)? 
	  R_PAREN
	;

exception_spec
	: (EXCLAMATION 
	  ((signed_number)
	   |(defined_value)
	   |type COLONvalue))
	;

element_set_specs
	:	(element_set_spec
		(COMMA DOTDOTDOT)? 
		(COMMAelement_set_spec)?)
	;

element_set_spec
	:intersections
		((BAR | UNION_KW )intersections)*
	| 	ALL EXCEPTconstraint_elements
	;

// Coding is not proper for EXCEPT constraint elements. 
// One EXCEPT constraint elements should be tied to one Constraint elements
//(an object of constraint and except list)
// and not in one single list
intersections

	:constraint_elements
	   (EXCEPTconstraint_elements)? 
	   ((INTERSECTION | INTERSECTION_KW)constraint_elements
	   (EXCEPTconstraint_elements)?)*
	;
				
constraint_elements
	:( value)
	|(value_range)
	|	(SIZE_KWconstraint)
	|	(FROM_KWconstraint)
	|	(L_PARENelement_set_spec R_PAREN)
	|	((INCLUDES)?type)
	|	(PATTERN_KWvalue)
	|	(WITH_KW 
		((COMPONENT_KWconstraint)
		|	
		(COMPONENTS_KW
		L_BRACE (DOTDOTDOT COMMA)? type_constraint_list R_BRACE )))
	;

value_range
	: (value | MIN_KW) (LESS)?  // lower end
	   DOTDOT
	  (LESS)? (value | MAX_KW) // upper end
	;
	
type_constraint_list
	:named_constraint
	 (COMMAnamed_constraint)*
	;

named_constraint
	:LOWER
	    (constraint)? 
	    (PRESENT_KW
	     |ABSENT_KW
	     | OPTIONAL_KW)?
	;
				
/*-----------VALUES ---------------------------------------*/

value		

	:(TRUE_KW)
	|(FALSE_KW)
	|(NULL_KW)
	|(C_STRING)
	|( defined_value)
	|( signed_number) 
	|(choice_value)
	|(sequence_value)
	|(sequenceof_value)
	|(cstr_value)
	|(obj_id_comp_lst)
	|(PLUS_INFINITY_KW)
	|(MINUS_INFINITY_KW)
	;

cstr_value
	:  ((H_STRING)
	|(B_STRING)
	|	(L_BRACE	((id_list)
					|(char_defs_list)
					| tuple_or_quad)    R_BRACE))
	;

id_list
	: (LOWER) 
	  (COMMALOWER)*
	;
	
char_defs_list
	: char_defs 
	(COMMA ( char_defs))* 
	;

tuple_or_quad
	: ( signed_number) 
	  COMMA 
	  ( signed_number) 
	  ((R_BRACE)  |  (COMMA 
	  ( signed_number) 
	  COMMA ( signed_number)))
	;

char_defs
	:	(C_STRING)
	|	(L_BRACE ( signed_number) COMMA ( signed_number) 
		((R_BRACE)
		|(COMMA ( signed_number) 
		COMMA ( signed_number) R_BRACE)))
	|	( defined_value)
	;

choice_value
	: ((LOWER)
	   (COLON)?  (value))
	;

sequence_value
	:	L_BRACE  ((named_value)?
		(COMMAnamed_value)*)   R_BRACE
	;

sequenceof_value
	: L_BRACE ((value)?
       (COMMAvalue)*) 
	  R_BRACE
	;
defined_value
	:	((UPPER 
			DOT)?LOWER)
	;
		
signed_number
	:	((MINUS)? 
		(NUMBER) )
	;
	
named_value	
	:	(LOWERvalue)
	; 
ABSENT_KW : 'ABSENT';
 
ABSTRACT_SYNTAX_KW : 'ABSTRACT-SYNTAX';
 
ALL_KW : 'ALL';
 
ANY_KW : 'ANY';
 
ARGUMENT_KW : 'ARGUMENT';
 
APPLICATION_KW : 'APPLICATION';
 
AUTOMATIC_KW : 'AUTOMATIC';
 
BASED_NUM_KW : 'BASEDNUM';
 
BEGIN_KW : 'BEGIN';
 
BIT_KW : 'BIT';
 
BMP_STRING_KW : 'BMPString';
 
BOOLEAN_KW : 'BOOLEAN';
 
BY_KW : 'BY';
 
CHARACTER_KW : 'CHARACTER';
 
CHOICE_KW : 'CHOICE';
 
CLASS_KW : 'CLASS';
 
COMPONENTS_KW : 'COMPONENTS';
 
COMPONENT_KW : 'COMPONENT';
 
CONSTRAINED_KW : 'CONSTRAINED';
 
DEFAULT_KW : 'DEFAULT';
 
DEFINED_KW : 'DEFINED';
 
DEFINITIONS_KW : 'DEFINITIONS';
 
EMBEDDED_KW : 'EMBEDDED';
 
END_KW : 'END';
 
ENUMERATED_KW : 'ENUMERATED';
 
ERROR_KW : 'ERROR';
 
ERRORS_KW : 'ERRORS';
 
EXCEPT_KW : 'EXCEPT';
 
EXPLICIT_KW : 'EXPLICIT';
 
EXPORTS_KW : 'EXPORTS';
 
EXTENSIBILITY_KW : 'EXTENSIBILITY';
 
EXTERNAL_KW : 'EXTERNAL';
 
FALSE_KW : 'FALSE';
 
FROM_KW : 'FROM';
 
GENERALIZED_TIME_KW : 'GeneralizedTime';
 
GENERAL_STR_KW : 'GeneralString';
 
GRAPHIC_STR_KW : 'GraphicString';
 
IA5_STRING_KW : 'IA5String';
 
IDENTIFIER_KW : 'IDENTIFIER';
 
IMPLICIT_KW : 'IMPLICIT';
 
IMPLIED_KW : 'IMPLIED';
 
IMPORTS_KW : 'IMPORTS';
 
INCLUDES_KW : 'INCLUDES';
 
INSTANCE_KW : 'INSTANCE';
 
INTEGER_KW : 'INTEGER';
 
INTERSECTION_KW : 'INTERSECTION';
 
ISO646STRING_KW : 'ISO646String';
 
LINKED_KW : 'LINKED';
 
MAX_KW : 'MAX';
 
MINUS_INFINITY_KW : 'MINUSINFINITY';
 
MIN_KW : 'MIN';
 
NULL_KW : 'NULL';
 
NUMERIC_STR_KW : 'NumericString';
 
OBJECT_DESCRIPTOR_KW : 'ObjectDescriptor';
 
OBJECT_KW : 'OBJECT';
 
OCTET_KW : 'OCTET';
 
OPERATION_KW : 'OPERATION';
 
OF_KW : 'OF';
 
OID_KW : 'OID';
 
OPTIONAL_KW : 'OPTIONAL';
 
PARAMETER_KW : 'PARAMETER';
 
PDV_KW : 'PDV';
 
PLUS_INFINITY_KW : 'PLUSINFINITY';
 
PRESENT_KW : 'PRESENT';
 
PRINTABLE_STR_KW : 'PrintableString';
 
PRIVATE_KW : 'PRIVATE';
 
REAL_KW : 'REAL';
 
RELATIVE_KW : 'RELATIVE';
 
RESULT_KW : 'RESULT';
 
SEQUENCE_KW : 'SEQUENCE';
 
SET_KW : 'SET';
 
SIZE_KW : 'SIZE';
 
STRING_KW : 'STRING';
 
TAGS_KW : 'TAGS';
 
TELETEX_STR_KW : 'TeletexString';
 
TRUE_KW : 'TRUE';
 
TYPE_IDENTIFIER_KW : 'TYPE-IDENTIFIER';
 
UNION_KW : 'UNION';
 
UNIQUE_KW : 'UNIQUE';
 
UNIVERSAL_KW : 'UNIVERSAL';
 
UNIVERSAL_STR_KW : 'UniversalString';
 
UTC_TIME_KW : 'UTCTime';
 
UTF8STRING_KW : 'UTF8String';
 
VIDEOTEX_STR_KW : 'VideotexString';
 
VISIBLE_STR_KW : 'VisibleString';
 
WITH_KW : 'WITH';


// Operators

ASSIGN_OP			:	'::='	;
BAR					:	'|'		;
COLON				:	':'		;
COMMA				:	','		;
COMMENT				:	'--'	;
DOT					:	'.'		;
DOTDOT				:	'..'	;
ELLIPSIS			:	'...'	;
EXCLAMATION			:	'!'		;
INTERSECTION		:	'^'		;
LESS				:	'<'		;
L_BRACE				:	'{'		;
L_BRACKET			:	'['		;
L_PAREN				:	'('		;
MINUS				:	'-'		;
PLUS				:	'+'		;
R_BRACE				:	'}'		;
R_BRACKET			:	']'		;
R_PAREN				:	')'		;
SEMI				:	';'		;
SINGLE_QUOTE		:	'\''		;
CHARB				:	'\'B'	;
CHARH				:	'\'H'	;

// Whitespace -- ignored

WS			
	:	(	' ' | '\t' | '\f'	|	(	'\r\n'// DOS
	|	'\r'// Macintosh
	|	'\n'// Unix 
	))+
	;

// Single-line comments
SL_COMMENT
	: ( COMMENT (  { LA(2)!='-' }? '-' 	|	~('-'|'\n'|'\r'))*	( (('\r')? '\n')| COMMENT) )
	;

NUMBER	:	('0'..'9')+ ;

UPPER
	:   ('A'..'Z') 
		(	( 'a'..'z' | 'A'..'Z' |'-' | '0'..'9' ))* 	;

LOWER
	:	('a'..'z') 
		(	( 'a'..'z' | 'A'..'Z' |'-' | '0'..'9' ))* 	;
BDIG		: ('0'|'1') ;
HDIG		:	(('0'..'9') )
			|	('A'..'F')
			|	('a'..'f')
			;

// Unable to resolve a string like 010101 followed by 'H
//B_STRING 	: 	SINGLE_QUOTE ({LA(3)!='B'}? BDIG)+  BDIG SINGLE_QUOTE 'B' 	;
//H_STRING 	: 	SINGLE_QUOTE ({LA(3)!='H'}? HDIG)+  HDIG SINGLE_QUOTE 'H'  ;

B_OR_H_STRING
	:	(B_STRING
		| H_STRING)
	;
B_STRING 	: 	SINGLE_QUOTE (BDIG)+ SINGLE_QUOTE 'B' 	;
H_STRING 	: 	SINGLE_QUOTE (HDIG)+ SINGLE_QUOTE 'H'  ;

			
C_STRING 	: 	'"'	(UPPER | LOWER)*  '"' ;	

