grammar foo;

translation_unit : top_level_item_* ;

top_level_item_ : function_definition | linkage_specification | declaration | statement_ | attributed_statement | type_definition | empty_declaration_ | preproc_if | preproc_ifdef | preproc_include | preproc_def | preproc_function_def | preproc_call | namespace_definition | concept_definition | namespace_alias_definition | using_declaration | alias_declaration | static_assert_declaration | template_declaration | template_instantiation | constructor_or_destructor_definition | operator_cast_definition | operator_cast_declaration ;

preproc_include : '#[ \\t]*include' path = ( string_literal | system_lib_string | identifier | preproc_call_expression ) '\\n' ;

preproc_def : '#[ \\t]*define' name = identifier value = ( preproc_arg | empty ) '\\n' ;

preproc_function_def : '#[ \\t]*define' name = identifier parameters = preproc_params value = ( preproc_arg | empty ) '\\n' ;

preproc_params : '(' ( ( identifier | '...' ) ',' ( identifier | '...' )* | empty ) ')' ;

preproc_call : directive = preproc_directive argument = ( preproc_arg | empty ) '\\n' ;

preproc_if : '#[ \\t]*if' condition = preproc_expression_ '\\n' top_level_item_* alternative = ( ( preproc_else | preproc_elif ) | empty ) '#[ \\t]*endif' ;

preproc_ifdef : ( '#[ \\t]*ifdef' | '#[ \\t]*ifndef' ) name = identifier top_level_item_* alternative = ( ( preproc_else | preproc_elif ) | empty ) '#[ \\t]*endif' ;

preproc_else : '#[ \\t]*else' top_level_item_* ;

preproc_elif : '#[ \\t]*elif' condition = preproc_expression_ '\\n' top_level_item_* alternative = ( ( preproc_else | preproc_elif ) | empty ) ;

preproc_if_in_field_declaration_list : '#[ \\t]*if' condition = preproc_expression_ '\\n' field_declaration_list_item_* alternative = ( ( preproc_else_in_field_declaration_list | preproc_elif_in_field_declaration_list ) | empty ) '#[ \\t]*endif' ;

preproc_ifdef_in_field_declaration_list : ( '#[ \\t]*ifdef' | '#[ \\t]*ifndef' ) name = identifier field_declaration_list_item_* alternative = ( ( preproc_else_in_field_declaration_list | preproc_elif_in_field_declaration_list ) | empty ) '#[ \\t]*endif' ;

preproc_else_in_field_declaration_list : '#[ \\t]*else' field_declaration_list_item_* ;

preproc_elif_in_field_declaration_list : '#[ \\t]*elif' condition = preproc_expression_ '\\n' field_declaration_list_item_* alternative = ( ( preproc_else_in_field_declaration_list | preproc_elif_in_field_declaration_list ) | empty ) ;

preproc_directive : '#[ \\\\t]*[a-zA-Z]\\\\w*' ;

preproc_arg : '.|\\\\\\\\\\\\r?\\\\n'+ ;

preproc_expression_ : identifier | preproc_call_expression | number_literal | char_literal | preproc_defined | preproc_unary_expression | preproc_binary_expression | preproc_parenthesized_expression ;

preproc_parenthesized_expression : '(' preproc_expression_ ')' ;

preproc_defined : 'defined' '(' identifier ')' | 'defined' identifier ;

preproc_unary_expression : operator = ( '!' | '~' | '-' | '+' ) argument = preproc_expression_ ;

preproc_call_expression : function = identifier arguments = preproc_argument_list ;

preproc_argument_list : '(' ( preproc_expression_ ',' preproc_expression_* | empty ) ')' ;

preproc_binary_expression : left = preproc_expression_ operator = '+' right = preproc_expression_ | left = preproc_expression_ operator = '-' right = preproc_expression_ | left = preproc_expression_ operator = '*' right = preproc_expression_ | left = preproc_expression_ operator = '/' right = preproc_expression_ | left = preproc_expression_ operator = '%' right = preproc_expression_ | left = preproc_expression_ operator = '||' right = preproc_expression_ | left = preproc_expression_ operator = '&&' right = preproc_expression_ | left = preproc_expression_ operator = '|' right = preproc_expression_ | left = preproc_expression_ operator = '^' right = preproc_expression_ | left = preproc_expression_ operator = '&' right = preproc_expression_ | left = preproc_expression_ operator = '==' right = preproc_expression_ | left = preproc_expression_ operator = '!=' right = preproc_expression_ | left = preproc_expression_ operator = '>' right = preproc_expression_ | left = preproc_expression_ operator = '>=' right = preproc_expression_ | left = preproc_expression_ operator = '<=' right = preproc_expression_ | left = preproc_expression_ operator = '<' right = preproc_expression_ | left = preproc_expression_ operator = '<<' right = preproc_expression_ | left = preproc_expression_ operator = '>>' right = preproc_expression_ ;

function_definition : ( ms_call_modifier | empty ) declaration_specifiers_ declarator = declarator_ body = compound_statement ;

declaration : declaration_specifiers_ declarator = ( declarator_ | init_declarator ) ',' declarator = ( declarator_ | init_declarator )* ';' ;

type_definition : 'typedef' type_qualifier* type = type_specifier_ declarator = type_declarator_ ',' declarator = type_declarator_* ';' ;

declaration_modifiers_ : storage_class_specifier | type_qualifier | attribute_specifier | attribute_declaration | ms_declspec_modifier | virtual_function_specifier ;

declaration_specifiers_ : declaration_modifiers_* type = type_specifier_ declaration_modifiers_* ;

linkage_specification : 'extern' value = string_literal body = ( function_definition | declaration | declaration_list ) ;

attribute_specifier : '__attribute__' '(' argument_list ')' ;

attribute : ( prefix = identifier '::' | empty ) name = identifier ( argument_list | empty ) ;

attribute_declaration : '[[' attribute ',' attribute* ']]' ;

ms_declspec_modifier : '__declspec' '(' identifier ')' ;

ms_based_modifier : '__based' argument_list ;

ms_call_modifier : '__cdecl' | '__clrcall' | '__stdcall' | '__fastcall' | '__thiscall' | '__vectorcall' ;

ms_restrict_modifier : '__restrict' ;

ms_unsigned_ptr_modifier : '__uptr' ;

ms_signed_ptr_modifier : '__sptr' ;

ms_unaligned_ptr_modifier : '_unaligned' | '__unaligned' ;

ms_pointer_modifier : ms_unaligned_ptr_modifier | ms_restrict_modifier | ms_unsigned_ptr_modifier | ms_signed_ptr_modifier ;

declaration_list : '{' top_level_item_* '}' ;

declarator_ : attributed_declarator | pointer_declarator | function_declarator | array_declarator | parenthesized_declarator | identifier | reference_declarator | qualified_identifier | template_function | operator_name | destructor_name | structured_binding_declarator ;

field_declarator_ : attributed_field_declarator | pointer_field_declarator | function_field_declarator | array_field_declarator | parenthesized_field_declarator | field_identifier_ | reference_field_declarator | template_method | operator_name ;

type_declarator_ : attributed_type_declarator | pointer_type_declarator | function_type_declarator | array_type_declarator | parenthesized_type_declarator | type_identifier_ ;

abstract_declarator_ : abstract_pointer_declarator | abstract_function_declarator | abstract_array_declarator | abstract_parenthesized_declarator | abstract_reference_declarator ;

parenthesized_declarator : '(' declarator_ ')' ;

parenthesized_field_declarator : '(' field_declarator_ ')' ;

parenthesized_type_declarator : '(' type_declarator_ ')' ;

abstract_parenthesized_declarator : '(' abstract_declarator_ ')' ;

attributed_declarator : declarator_ attribute_declaration+ ;

attributed_field_declarator : field_declarator_ attribute_declaration+ ;

attributed_type_declarator : type_declarator_ attribute_declaration+ ;

pointer_declarator : ( ms_based_modifier | empty ) '*' ms_pointer_modifier* type_qualifier* declarator = declarator_ ;

pointer_field_declarator : ( ms_based_modifier | empty ) '*' ms_pointer_modifier* type_qualifier* declarator = field_declarator_ ;

pointer_type_declarator : ( ms_based_modifier | empty ) '*' ms_pointer_modifier* type_qualifier* declarator = type_declarator_ ;

abstract_pointer_declarator : '*' type_qualifier* declarator = ( abstract_declarator_ | empty ) ;

function_declarator : declarator = declarator_ parameters = parameter_list attribute_specifier* ( type_qualifier | ref_qualifier | virtual_specifier | noexcept | throw_specifier )* ( trailing_return_type | empty ) ( requires_clause | empty ) ;

function_field_declarator : declarator = field_declarator_ parameters = parameter_list ( type_qualifier | ref_qualifier | virtual_specifier | noexcept | throw_specifier )* ( trailing_return_type | empty ) ( requires_clause | empty ) ;

function_type_declarator : declarator = type_declarator_ parameters = parameter_list ;

abstract_function_declarator : declarator = ( abstract_declarator_ | empty ) parameters = parameter_list ( type_qualifier | ref_qualifier | noexcept | throw_specifier )* ( trailing_return_type | empty ) ( requires_clause | empty ) ;

array_declarator : declarator = declarator_ '[' type_qualifier* size = ( ( expression_ | '*' ) | empty ) ']' ;

array_field_declarator : declarator = field_declarator_ '[' type_qualifier* size = ( ( expression_ | '*' ) | empty ) ']' ;

array_type_declarator : declarator = type_declarator_ '[' type_qualifier* size = ( ( expression_ | '*' ) | empty ) ']' ;

abstract_array_declarator : declarator = ( abstract_declarator_ | empty ) '[' type_qualifier* size = ( ( expression_ | '*' ) | empty ) ']' ;

init_declarator : declarator = declarator_ '=' value = ( initializer_list | expression_ ) | declarator = declarator_ value = ( argument_list | initializer_list ) ;

compound_statement : '{' top_level_item_* '}' ;

storage_class_specifier : 'extern' | 'static' | 'register' | 'inline' | 'thread_local' ;

type_qualifier : 'const' | 'volatile' | 'restrict' | '_Atomic' | 'mutable' | 'constexpr' | 'constinit' | 'consteval' ;

type_specifier_ : struct_specifier | union_specifier | enum_specifier | class_specifier | sized_type_specifier | primitive_type | template_type | dependent_type | placeholder_type_specifier | decltype | qualified_type_identifier | type_identifier_ ;

sized_type_specifier : ( 'signed' | 'unsigned' | 'long' | 'short' )+ type = ( ( type_identifier_ | primitive_type ) | empty ) ;

primitive_type : 'bool' | 'char' | 'int' | 'float' | 'double' | 'void' | 'size_t' | 'ssize_t' | 'intptr_t' | 'uintptr_t' | 'charptr_t' | 'int8_t' | 'int16_t' | 'int32_t' | 'int64_t' | 'uint8_t' | 'uint16_t' | 'uint32_t' | 'uint64_t' | 'char8_t' | 'char16_t' | 'char32_t' | 'char64_t' ;

enum_specifier : 'enum' ( ( 'class' | 'struct' ) | empty ) ( name = class_name_ ( enum_base_clause_ | empty ) ( body = enumerator_list | empty ) | body = enumerator_list ) ;

enumerator_list : '{' ( enumerator ',' enumerator* | empty ) ( ',' | empty ) '}' ;

struct_specifier : 'struct' ( ms_declspec_modifier | empty ) ( attribute_declaration | empty ) ( name = class_name_ | ( name = class_name_ | empty ) ( virtual_specifier | empty ) ( base_class_clause | empty ) body = field_declaration_list ) ;

union_specifier : 'union' ( ms_declspec_modifier | empty ) ( attribute_declaration | empty ) ( name = class_name_ | ( name = class_name_ | empty ) ( virtual_specifier | empty ) ( base_class_clause | empty ) body = field_declaration_list ) ;

field_declaration_list : '{' field_declaration_list_item_* '}' ;

field_declaration_list_item_ : field_declaration | preproc_def | preproc_function_def | preproc_call | preproc_if_in_field_declaration_list | preproc_ifdef_in_field_declaration_list | template_declaration | inline_method_definition | constructor_or_destructor_definition | constructor_or_destructor_declaration | operator_cast_definition | operator_cast_declaration | friend_declaration | access_specifier | alias_declaration | using_declaration | type_definition | static_assert_declaration ;

field_declaration : declaration_specifiers_ ( declarator = field_declarator_ ',' declarator = field_declarator_* | empty ) ( ( bitfield_clause | default_value = initializer_list | '=' default_value = ( expression_ | initializer_list ) ) | empty ) ';' ;

bitfield_clause : ':' expression_ ;

enumerator : name = identifier ( '=' value = expression_ | empty ) ;

variadic_parameter : '...' ;

parameter_list : '(' ( ( parameter_declaration | optional_parameter_declaration | variadic_parameter_declaration | '...' ) ',' ( parameter_declaration | optional_parameter_declaration | variadic_parameter_declaration | '...' )* | empty ) ')' ;

parameter_declaration : declaration_specifiers_ ( declarator = ( declarator_ | abstract_declarator_ ) | empty ) ;

attributed_statement : attribute_declaration+ statement_ ;

attributed_non_case_statement : attribute_declaration+ non_case_statement_ ;

statement_ : case_statement | non_case_statement_ ;

non_case_statement_ : labeled_statement | compound_statement | expression_statement | if_statement | switch_statement | do_statement | while_statement | for_statement | return_statement | break_statement | continue_statement | goto_statement | co_return_statement | co_yield_statement | for_range_loop | try_statement | throw_statement ;

labeled_statement : label = statement_identifier_ ':' statement_ ;

expression_statement : ( ( expression_ | comma_expression ) | empty ) ';' ;

if_statement : 'if' ( 'constexpr' | empty ) condition = condition_clause consequence = statement_ ( 'else' alternative = statement_ | empty ) ;

switch_statement : 'switch' condition = condition_clause body = compound_statement ;

case_statement : ( 'case' value = expression_ | 'default' ) ':' ( attributed_non_case_statement | non_case_statement_ | declaration | type_definition )* ;

while_statement : 'while' condition = condition_clause body = statement_ ;

do_statement : 'do' body = statement_ 'while' condition = parenthesized_expression ';' ;

for_statement : 'for' '(' ( initializer = declaration | initializer = ( ( expression_ | comma_expression ) | empty ) ';' ) condition = ( expression_ | empty ) ';' update = ( ( expression_ | comma_expression ) | empty ) ')' statement_ ;

return_statement : 'return' ( ( expression_ | comma_expression ) | empty ) ';' | 'return' initializer_list ';' ;

break_statement : 'break' ';' ;

continue_statement : 'continue' ';' ;

goto_statement : 'goto' label = statement_identifier_ ';' ;

expression_ : conditional_expression | assignment_expression | binary_expression | unary_expression | update_expression | cast_expression | pointer_expression | sizeof_expression | subscript_expression | call_expression | field_expression | compound_literal_expression | identifier | number_literal | string_literal | true | false | null | concatenated_string | char_literal | parenthesized_expression | co_await_expression | requires_expression | requires_clause | template_function | qualified_identifier | new_expression | delete_expression | lambda_expression | parameter_pack_expansion | nullptr | this | raw_string_literal | user_defined_literal ;

comma_expression : left = expression_ ',' right = ( expression_ | comma_expression ) ;

conditional_expression : condition = expression_ '?' consequence = expression_ ':' alternative = expression_ ;

assignment_left_expression_ : identifier | call_expression | field_expression | pointer_expression | subscript_expression | parenthesized_expression | qualified_identifier ;

assignment_expression : left = assignment_left_expression_ operator = ( '=' | '*=' | '/=' | '%=' | '+=' | '-=' | '<<=' | '>>=' | '&=' | '^=' | '|=' ) right = expression_ ;

pointer_expression : operator = ( '*' | '&' ) argument = expression_ ;

unary_expression : operator = ( '!' | '~' | '-' | '+' ) argument = expression_ ;

binary_expression : left = expression_ operator = '+' right = expression_ | left = expression_ operator = '-' right = expression_ | left = expression_ operator = '*' right = expression_ | left = expression_ operator = '/' right = expression_ | left = expression_ operator = '%' right = expression_ | left = expression_ operator = '||' right = expression_ | left = expression_ operator = '&&' right = expression_ | left = expression_ operator = '|' right = expression_ | left = expression_ operator = '^' right = expression_ | left = expression_ operator = '&' right = expression_ | left = expression_ operator = '==' right = expression_ | left = expression_ operator = '!=' right = expression_ | left = expression_ operator = '>' right = expression_ | left = expression_ operator = '>=' right = expression_ | left = expression_ operator = '<=' right = expression_ | left = expression_ operator = '<' right = expression_ | left = expression_ operator = '<<' right = expression_ | left = expression_ operator = '>>' right = expression_ | left = expression_ operator = '<=>' right = expression_ ;

update_expression : operator = ( '--' | '++' ) argument = expression_ | argument = expression_ operator = ( '--' | '++' ) ;

cast_expression : '(' type = type_descriptor ')' value = expression_ ;

type_descriptor : type_qualifier* type = type_specifier_ type_qualifier* declarator = ( abstract_declarator_ | empty ) ;

sizeof_expression : 'sizeof' ( value = expression_ | '(' type = type_descriptor ')' ) | 'sizeof' '...' '(' value = identifier ')' ;

subscript_expression : argument = expression_ '[' index = ( expression_ | initializer_list ) ']' ;

call_expression : function = expression_ arguments = argument_list | function = primitive_type arguments = argument_list ;

argument_list : '(' ( ( expression_ | initializer_list ) ',' ( expression_ | initializer_list )* | empty ) ')' ;

field_expression : argument = expression_ operator = ( '.' | '->' ) field = field_identifier_ | argument = expression_ ( '.' | '->' ) field = ( destructor_name | template_method | dependent_field_identifier ) ;

compound_literal_expression : '(' type = type_descriptor ')' value = initializer_list | type = class_name_ value = initializer_list ;

parenthesized_expression : '(' ( expression_ | comma_expression ) ')' ;

initializer_list : '{' ( ( initializer_pair | expression_ | initializer_list ) ',' ( initializer_pair | expression_ | initializer_list )* | empty ) ( ',' | empty ) '}' ;

initializer_pair : designator = ( subscript_designator | field_designator )+ '=' value = ( expression_ | initializer_list ) ;

subscript_designator : '[' expression_ ']' ;

field_designator : '.' field_identifier_ ;

number_literal : ( '[-\\\\+]' | empty ) ( ( '0x' | '0b' ) | empty ) ( ( '[0-9]'+ '\'' '[0-9]'+* | '0b' '[0-9]'+ '\'' '[0-9]'+* | '0x' '[0-9a-fA-F]'+ '\'' '[0-9a-fA-F]'+* ) ( '.' ( '[0-9a-fA-F]'+ '\'' '[0-9a-fA-F]'+* | empty ) | empty ) | '.' '[0-9]'+ '\'' '[0-9]'+* ) ( '[eEpP]' ( ( '[-\\\\+]' | empty ) '[0-9a-fA-F]'+ '\'' '[0-9a-fA-F]'+* | empty ) | empty ) ( 'u' | 'l' | 'U' | 'L' | 'f' | 'F' )* ;

char_literal : ( 'L\'' | 'u\'' | 'U\'' | 'u8\'' | '\'' ) ( escape_sequence | '[^\\\\n\']' ) '\'' ;

concatenated_string : ( raw_string_literal | string_literal ) ( raw_string_literal | string_literal )+ ;

string_literal : ( 'L\\' | 'u\\' | 'U\\' | 'u8\\' | '\\' ) ( '[^\\\\\\\\\\\\\\n]+' | escape_sequence )* '\\' ;

escape_sequence : '\\\\' ( '[^xuU]' | '\\[0-9]{2,3}' | 'x[0-9a-fA-F]{2,}' | 'u[0-9a-fA-F]{4}' | 'U[0-9a-fA-F]{8}' ) ;

system_lib_string : '<' ( '[^>\\\\n]' | '\\\\>' )* '>' ;

true : 'TRUE' | 'true' ;

false : 'FALSE' | 'false' ;

null : 'NULL' ;

identifier : '[a-zA-Z_]\\\\w*' ;

type_identifier_ : identifier ;

field_identifier_ : identifier ;

statement_identifier_ : identifier ;

empty_declaration_ : type_specifier_ ';' ;

macro_type_specifier : name = identifier '(' type = type_descriptor ')' ;

comment : '//' '(\\\\\\\\(.|\\\\r?\\\\n)|[^\\\\\\\\\\\\n])*' | '/*' '[^*]*\\\\*+([^/*][^*]*\\\\*+)*' '/' ;

placeholder_type_specifier : constraint = ( type_specifier_ | empty ) ( auto | decltype_auto ) ;

auto : 'auto' ;

decltype_auto : 'decltype' '(' auto ')' ;

decltype : 'decltype' '(' expression_ ')' ;

class_specifier : 'class' ( ms_declspec_modifier | empty ) ( attribute_declaration | empty ) ( name = class_name_ | ( name = class_name_ | empty ) ( virtual_specifier | empty ) ( base_class_clause | empty ) body = field_declaration_list ) ;

class_name_ : type_identifier_ | template_type | qualified_type_identifier ;

virtual_specifier : 'final' | 'override' ;

virtual_function_specifier : 'virtual' ;

explicit_function_specifier : 'explicit' | 'explicit' '(' expression_ ')' ;

base_class_clause : ':' ( ( 'public' | 'private' | 'protected' ) | empty ) class_name_ ( '...' | empty ) ',' ( ( 'public' | 'private' | 'protected' ) | empty ) class_name_ ( '...' | empty )* ;

enum_base_clause_ : ':' base = ( qualified_type_identifier | type_identifier_ | sized_type_specifier ) ;

dependent_type : 'typename' type_specifier_ ;

template_declaration : 'template' parameters = template_parameter_list ( requires_clause | empty ) ( empty_declaration_ | alias_declaration | declaration | template_declaration | function_definition | concept_definition | constructor_or_destructor_declaration | constructor_or_destructor_definition | operator_cast_declaration | operator_cast_definition ) ;

template_instantiation : 'template' ( declaration_specifiers_ | empty ) declarator = declarator_ ';' ;

template_parameter_list : '<' ( ( parameter_declaration | optional_parameter_declaration | type_parameter_declaration | variadic_parameter_declaration | variadic_type_parameter_declaration | optional_type_parameter_declaration | template_template_parameter_declaration ) ',' ( parameter_declaration | optional_parameter_declaration | type_parameter_declaration | variadic_parameter_declaration | variadic_type_parameter_declaration | optional_type_parameter_declaration | template_template_parameter_declaration )* | empty ) '>' ;

type_parameter_declaration : ( 'typename' | 'class' ) ( type_identifier_ | empty ) ;

variadic_type_parameter_declaration : ( 'typename' | 'class' ) '...' ( type_identifier_ | empty ) ;

optional_type_parameter_declaration : ( 'typename' | 'class' ) ( name = type_identifier_ | empty ) '=' default_type = type_specifier_ ;

template_template_parameter_declaration : 'template' parameters = template_parameter_list ( type_parameter_declaration | variadic_type_parameter_declaration | optional_type_parameter_declaration ) ;

optional_parameter_declaration : declaration_specifiers_ declarator = ( declarator_ | empty ) '=' default_value = expression_ ;

variadic_parameter_declaration : declaration_specifiers_ declarator = ( variadic_declarator | variadic_reference_declarator ) ;

variadic_declarator : '...' ( identifier | empty ) ;

variadic_reference_declarator : ( '&&' | '&' ) variadic_declarator ;

operator_cast : 'operator' declaration_specifiers_ declarator = abstract_declarator_ ;

field_initializer_list : ':' field_initializer ',' field_initializer* ;

field_initializer : ( field_identifier_ | template_method | qualified_field_identifier ) ( initializer_list | argument_list ) ( '...' | empty ) ;

inline_method_definition : declaration_specifiers_ declarator = field_declarator_ ( body = compound_statement | default_method_clause | delete_method_clause ) ;

constructor_specifiers_ : declaration_modifiers_ | explicit_function_specifier ;

operator_cast_definition : constructor_specifiers_* declarator = ( operator_cast | qualified_operator_cast_identifier ) body = compound_statement ;

operator_cast_declaration : constructor_specifiers_* declarator = ( operator_cast | qualified_operator_cast_identifier ) ( '=' default_value = expression_ | empty ) ';' ;

constructor_or_destructor_definition : constructor_specifiers_* declarator = function_declarator ( field_initializer_list | empty ) ( body = compound_statement | default_method_clause | delete_method_clause ) ;

constructor_or_destructor_declaration : constructor_specifiers_* declarator = function_declarator ';' ;

default_method_clause : '=' 'default' ';' ;

delete_method_clause : '=' 'delete' ';' ;

friend_declaration : 'friend' ( declaration | function_definition | ( ( 'class' | 'struct' | 'union' ) | empty ) class_name_ ';' ) ;

access_specifier : ( 'public' | 'private' | 'protected' ) ':' ;

reference_declarator : ( '&' | '&&' ) declarator_ ;

reference_field_declarator : ( '&' | '&&' ) field_declarator_ ;

abstract_reference_declarator : ( '&' | '&&' ) ( abstract_declarator_ | empty ) ;

structured_binding_declarator : '[' identifier ',' identifier* ']' ;

ref_qualifier : '&' | '&&' ;

trailing_return_type : '->' ( type_qualifier | empty ) type_specifier_ ( abstract_declarator_ | empty ) ;

noexcept : 'noexcept' ( '(' ( expression_ | empty ) ')' | empty ) ;

throw_specifier : 'throw' '(' ( type_descriptor ',' type_descriptor* | empty ) ')' ;

template_type : name = type_identifier_ arguments = template_argument_list ;

template_method : name = field_identifier_ arguments = template_argument_list ;

template_function : name = identifier arguments = template_argument_list ;

template_argument_list : '<' ( ( type_descriptor | type_parameter_pack_expansion | expression_ ) ',' ( type_descriptor | type_parameter_pack_expansion | expression_ )* | empty ) '>' ;

namespace_definition : 'namespace' name = ( ( identifier | namespace_definition_name ) | empty ) body = declaration_list ;

namespace_alias_definition : 'namespace' name = identifier '=' ( identifier | qualified_identifier ) ';' ;

namespace_definition_name : ( identifier | namespace_definition_name ) '::' ( 'inline' | empty ) identifier ;

using_declaration : 'using' ( 'namespace' | empty ) ( identifier | qualified_identifier ) ';' ;

alias_declaration : 'using' name = type_identifier_ '=' type = type_descriptor ';' ;

static_assert_declaration : 'static_assert' '(' condition = expression_ ( ',' message = ( string_literal | raw_string_literal | concatenated_string ) | empty ) ')' ';' ;

concept_definition : 'concept' name = identifier '=' expression_ ';' ;

condition_clause : '(' ( initializer = ( ( declaration | expression_statement ) | empty ) value = ( expression_ | comma_expression ) | value = condition_declaration ) ')' ;

condition_declaration : declaration_specifiers_ declarator = declarator_ ( '=' value = expression_ | value = initializer_list ) ;

for_range_loop : 'for' '(' declaration_specifiers_ declarator = declarator_ ':' right = ( expression_ | initializer_list ) ')' body = statement_ ;

co_return_statement : 'co_return' ( expression_ | empty ) ';' ;

co_yield_statement : 'co_yield' expression_ ';' ;

throw_statement : 'throw' ( expression_ | empty ) ';' ;

try_statement : 'try' body = compound_statement catch_clause+ ;

catch_clause : 'catch' parameters = parameter_list body = compound_statement ;

co_await_expression : operator = 'co_await' argument = expression_ ;

new_expression : ( '::' | empty ) 'new' placement = ( argument_list | empty ) type = type_specifier_ declarator = ( new_declarator | empty ) arguments = ( ( argument_list | initializer_list ) | empty ) ;

new_declarator : '[' length = expression_ ']' ( new_declarator | empty ) ;

delete_expression : ( '::' | empty ) 'delete' ( '[' ']' | empty ) expression_ ;

type_requirement : 'typename' class_name_ ;

compound_requirement : '{' expression_ '}' ( 'noexcept' | empty ) ( trailing_return_type | empty ) ';' ;

requirement_ : expression_statement | type_requirement | compound_requirement ;

requirement_seq : '{' requirement_* '}' ;

requires_clause : 'requires' constraint = ( class_name_ | requires_expression ) ;

requires_expression : 'requires' parameters = ( parameter_list | empty ) requirements = requirement_seq ;

lambda_expression : captures = lambda_capture_specifier ( template_parameters = template_parameter_list ( constraint = requires_clause | empty ) | empty ) ( declarator = abstract_function_declarator | empty ) body = compound_statement ;

lambda_capture_specifier : '[' ( lambda_default_capture | ( expression_ ',' expression_* | empty ) | lambda_default_capture ',' expression_ ',' expression_* ) ']' ;

lambda_default_capture : '=' | '&' ;

parameter_pack_expansion : pattern = expression_ '...' ;

type_parameter_pack_expansion : pattern = type_descriptor '...' ;

destructor_name : '~' identifier ;

dependent_identifier : 'template' template_function ;

dependent_field_identifier : 'template' template_method ;

dependent_type_identifier : 'template' template_type ;

scope_resolution_ : scope = ( ( namespace_identifier_ | template_type | dependent_type_identifier ) | empty ) '::' ;

qualified_field_identifier : scope_resolution_ name = ( dependent_field_identifier | qualified_field_identifier | template_method | field_identifier_ ) ;

qualified_identifier : scope_resolution_ name = ( dependent_identifier | qualified_identifier | template_function | identifier | operator_name | destructor_name ) ;

qualified_type_identifier : scope_resolution_ name = ( dependent_type_identifier | qualified_type_identifier | template_type | type_identifier_ ) ;

qualified_operator_cast_identifier : scope_resolution_ name = ( qualified_operator_cast_identifier | operator_cast ) ;

operator_name : 'operator' ( 'co_await' | '+' | '-' | '*' | '/' | '%' | '^' | '&' | '|' | '~' | '!' | '=' | '<' | '>' | '+=' | '-=' | '*=' | '/=' | '%=' | '^=' | '&=' | '|=' | '<<' | '>>' | '>>=' | '<<=' | '==' | '!=' | '<=' | '>=' | '<=>' | '&&' | '||' | '++' | '--' | ',' | '->*' | '->' | '()' | '[]' | ( 'new' | 'delete' ) ( '[]' | empty ) | '\\\\' identifier ) ;

this : 'this' ;

nullptr : 'nullptr' ;

literal_suffix : '[a-zA-Z_]\\\\w*' ;

user_defined_literal : ( number_literal | char_literal | string_literal | raw_string_literal | concatenated_string ) literal_suffix ;

namespace_identifier_ : identifier ;


