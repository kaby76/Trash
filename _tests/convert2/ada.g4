


//-----------------------------------------------------------------------------
// Define a Parser, calling it AdaParser
//-----------------------------------------------------------------------------
grammar Ada;

// Compilation Unit:  This is the start rule for this parser.
// The rules in this grammar are listed in the order in which
// compilation_unit introduces them, depth first, with the
// exception of the expression related rules which are listed
// towards the end.
compilation_unit
	: context_items_opt ( library_item | subunit ) ( pragma )*
	;

// The pragma related rules are pulled up here to get them out of the way.

pragma  : PRAGMA IDENTIFIER pragma_args_opt SEMI
	;

pragma_args_opt : ( LPAREN pragma_arg ( COMMA pragma_arg )* RPAREN )?
	;

pragma_arg : ( IDENTIFIER RIGHT_SHAFT )? expression
	;

context_items_opt : ( pragma )* ( with_clause ( use_clause | pragma )* )*
		// RM Annex P neglects pragmas; we include them.
		// The node should really be named CONTEXT_ITEMS_OPT, but we
		// stick with the RM wording.
	;

with_clause :WITH c_name_list SEMI
	;

c_name_list : compound_name ( COMMA compound_name )*
	;

compound_name : IDENTIFIER ( DOT IDENTIFIER )*
	// Strangely, the RM never defines this rule, which however is
	// required for tightening up the syntax of certain names
	// (library unit names etc.)
	;

use_clause :USE
		( TYPE subtype_mark ( COMMA subtype_mark )*
		| c_name_list
		)
	SEMI
	;

subtype_mark : compound_name ( TIC attribute_id )?
	// { #subtype_mark = #(#[SUBTYPE_MARK, "SUBTYPE_MARK"], #subtype_mark); }
	;

attribute_id : RANGE
	| DIGITS
	| DELTA
	| ACCESS
	| IDENTIFIER
	;

library_item : private_opt
		/* Slightly loose; PRIVATE can only precede
		  {generic|package|subprog}_decl.
		  Semantic check required to ensure it.*/
	( lib_pkg_spec_or_body
	| subprog_decl_or_rename_or_inst_or_body
	| generic_decl
	)
	;

private_opt : ( PRIVATE )?
	;

lib_pkg_spec_or_body
	:PACKAGE
		( BODY def_id IS pkg_body_part end_id_opt SEMI
		| def_id spec_decl_part
		)
	;

subprog_decl
	:PROCEDURE def_id
		( generic_subp_inst
		| formal_part_opt
			( renames
			| is_separate_or_abstract_or_decl
			)
			SEMI
		)
	|FUNCTION def_designator
		( generic_subp_inst
		| function_tail
			( renames
			| is_separate_or_abstract_or_decl
			)
			SEMI
		)
	;

def_id
	: { lib_level }?compound_name
	| { !lib_level }?IDENTIFIER
	;

generic_subp_inst : IS generic_inst SEMI
	;

generic_inst : NEW compound_name ( LPAREN value_s RPAREN )?
	;

parenth_values : LPAREN value ( COMMA value )* RPAREN
	;

value : ( OTHERS RIGHT_SHAFT expression
	| ranged_expr_s ( RIGHT_SHAFT expression )?
	)
	// { #value = #(#[VALUE, "VALUE"], #value); }
	;

ranged_expr_s : ranged_expr ( PIPE ranged_expr )*
	// { #ranged_expr_s =
	// 	#(#[RANGED_EXPRS, "RANGED_EXPRS"], #ranged_expr_s); }
	;

ranged_expr : expression
		( DOT_DOT simple_expression
		| RANGE range
		)?
	;

range_constraint : RANGE range
	;

range : ( range_dots
	| range_attrib_ref
	)
	// Current assumption is we don't need an extra node for range,
	// otherwise uncomment the following line:
	// { #range = #(#[RANGE_EXPR, "RANGE_EXPR"], #range); }
	;

range_dots : simple_expression DOT_DOT simple_expression
	;

range_attrib_ref : // "name TIC RANGE" is ambiguous; instead:
	prefix TICRANGE ( LPAREN expression RPAREN )?
	;

// Here, the definition of `prefix' deviates from the RM.
// This gives us some more strictness than `name' (which the RM uses to
// define `prefix'.)
prefix : IDENTIFIER
		( DOT ( ALL | IDENTIFIER )
		|LPAREN value_s RPAREN
		)*
	;

formal_part_opt : ( LPAREN parameter_specification
		( SEMI parameter_specification )*
		RPAREN )?
	;

parameter_specification : def_ids_colon mode_opt subtype_mark init_opt
	;

def_ids_colon : defining_identifier_list COLON
	;

defining_identifier_list : IDENTIFIER ( COMMA IDENTIFIER )*
	;

mode_opt : ( IN ( OUT )? | OUT | ACCESS )?
	;

renames
	: RENAMES ( name
		|definable_operator_symbol
		)
	;

name
	: IDENTIFIER
		( DOT	( ALL
			| IDENTIFIER
			| CHARACTER_LITERAL
			|is_operator
			)
		|LPAREN value_s RPAREN
		| TIC attribute_id   // must be in here because of e.g.
				     // Character'Pos (x)
		)*
	// { #name = #(#[NAME, "NAME"], #name); }
	;

is_operator
	: { is_operator_symbol(LT(1)->getText().c_str()) }?CHAR_STRING
	;

definable_operator_symbol
	: { definable_operator(LT(1)->getText().c_str()) }?CHAR_STRING
	;

parenthesized_primary :LPAREN
		( NuLL RECORD
		| value_s extension_opt
		)
	RPAREN
	;

extension_opt :  ( WITH ( NuLL RECORD | value_s ) )?
	;

is_separate_or_abstract_or_decl
	: IS separate_or_abstract
	|
	;

separate_or_abstract
	: SEPARATE
	| ABSTRACT
	;

def_designator
	: { lib_level }?compound_name
	| { !lib_level }?designator
	;

designator
	:definable_operator_symbol
	|IDENTIFIER
	;

function_tail : func_formal_part_opt RETURN subtype_mark
	;

// formal_part_opt is not strict enough for functions, i.e. it permits
// "in out" and "out" as modes, thus we make an extra rule:
func_formal_part_opt : ( LPAREN func_param ( SEMI func_param )* RPAREN )?
	;

func_param : def_ids_colon in_access_opt subtype_mark init_opt
	;

in_access_opt : ( IN | ACCESS )?
	;

spec_decl_part
	: ( IS ( generic_inst
		| pkg_spec_part
		)
	| renames
	)
	SEMI
	;

pkg_spec_part : basic_declarative_items_opt
		( PRIVATE basic_declarative_items_opt )?
		end_id_opt
	;

basic_declarative_items_opt : ( basic_decl_item | pragma )*
	;

basic_declarative_items : ( basic_decl_item | pragma )+
	;

basic_decl_item
	:PACKAGE def_id spec_decl_part
	|TASK task_type_or_single_decl
	|PROTECTED prot_type_or_single_decl SEMI
	| subprog_decl
	| decl_common
	;

task_type_or_single_decl
	: TYPE def_id discrim_part_opt task_definition_opt
	| def_id task_definition_opt
	;

task_definition_opt
	: IS task_items_opt private_task_items_opt end_id_opt SEMI
	| SEMI
	;

discrim_part_opt
	: ( discrim_part_text )?
	;

discrim_part_text : LPAREN (BOX | discriminant_specifications) RPAREN
	;

known_discrim_part
	: LPAREN discriminant_specifications RPAREN
	;

empty_discrim_opt :
	;

discrim_part
	: discrim_part_text
	;

discriminant_specifications : discriminant_specification
		( SEMI discriminant_specification )*
	;

discriminant_specification : def_ids_colon access_opt subtype_mark init_opt
	;

access_opt : ( ACCESS )?
	;

init_opt : ( ASSIGN expression )?
	;  // `expression' is of course much too loose;
	   // semantic checks are required in the usage contexts.

task_items_opt : ( pragma )* entrydecls_repspecs_opt
	;

entrydecls_repspecs_opt : ( entry_declaration ( pragma | rep_spec )* )*
	;

entry_declaration :ENTRY IDENTIFIER
		discrete_subtype_def_opt formal_part_opt SEMI
	;

discrete_subtype_def_opt : (
		LPAREN discrete_subtype_definition RPAREN
	| /* empty */
	)
	;

discrete_subtype_definition : ( range
	| subtype_ind
	)
	// Looks alot like discrete_range, but it's not
	// (as soon as we start doing semantics.)
	/* TBC: No need for extra node, just use the inner nodes?
	 { #discrete_subtype_definition =
		#(#[DISCRETE_SUBTYPE_DEFINITION,
		   "DISCRETE_SUBTYPE_DEFINITION"],
		   #discrete_subtype_definition); }
	 */
	;

rep_spec :FOR subtype_mark USE rep_spec_part SEMI
	;

rep_spec_part
	: RECORD align_opt comp_loc_s END RECORD
	| AT expression
	| expression
	;

align_opt : ( AT MOD expression SEMI )?
	;

comp_loc_s : ( pragma | subtype_mark AT expression RANGE range SEMI )*
	;

private_task_items_opt : ( PRIVATE ( pragma )* entrydecls_repspecs_opt )?
	// Maybe we could just reuse TASK_ITEMS_OPT here instead of
	// making a separate node type.
	;

prot_type_or_single_decl
	: TYPE def_id discrim_part_opt protected_definition
	| def_id protected_definition
	;

protected_definition
	: IS prot_op_decl_s ( PRIVATE prot_member_decl_s )? end_id_opt
	;

prot_op_decl_s : ( prot_op_decl )*
	;

prot_op_decl : entry_declaration
	|PROCEDURE def_id formal_part_opt SEMI
	|FUNCTION def_designator function_tail SEMI
	| rep_spec
	| pragma
	;

prot_member_decl_s : ( prot_op_decl | comp_decl )*
	;

comp_decl : def_ids_colon component_subtype_def init_opt SEMI
	;

// decl_common is shared between declarative_item and basic_decl_item.
// decl_common only contains specifications.
decl_common
	:TYPE IDENTIFIER
		( IS type_def
		|	( discrim_part
				( IS derived_or_private_or_record
				|
				)
			| empty_discrim_opt
			  // NB: In this case, the discrim_part_opt does not
			  //   appear in the INCOMPLETE_TYPE_DECLARATION node.
			)
		  /* The artificial derived_or_private_or_record rule
		     gives us some syntax-level control over where a
		     discrim_part may appear.
		     However, a semantic check is still necessary to make
		     sure the discrim_part is not given for a derived type
		     of an elementary type, or for the full view of a
		     private type that turns out to be such.  */
		)
		SEMI
	|SUBTYPE IDENTIFIER IS subtype_ind SEMI
	| generic_decl
	| use_clause
	|FOR ( local_enum_name USE
			enumeration_aggregate
		| subtype_mark USE rep_spec_part
		)
		SEMI
	|
		IDENTIFIERCOLON EXCEPTION RENAMES compound_name SEMI
	|
		IDENTIFIERCOLON subtype_mark RENAMES name SEMI
	| defining_identifier_listCOLON  // object_declaration
		( EXCEPTION
		| CONSTANT ASSIGN expression
		| aliased_constant_opt
			( array_type_definition init_opt
				// Not an RM rule, but simplifies distinction
				// from the non-array object_declaration.
			| subtype_ind init_opt
			)
		)
		SEMI
	;

type_def
	: LPAREN enum_id_s RPAREN
	| RANGE range
	| MOD expression
	| DIGITS expression range_constraint_opt
	| DELTA expression
		( RANGE range
		| DIGITS expression range_constraint_opt
		)
	| array_type_definition
	| access_type_definition
	| empty_discrim_opt derived_or_private_or_record
	;

enum_id_s : enumeration_literal_specification
		( COMMA enumeration_literal_specification )*
	;

enumeration_literal_specification : IDENTIFIER | CHARACTER_LITERAL
	;

range_constraint_opt : ( range_constraint )?
	;

array_type_definition
	: ARRAY LPAREN index_or_discrete_range_s RPAREN
		OF component_subtype_def
	;

index_or_discrete_range_s
	: index_or_discrete_range ( COMMA index_or_discrete_range )*
	;

index_or_discrete_range
	: simple_expression
		( DOT_DOT simple_expression  // constrained
		| RANGE ( BOX                // unconstrained
			| range              // constrained
			)
		)?
	;

component_subtype_def : aliased_opt subtype_ind
	;

aliased_opt : ( ALIASED )?
	;

subtype_ind : subtype_mark constraint_opt
	;

constraint_opt : ( range_constraint
	| digits_constraint
	| delta_constraint
	| index_constraint
	| discriminant_constraint
	)?
	;

digits_constraint :DIGITS expression range_constraint_opt
	;

delta_constraint :DELTA expression range_constraint_opt
	;

index_constraint :LPAREN discrete_range ( COMMA discrete_range )* RPAREN
	;

discrete_range
	: range
	| subtype_ind
	;

discriminant_constraint :LPAREN discriminant_association 
		( COMMA discriminant_association )* RPAREN
	;

discriminant_association : selector_names_opt expression
	;

selector_names_opt : ( association_head
	| /* empty */
	)
	;

association_head : selector_name ( PIPE selector_name )* RIGHT_SHAFT
	;

selector_name : IDENTIFIER  // TBD: sem pred
	;

access_type_definition
	: ACCESS
		( protected_opt
			( PROCEDURE formal_part_opt
			| FUNCTION func_formal_part_opt RETURN subtype_mark
			)
		| constant_all_opt subtype_ind
		)
	;

protected_opt : ( PROTECTED )?
	;

constant_all_opt : ( CONSTANT | ALL )?
	;

derived_or_private_or_record
	:
		abstract_opt NEW subtype_ind WITH
			( PRIVATE
			| record_definition
			)
	| NEW subtype_ind
	| abstract_tagged_limited_opt
		( PRIVATE
		| record_definition
		)
	;

abstract_opt : ( ABSTRACT )?
	;

record_definition
	: RECORD component_list END RECORD
	| NuLL RECORD  // Thus the component_list is optional in the tree.
	;

component_list
	: NuLL SEMI  // Thus the component_list is optional in the tree.
	| component_items ( variant_part { has_discrim }? )?
	| empty_component_items variant_part { has_discrim }?
	;

component_items : ( pragma | comp_decl )+
	;

empty_component_items :
	;

variant_part :CASE discriminant_direct_name IS variant_s END CASE SEMI
	;

discriminant_direct_name : IDENTIFIER  // TBD: symtab lookup.
	;

variant_s : ( variant )+
	;

variant :WHEN choice_s RIGHT_SHAFT component_list
	;

choice_s : choice ( PIPE choice )*
	;

choice : OTHERS
	| discrete_with_range
	| expression   //  ( DOT_DOT^ simple_expression )?
	;              // No, that's already in discrete_with_range

discrete_with_range : mark_with_constraint
	| range
	;

mark_with_constraint : subtype_mark range_constraint
	;

abstract_tagged_limited_opt
	: ( ABSTRACT TAGGED | TAGGED )?
	  ( LIMITED )?
	;

local_enum_name : IDENTIFIER  // to be refined: do a symbol table lookup
	;

enumeration_aggregate : parenth_values
	;

aliased_constant_opt : ( ALIASED )? ( CONSTANT )?
	;

generic_decl
	:GENERIC generic_formal_part_opt
	( PACKAGE def_id
		( renames
		| IS pkg_spec_part
		)
	| PROCEDURE def_id formal_part_opt
		( renames
		  // ^^^ Semantic check must ensure that the (generic_formal)*
		  //     after GENERIC is not given here.
		|
		)
	| FUNCTION def_designator function_tail
		( renames
		  // ^^^ Semantic check must ensure that the (generic_formal)*
		  //     after GENERIC is not given here.
		|
		)
	)
	SEMI
	;

generic_formal_part_opt : ( use_clause | pragma | generic_formal_parameter )*
	;

generic_formal_parameter :
	(TYPE def_id
		( IS
			( LPAREN BOX RPAREN
			| RANGE BOX
			| MOD BOX
			| DELTA BOX
				( DIGITS BOX
				|
				)
			| DIGITS BOX
			| array_type_definition
			| access_type_definition
			| empty_discrim_opt discriminable_type_definition
			)
		| discrim_part IS discriminable_type_definition
		)
	|WITH ( PROCEDURE def_id formal_part_opt subprogram_default_opt
		| FUNCTION def_designator function_tail subprogram_default_opt
		| PACKAGE def_id IS NEW compound_name formal_package_actual_part_opt
		)
	| parameter_specification
	)
	SEMI
	;

discriminable_type_definition
	:
		abstract_opt NEW subtype_ind WITH PRIVATE
	| NEW subtype_ind
	| abstract_tagged_limited_opt PRIVATE
	;

subprogram_default_opt : ( IS ( BOX | name ) )?
	;

formal_package_actual_part_opt
	: ( LPAREN ( BOX | defining_identifier_list ) RPAREN )?
	;

subprog_decl_or_rename_or_inst_or_body
	:PROCEDURE def_id
		( generic_subp_inst
		| formal_part_opt
			( renames
			| IS	( separate_or_abstract
				| body_part
				)
			|
			)
			SEMI
		)
	|FUNCTION def_designator
		( generic_subp_inst
		| function_tail
			( renames
			| IS	( separate_or_abstract
				| body_part
				)
			|
			)
			SEMI
		)
	;

body_part : declarative_part block_body end_id_opt
	;

declarative_part : ( pragma | declarative_item )*
	;

// A declarative_item may appear in the declarative part of any body.
declarative_item :
	(PACKAGE ( body_is
			( separate
			| pkg_body_part end_id_opt
			)
			SEMI
		| def_id spec_decl_part
		)
	|TASK ( body_is
			( separate
			| body_part
			)
			SEMI
		| task_type_or_single_decl
		)
	|PROTECTED
		( body_is
			( separate
	       		| prot_op_bodies_opt end_id_opt
			)
		| prot_type_or_single_decl
		)
		SEMI
	| subprog_decl_or_rename_or_inst_or_body
	| decl_common
	)
	/* DECLARATIVE_ITEM is just a pass-thru node so we omit it.
	   Objections anybody?
	 { #declarative_item =
		#(#[DECLARATIVE_ITEM,
		   "DECLARATIVE_ITEM"], #declarative_item); }
	 */
	;

body_is : BODY def_id IS
	;

separate : SEPARATE
	;

pkg_body_part : declarative_part block_body_opt
	;

block_body_opt : ( BEGIN handled_stmt_s )?
	;

prot_op_bodies_opt : ( entry_body
	| subprog_decl_or_body
	| pragma
	)*
	;

subprog_decl_or_body
	:PROCEDURE def_id formal_part_opt
		( IS body_part
		|
		)
		SEMI
	|FUNCTION def_designator function_tail
		( IS body_part
		|
		)
		SEMI
	;

block_body :BEGIN handled_stmt_s
	;

handled_stmt_s : statements except_handler_part_opt
	;

statements : ( pragma | statement )+
	;

statement : def_label_opt
	( null_stmt
	| exit_stmt
	| return_stmt
	| goto_stmt
	| delay_stmt
	| abort_stmt
	| raise_stmt
	| requeue_stmt
	| accept_stmt
	| select_stmt
	| if_stmt
	| case_stmt
	| loop_stmt SEMI
	| block END SEMI
	| statement_identifier
		( loop_stmt id_opt SEMI   // FIXME: The statement_identifier
		| block end_id_opt SEMI   // is not promoted into the tree.
		)
	| call_or_assignment
	// | code_stmt  // TBD: resolve ambiguity
	)
	;

def_label_opt : ( LT_LT IDENTIFIER GT_GT )?
	;

null_stmt :NuLL SEMI
	;

if_stmt :IF cond_clause elsifs_opt
	  else_opt
	  END IF SEMI
	;

cond_clause : conditionTHEN statements
	;

condition : expression
	// { #condition = #(#[CONDITION, "CONDITION"], #condition); }
	;

elsifs_opt : ( ELSIF cond_clause )*
	;

else_opt : ( ELSE statements )?
	;

case_stmt :CASE expression IS alternative_s END CASE SEMI
	;

alternative_s : ( case_statement_alternative )+
	;

case_statement_alternative :WHEN choice_s RIGHT_SHAFT statements
	;

loop_stmt : iteration_scheme_opt
		LOOP statements END LOOP
        ;

iteration_scheme_opt : ( WHILE condition
	| FOR IDENTIFIER IN reverse_opt discrete_subtype_definition
	)?
	;

reverse_opt : ( REVERSE )?
	;

id_opt :definable_operator_symbol { end_id_matches_def_id (endid) }?
	|compound_name { end_id_matches_def_id (#n) }?
	  /* Ordinarily we would need to be stricter here, i.e.
	     match compound_name only for the library-level case
	     (and IDENTIFIER otherwise), but end_id_matches_def_id
	     does the right thing for us.  */
	|
	;

end_id_opt : END id_opt
	;

/* Note: This rule should really be `statement_identifier_opt'.
   However, manual disambiguation of `loop_stmt' from `block'
   in the presence of the statement_identifier in `statement'
   results in this rule. The case of loop_stmt/block given
   without the statement_identifier is directly coded in
   `statement'.  */
statement_identifier :IDENTIFIER COLON
	;

/*
statement_identifier_opt : ( n:IDENTIFIER COLON!  { push_def_id(#n); } )?
	{ #statement_identifier_opt =
	  	#(#[STATEMENT_IDENTIFIER_OPT,
		   "STATEMENT_IDENTIFIER_OPT"], #statement_identifier_opt); }
	;
 */

block : declare_opt block_body
	;

declare_opt : ( DECLARE declarative_part )?
	;

exit_stmt :EXIT ( label_name )? ( WHEN condition )? SEMI
	;

label_name : IDENTIFIER
	;

return_stmt :RETURN ( expression )? SEMI
	;

goto_stmt :GOTO label_name SEMI
	;

call_or_assignment :  // procedure_call is in here.
	name ( ASSIGN expression
	     |
		/* Preliminary. Use semantic analysis to produce
		   {PROCEDURE|ENTRY}_CALL_STATEMENT.  */
	     )
	SEMI
	;

entry_body :ENTRY def_id entry_body_formal_part entry_barrier IS
		body_part SEMI
	;

entry_body_formal_part : entry_index_spec_opt formal_part_opt
	;

entry_index_spec_opt :
	(
		LPAREN FOR def_id IN discrete_subtype_definition RPAREN
	| /* empty */
	)
	;

entry_barrier : WHEN condition
	;

entry_call_stmt : name SEMI
	;

accept_stmt :ACCEPT def_id entry_index_opt formal_part_opt
		( DO handled_stmt_s end_id_opt SEMI
		| SEMI
		)
	;

entry_index_opt : ( LPAREN expression RPAREN
	// Looks alot like parenthesized_expr_opt, but it's not.
	// We need the syn pred for the usage context in accept_stmt.
	// The formal_part_opt that follows the entry_index_opt there
	// creates ambiguity (due to the opening LPAREN.)
	| /* empty */
	)
	;

delay_stmt :DELAY until_opt expression SEMI
	;

until_opt : ( UNTIL )?
	;

// SELECT_STATEMENT itself is not modeled since it is trivially
// reconstructed:
//   select_statement ::= selective_accept | timed_entry_call
//             | conditional_entry_call | asynchronous_select
//
select_stmt :SELECT
	(
		triggering_alternative THEN ABORT abortable_part
	| selective_accept
	| entry_call_alternative
		( OR delay_alternative
		| ELSE statements
		)
	)
	END SELECT SEMI
	// { Set (#s, SELECT_STATEMENT); }
	;

triggering_alternative : ( delay_stmt | entry_call_stmt ) stmts_opt
	;

abortable_part : stmts_opt
	;

entry_call_alternative : entry_call_stmt stmts_opt
	;

selective_accept : guard_opt select_alternative or_select_opt else_opt
	;

guard_opt : ( WHEN condition RIGHT_SHAFT ( pragma )* )?
	;

select_alternative  // Not modeled since it's just a pass-through.
	: accept_alternative
	| delay_alternative
	|TERMINATE SEMI
	;

accept_alternative : accept_stmt stmts_opt
	;

delay_alternative : delay_stmt stmts_opt
	;

stmts_opt : ( pragma | statement )*
	;

or_select_opt : ( OR guard_opt select_alternative )*
	;

abort_stmt :ABORT name ( COMMA name )* SEMI
	;

except_handler_part_opt : ( EXCEPTION ( exception_handler )+ )?
	;

exception_handler :WHEN identifier_colon_opt except_choice_s RIGHT_SHAFT
		statements
	;

identifier_colon_opt : ( IDENTIFIER COLON )?
	;

except_choice_s : exception_choice ( PIPE exception_choice )*
	;

exception_choice : compound_name
	| OTHERS
	;

raise_stmt :RAISE ( compound_name )? SEMI
	;

requeue_stmt :REQUEUE name ( WITH ABORT )? SEMI
	;

operator_call :CHAR_STRING operator_call_tail
	;

operator_call_tail
	: LPAREN { is_operator_symbol(opstr->getText().c_str()) }?
		  value_s RPAREN
	;

value_s : value ( COMMA value )*
	;

/*
literal : NUMERIC_LIT
	| CHARACTER_LITERAL
	| CHAR_STRING
	| NuLL
	;
 */

expression : relation
		(AND ( THEN )? relation
		|OR ( ELSE )? relation
		| XOR relation
		)*
	;

relation : simple_expression
		( IN range_or_mark
		|NOT IN range_or_mark
		| EQ simple_expression
		| NE simple_expression
		| LT_ simple_expression
		| LE simple_expression
		| GT simple_expression
		| GE simple_expression
		)?
	;

range_or_mark : range
	| subtype_mark
	;

simple_expression : signed_term
		( PLUS signed_term
		| MINUS signed_term
		| CONCAT signed_term
		)*
	;

signed_term
	:PLUS term
	|MINUS term
	| term
	;

term    : factor ( STAR factor
		| DIV factor
		| MOD factor
		| REM factor
		)*
	;

factor : ( NOT primary
	| ABS primary
	| primary ( EXPON primary )?
	)
	;

primary : ( name_or_qualified
	| parenthesized_primary
	| allocator
	| NuLL
	| NUMERIC_LIT
	| CHARACTER_LITERAL
	|CHAR_STRING ( operator_call_tail )?
	)
	;

// Temporary, to be turned into just `qualified'.
// We get away with it because `qualified' is always mentioned
// together with `name'.
// Only exception: `code_stmt', which is not yet implemented.
name_or_qualified
	: IDENTIFIER
		( DOT	( ALL
			| IDENTIFIER
			| CHARACTER_LITERAL
			|is_operator
			)
		|LPAREN value_s RPAREN
		| TIC ( parenthesized_primary | attribute_id )
		)*
	;

allocator :NEW name_or_qualified
	;

subunit :SEPARATE LPAREN compound_name RPAREN
		( subprogram_body
		| package_body
		| task_body
		| protected_body
		)
	;

subprogram_body
	:PROCEDURE def_id formal_part_opt IS body_part SEMI
	|FUNCTION function_tail IS body_part SEMI
	;

package_body :PACKAGE body_is pkg_body_part end_id_opt SEMI
	;

task_body :TASK body_is body_part SEMI
	;
 
protected_body :PROTECTED body_is prot_op_bodies_opt end_id_opt SEMI
	; 
ABORT : 'abort';
 
ABS : 'abs';
 
ABSTRACT : 'abstract';
 
ACCEPT : 'accept';
 
ACCESS : 'access';
 
ALIASED : 'aliased';
 
ALL : 'all';
 
AND : 'and';
 
ARRAY : 'array';
 
AT : 'at';
 
BEGIN : 'begin';
 
BODY : 'body';
 
CASE : 'case';
 
CONSTANT : 'constant';
 
DECLARE : 'declare';
 
DELAY : 'delay';
 
DELTA : 'delta';
 
DIGITS : 'digits';
 
DO : 'do';
 
ELSE : 'else';
 
ELSIF : 'elsif';
 
END : 'end';
 
ENTRY : 'entry';
 
EXCEPTION : 'exception';
 
EXIT : 'exit';
 
FOR : 'for';
 
FUNCTION : 'function';
 
GENERIC : 'generic';
 
GOTO : 'goto';
 
IF : 'if';
 
IN : 'in';
 
IS : 'is';
 
LIMITED : 'limited';
 
LOOP : 'loop';
 
MOD : 'mod';
 
NEW : 'new';
 
NOT : 'not';
 
NuLL : 'null';
 
OF : 'of';
 
OR : 'or';
 
OTHERS : 'others';
 
OUT : 'out';
 
PACKAGE : 'package';
 
PRAGMA : 'pragma';
 
PRIVATE : 'private';
 
PROCEDURE : 'procedure';
 
PROTECTED : 'protected';
 
RAISE : 'raise';
 
RANGE : 'range';
 
RECORD : 'record';
 
REM : 'rem';
 
RENAMES : 'renames';
 
REQUEUE : 'requeue';
 
RETURN : 'return';
 
REVERSE : 'reverse';
 
SELECT : 'select';
 
SEPARATE : 'separate';
 
SUBTYPE : 'subtype';
 
TAGGED : 'tagged';
 
TASK : 'task';
 
TERMINATE : 'terminate';
 
THEN : 'then';
 
TYPE : 'type';
 
UNTIL : 'until';
 
USE : 'use';
 
WHEN : 'when';
 
WHILE : 'while';
 
WITH : 'with';
 
XOR : 'xor';


//----------------------------------------------------------------------------
// OPERATORS
//----------------------------------------------------------------------------
COMMENT_INTRO      :       '--'    ;
DOT_DOT            :       '..'    ;
LT_LT              :       '<<'    ;
BOX                :       '<>'    ;
GT_GT              :       '>>'    ;
ASSIGN             :       ':='    ;
RIGHT_SHAFT        :       '=>'    ;
NE                 :       '/='    ;
LE                 :       '<='    ;
GE                 :       '>='    ;
EXPON              :       '**'    ;
PIPE               :       '|'     ;
CONCAT             :       '&'     ;
DOT                :       '.'     ;
EQ                 :       '='     ;
LT_                :       '<'     ;
GT                 :       '>'     ;
PLUS               :       '+'     ;
MINUS              :       '-'     ;
STAR               :       '*'     ;
DIV                :       '/'     ;
LPAREN             :       '('     ;
RPAREN             :       ')'     ;
COLON              :       ':'     ;
COMMA              :       ','     ;
SEMI               :       ';'     ;

TIC    : { LA(3)!='\'' }?  '\''    ;
	 // condition needed to disambiguate from CHARACTER_LITERAL


// Literals.

// Rule for IDENTIFIER: testLiterals is set to true.  This means that
// after we match the rule, we look in the literals table to see if
// it's a keyword or really an identifier.
IDENTIFIER
            : ( 'a'..'z' ) ( ('_')? ( 'a'..'z'|'0'..'9' ) )*
	;

CHARACTER_LITERAL    : { LA(3)=='\'' }?
	// condition needed to disambiguate from TIC
	'\'' . '\''
	;

CHAR_STRING : '"' ('\"\"' | ~('"'))* '"'
	;

NUMERIC_LIT : ( DIGIT )+
		( '#' BASED_INTEGER ( '.' BASED_INTEGER )? '#'
		| ( '_' ( DIGIT )+ )+  // INTEGER
		)?
		( { LA(2)!='.' }?  //&& LA(3)!='.' }?
			( '.' ( DIGIT )+ ( '_' ( DIGIT )+ )* ( EXPONENT )?
			| EXPONENT
			)
		)?
	;
DIGIT   :  ( '0'..'9' ) ;
EXPONENT           :  ('e') ('+'|'-')? ( DIGIT )+ ;
EXTENDED_DIGIT     :  ( DIGIT | 'a'..'f' ) ;
BASED_INTEGER      :  ( EXTENDED_DIGIT ) ( ('_')? EXTENDED_DIGIT )* ;


// Whitespace -- ignored
WS_	:	(	' '
		|	'\t'
		|	'\f'
		// handle newlines
		|	(	'\r\n'  // Evil DOS
			|	'\r'    // Macintosh
			|	'\n'    // Unix (the right way)
			)
		)
	;

// Single-line comments
COMMENT :	( COMMENT_INTRO (~('\n'|'\r'))* ('\n'|'\r'('\n')?) )
	;
