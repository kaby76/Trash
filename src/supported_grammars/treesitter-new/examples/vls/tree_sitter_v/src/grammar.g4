grammar foo;

source_file : (( top_level_declaration_ | statement_ ) ( automatic_separator_ | empty ))* ;

top_level_declaration_ : const_declaration | global_var_declaration | function_declaration | type_declaration | struct_declaration | binded_struct_declaration_ | enum_declaration | interface_declaration | import_declaration | module_clause ;

expression_ : empty_literal_value | int_literal | float_literal | string_literal_ | rune_literal | reserved_identifier_ | binded_identifier | identifier | single_line_expression_ | type_initializer | map | array | fixed_array | unary_expression | binary_expression | is_expression | index_expression | slice_expression | type_cast_expression | as_type_cast_expression | call_expression | special_call_expression | fn_literal | selector_expression | parenthesized_expression | expression_with_blocks_ ;

parenthesized_expression : '(' expression_ ')' ;

unary_expression : operator = ( '+' | '-' | '!' | '~' | '^' | '*' | '&' | '<-' ) operand = expression_ ;

binary_expression : left = expression_ operator = ( '*' | '/' | '%' | '<<' | '>>' | '>>>' | '&' | '&^' ) right = expression_ | left = expression_ operator = ( '+' | '-' | '|' | '^' ) right = expression_ | left = expression_ operator = ( '==' | '!=' | '<' | '<=' | '>' | '>=' | 'in' | '!in' ) right = expression_ | left = expression_ operator = '&&' right = expression_ | left = expression_ operator = '||' right = expression_ ;

as_type_cast_expression : expression_ 'as' simple_type_ ;

type_cast_expression : type = simple_type_ '(' operand = expression_ ')' ;

call_expression : function = ( identifier | binded_identifier | comptime_identifier | selector_expression | comptime_selector_expression ) type_parameters = ( type_parameters | empty ) arguments = argument_list ( option_propagator | empty ) ;

special_argument_list : '(' ( simple_type_ | option_type ) ( ',' expression_ | empty ) ')' ;

special_call_expression : function = ( identifier | selector_expression ) arguments = special_argument_list ( option_propagator | empty ) ;

comptime_identifier : '$' identifier ;

comptime_selector_expression : '$' '(' selector_expression ')' ;

option_propagator : '?' | or_block ;

or_block : 'or' block ;

expression_with_blocks_ : if_expression | match_expression | select_expression | sql_expression | lock_expression | unsafe_expression | comptime_if_expression ;

single_line_expression_ : pseudo_comptime_identifier | type_selector_expression | none | true | false ;

comment : comment_ ;

escape_sequence : '\\\\' ( 'u[a-fA-F\\[0-9]]{4}' | 'U[a-fA-F\\[0-9]]{8}' | 'x[a-fA-F\\[0-9]]{2}' | '\\[0-9]{3}' | '\\\\r?\\\\n' | '[\'\\abfrntv\\\\$\\\\\\\\]' | '\\\\S' ) ;

none : 'none' ;

true : 'true' ;

false : 'false' ;

spread_operator : '...' expression_ ;

type_initializer : type = ( builtin_type | type_identifier | type_placeholder | generic_type | binded_type_ | qualified_type | pointer_type | array_type | fixed_array_type | map_type | channel_type ) body = literal_value ;

literal_value : '{' ( ( ( spread_operator | keyed_element ) ( ( ',' | ( '\\n' | '\\r' | '\\r\\n' ) ) | empty )* | element ',' element* ) | empty ) '}' ;

element : expression_ ;

keyed_element : name = element_key_ ':' value = expression_ ;

element_key_ : field_identifier_ | type_identifier | string_literal_ | int_literal | call_expression | selector_expression | type_selector_expression | index_expression ;

map : '{' keyed_element ( ( ',' | ( '\\n' | '\\r' | '\\r\\n' ) ) | empty )+ '}' ;

array : non_empty_array_ ;

fixed_array : non_empty_array_ '!' ;

non_empty_array_ : '[' expression_ ( ',' | empty )* ']' ;

fixed_array_type : '[' size = ( int_literal | identifier ) ']' element = simple_type_ ;

array_type : '[' ']' element = simple_type_ ;

variadic_type : '...' simple_type_ ;

pointer_type : '&' simple_type_ ;

map_type : 'map[' key = simple_type_ ']' value = type_ ;

channel_type : 'chan' value = simple_type_ ;

shared_type : 'shared' simple_type_ ;

thread_type : 'thread' simple_type_ ;

int_literal : '0' ( 'b' | 'B' ) ( '_' | empty ) '[01]' ( '_' | empty ) '[01]'* | '0' | '[1-9]' ( ( '_' | empty ) '[0-9]' ( '_' | empty ) '[0-9]'* | empty ) | '0' ( ( 'o' | 'O' ) | empty ) ( '_' | empty ) '[0-7]' ( '_' | empty ) '[0-7]'* | '0' ( 'x' | 'X' ) ( '_' | empty ) '[0-9a-fA-F]' ( '_' | empty ) '[0-9a-fA-F]'* ;

float_literal : '[0-9]' ( '_' | empty ) '[0-9]'* '.' '[0-9]' ( '_' | empty ) '[0-9]'* ( ( 'e' | 'E' ) ( ( '+' | '-' ) | empty ) '[0-9]' ( '_' | empty ) '[0-9]'* | empty ) | '[0-9]' ( '_' | empty ) '[0-9]'* ( 'e' | 'E' ) ( ( '+' | '-' ) | empty ) '[0-9]' ( '_' | empty ) '[0-9]'* | '.' '[0-9]' ( '_' | empty ) '[0-9]'* ( ( 'e' | 'E' ) ( ( '+' | '-' ) | empty ) '[0-9]' ( '_' | empty ) '[0-9]'* | empty ) | '0' ( 'x' | 'X' ) ( ( '_' | empty ) '[0-9a-fA-F]' ( '_' | empty ) '[0-9a-fA-F]'* '.' ( '[0-9a-fA-F]' ( '_' | empty ) '[0-9a-fA-F]'* | empty ) | ( '_' | empty ) '[0-9a-fA-F]' ( '_' | empty ) '[0-9a-fA-F]'* | '.' '[0-9a-fA-F]' ( '_' | empty ) '[0-9a-fA-F]'* ) ( 'p' | 'P' ) ( ( '+' | '-' ) | empty ) '[0-9]' ( '_' | empty ) '[0-9]'* ;

rune_literal : '`' ( '[^\'\\\\\\\\]' | '\'' | '\\' | '\\\\' ( '0' | '`' | 'x' '[0-9a-fA-F]' '[0-9a-fA-F]' | '[0-7]' '[0-7]' '[0-7]' | 'u' '[0-9a-fA-F]' '[0-9a-fA-F]' '[0-9a-fA-F]' '[0-9a-fA-F]' | 'U' '[0-9a-fA-F]' '[0-9a-fA-F]' '[0-9a-fA-F]' '[0-9a-fA-F]' '[0-9a-fA-F]' '[0-9a-fA-F]' '[0-9a-fA-F]' '[0-9a-fA-F]' | ( 'a' | 'b' | 'e' | 'f' | 'n' | 'r' | 't' | 'v' | '\\\\' | '\'' | '\\' ) ) ) '`' ;

string_literal_ : c_string_literal | raw_string_literal | interpreted_string_literal ;

c_string_literal : c_string_opening_ ( string_content_ | escape_sequence | string_interpolation | '$' )* string_closing_ ;

raw_string_literal : raw_string_opening_ string_content_* string_closing_ ;

interpreted_string_literal : string_opening_ ( string_content_ | escape_sequence | string_interpolation | '$' )* string_closing_ ;

string_interpolation : braced_interpolation_opening_ expression_ ( format_specifier | empty ) interpolation_closing_ | unbraced_interpolation_opening_ ( identifier | selector_expression ) ;

format_specifier : ':' ( '[gGeEfFcdoxXpsSc]' | ( '[+-0]' | empty ) int_literal ( '.' int_literal | empty ) ( '[gGeEfFcdoxXpsSc]' | empty ) ) ;

reserved_identifier_ : 'array' | 'string' | 'char' | 'sql' ;

identifier : ( '[a-zà-?æ]' | '_' ) ( ( '[a-zA-Zà-?à-êæ]' | '_' ) | '[0-9]' )* | '@' ( 'pub' | 'none' | 'const' | 'mut' | '__global' | 'fn' | 'assert' | 'as' | 'go' | 'asm' | 'return' | 'type' | 'if' | 'else' | 'for' | 'in' | 'is' | 'union' | 'struct' | 'enum' | 'interface' | 'defer' | 'unsafe' | 'import' | 'match' | 'lock' | 'rlock' | 'select' | 'voidptr' | 'byteptr' | 'charptr' | 'i8' | 'i16' | 'i32' | 'int' | 'i64' | 'byte' | 'u8' | 'u16' | 'u32' | 'u64' | 'f32' | 'f64' | 'char' | 'bool' | 'string' | 'rune' | 'array' | 'map' | 'mapnode' | 'chan' | 'size_t' | 'usize' | 'isize' | 'float_literal' | 'int_literal' | 'thread' | 'IError' ) ;

immediate_identifier : '[a-zà-?æ]' ( ( '[a-zA-Zà-?à-êæ]' | '_' ) | '[0-9]' | '_' )* ;

old_identifier_ : ( '[a-zA-Zà-?à-êæ]' | '_' ) ( ( '[a-zA-Zà-?à-êæ]' | '_' ) | '[0-9]' )* ;

mutable_prefix_ : 'mut' ( 'static' | empty ) | 'shared' ;

mutable_identifier : mutable_prefix_ ( identifier | reserved_identifier_ ) ;

mutable_expression_2_ : mutable_prefix_ ( selector_expression | index_expression ) ;

mutable_expression : mutable_prefix_ expression_ ;

binded_identifier : language = ( 'C' | 'JS' ) '.' name = ( identifier | old_identifier_ ) ;

identifier_list : ( mutable_identifier | identifier | reserved_identifier_ ) ',' ( mutable_identifier | identifier | reserved_identifier_ )* ;

expression_list : ( expression_ | mutable_expression ) ',' ( expression_ | mutable_expression )* ;

expression_list_repeat1_ : ( expression_ | mutable_expression ) ',' ( expression_ | mutable_expression )+ ;

parameter_declaration : name = ( mutable_identifier | identifier | reserved_identifier_ ) type = ( simple_type_ | option_type | variadic_type ) ;

parameter_list : '(' ( parameter_declaration ',' parameter_declaration* | empty ) ')' ;

empty_literal_value : '{' '}' ;

argument_list : '(' ( ( expression_ | mutable_expression | keyed_element | spread_operator ) ( ',' | automatic_separator_ ) ( expression_ | mutable_expression | keyed_element | spread_operator )* ( automatic_separator_ | empty ) | empty ) ')' ;

type_ : simple_type_ | option_type | multi_return_type ;

option_type : '?' ( ( simple_type_ | multi_return_type ) | empty ) ;

multi_return_type : '(' simple_type_ ',' simple_type_* ')' ;

type_list : simple_type_ ',' simple_type_* ;

simple_type_ : builtin_type | type_identifier | type_placeholder | binded_type_ | qualified_type | pointer_type | array_type | fixed_array_type | function_type | generic_type | map_type | channel_type | shared_type | thread_type ;

type_parameters : '<' simple_type_ ',' simple_type_* '>' ;

builtin_type : 'voidptr' | 'byteptr' | 'charptr' | 'i8' | 'i16' | 'i32' | 'int' | 'i64' | 'byte' | 'u8' | 'u16' | 'u32' | 'u64' | 'f32' | 'f64' | 'char' | 'bool' | 'string' | 'rune' | 'array' | 'map' | 'mapnode' | 'chan' | 'size_t' | 'usize' | 'isize' | 'float_literal' | 'int_literal' | 'thread' | 'IError' ;

binded_type_ : binded_identifier ;

generic_type : ( qualified_type | type_identifier ) type_parameters ;

qualified_type : module = module_identifier_ '.' name = type_identifier ;

type_placeholder : '[A-Zà-ê]' ;

pseudo_comptime_identifier : '@' '[A-Z][A-Z0-9_]+' ;

type_identifier : '[A-Zà-ê]' ( ( '[a-zA-Zà-?à-êæ]' | '_' ) | '[0-9]' )+ ;

module_identifier_ : identifier ;

field_identifier_ : identifier ;

statement_list_ : statement_ ( automatic_separator_ | empty )+ ;

statement_ : simple_statement_ | assert_statement | continue_statement | break_statement | return_statement | asm_statement | go_statement | goto_statement | labeled_statement | defer_statement | for_statement | comptime_for_statement | send_statement | block | hash_statement ;

simple_statement_ : expression_ | inc_statement | dec_statement | assignment_statement | short_var_declaration ;

inc_statement : expression_ '++' ;

dec_statement : expression_ '--' ;

send_statement : channel = expression_ '<-' value = expression_ ;

short_var_declaration : left = expression_list ':=' right = expression_list ;

assignment_statement : left = expression_list operator = ( '*=' | '/=' | '%=' | '<<=' | '>>=' | '>>>=' | '&=' | '&^=' | '+=' | '-=' | '|=' | '^=' | '=' ) right = expression_list ;

assert_statement : 'assert' expression_ ;

block : '{' ( ( statement_list_ | expression_list_repeat1_ | empty_labeled_statement | statement_list_ ( expression_list_repeat1_ | empty_labeled_statement ) ) | empty ) '}' ;

defer_statement : 'defer' block ;

unsafe_expression : 'unsafe' block ;

overloadable_operator : '+' | '-' | '*' | '/' | '%' | '<' | '>' | '==' | '!=' | '<=' | '>=' ;

exposed_variables_list : '[' expression_list ']' ;

function_declaration : attributes = ( attribute_list | empty ) ( 'pub' | empty ) 'fn' receiver = ( parameter_list | empty ) exposed_variables = ( exposed_variables_list | empty ) name = ( binded_identifier | identifier | overloadable_operator ) type_parameters = ( type_parameters | empty ) parameters = ( parameter_list | type_only_parameter_list ) result = ( type_ | empty ) body = ( block | empty ) ;

function_type : 'fn' parameters = ( parameter_list | type_only_parameter_list ) result = ( type_ | empty ) ;

type_only_parameter_list : '(' ( type_parameter_declaration ',' type_parameter_declaration* | empty ) ')' ;

type_parameter_declaration : ( 'mut' | empty ) type = ( simple_type_ | option_type | variadic_type ) ;

fn_literal : 'fn' exposed_variables = ( exposed_variables_list | empty ) parameters = parameter_list result = ( type_ | empty ) body = block arguments = ( argument_list | empty ) ;

global_var_declaration : '__global' ( global_var_spec_ | global_var_type_initializer | '(' ( global_var_spec_ | global_var_type_initializer ) ( '\\n' | '\\r' | '\\r\\n' )* ')' ) ;

global_var_spec_ : const_spec ;

global_var_type_initializer : name = identifier type = type_ ;

const_declaration : ( 'pub' | empty ) 'const' ( const_spec | '(' const_spec ( '\\n' | '\\r' | '\\r\\n' )+ ')' ) ;

const_spec : name = ( identifier | old_identifier_ ) '=' value = expression_ ;

asm_statement : 'asm' identifier content_block_ ;

sql_expression : 'sql' ( identifier | empty ) content_block_ ;

content_block_ : '{' '[^{}]+' '}' ;

break_statement : 'break' ( identifier | empty ) ;

continue_statement : 'continue' ( identifier | empty ) ;

return_statement : 'return' ( expression_list | empty ) ;

type_declaration : ( 'pub' | empty ) 'type' name = ( type_identifier | builtin_type ) type_parameters = ( type_parameters | empty ) '=' types = sum_type_list ;

sum_type_list : simple_type_ '|' simple_type_* ;

go_statement : 'go' expression_ ;

goto_statement : 'goto' identifier ;

labeled_statement : label = identifier ':' statement_ ;

empty_labeled_statement : label = identifier ':' ;

for_statement : 'for' ( ( for_in_operator | cstyle_for_clause | expression_ ) | empty ) body = block ;

comptime_for_statement : '$for' for_in_operator body = block ;

for_in_operator : left = ( expression_ | identifier_list ) 'in' right = ( definite_range_ | expression_ ) ;

definite_range_ : start = expression_ ( '..' | '...' ) end = expression_ ;

range_ : start = ( expression_ | empty ) '..' end = ( expression_ | empty ) ;

selector_expression : operand = ( expression_ | comptime_identifier ) '.' field = ( identifier | type_identifier | type_placeholder | reserved_identifier_ | comptime_identifier | comptime_selector_expression ) ;

index_expression : operand = expression_ '[' index = expression_ ']' ( option_propagator | empty ) ;

slice_expression : operand = expression_ '[' range_ ']' ;

cstyle_for_clause : initializer = ( simple_statement_ | empty ) ';' condition = ( expression_ | empty ) ';' update = ( simple_statement_ | empty ) ;

comptime_if_expression : '$if' condition = expression_ ( '?' | empty ) consequence = block ( '$else' alternative = ( block | comptime_if_expression ) | empty ) ;

if_expression : 'if' ( condition = expression_ | initializer = short_var_declaration ) consequence = block ( 'else' alternative = ( block | if_expression ) | empty ) ;

is_expression : left = ( type_placeholder | mutable_identifier | mutable_expression_2_ | mutable_expression | expression_ ) ( 'is' | '!is' ) right = ( option_type | simple_type_ | none ) ;

attribute_spec : 'if' identifier ( '?' | empty ) | 'unsafe' | identifier | interpreted_string_literal | name = ( 'unsafe' | identifier ) ':' value = ( string_literal_ | identifier ) ;

attribute_declaration : '[' attribute_spec ';' attribute_spec* ']' ;

attribute_list : attribute_declaration ( ( '\\n' | '\\r' | '\\r\\n' ) | empty )+ ;

struct_declaration : attributes = ( attribute_list | empty ) ( 'pub' | empty ) ( 'struct' | 'union' ) name = ( type_identifier | builtin_type | generic_type ) struct_field_declaration_list ;

struct_field_declaration_list : '{' ( struct_field_scope | struct_field_declaration ) ( ( '\\n' | '\\r' | '\\r\\n' ) | empty )* '}' ;

struct_field_scope : ( 'pub' | 'mut' | 'pub' 'mut' | '__global' ) ':' ;

struct_field_declaration : name = field_identifier_ type = ( simple_type_ | option_type ) attributes = ( attribute_declaration | empty ) ( '=' default_value = expression_ | empty ) ( ( '\\n' | '\\r' | '\\r\\n' ) | empty ) | type = ( type_identifier | qualified_type ) ( ( '\\n' | '\\r' | '\\r\\n' ) | empty ) ;

binded_struct_declaration_ : attributes = ( attribute_list | empty ) ( 'pub' | empty ) ( 'struct' | 'union' ) name = binded_type_ binded_struct_field_declaration_list_ ;

binded_struct_field_declaration_list_ : '{' ( struct_field_scope | binded_struct_field_declaration_ ) ( ( '\\n' | '\\r' | '\\r\\n' ) | empty )* '}' ;

binded_struct_field_declaration_ : name = ( field_identifier_ | old_identifier_ ) type = ( simple_type_ | option_type ) attributes = ( attribute_declaration | empty ) ( '=' default_value = expression_ | empty ) ( ( '\\n' | '\\r' | '\\r\\n' ) | empty ) ;

enum_declaration : ( attribute_list | empty ) ( 'pub' | empty ) 'enum' name = type_identifier enum_member_declaration_list ;

enum_member_declaration_list : '{' ( enum_member ( ( '\\n' | '\\r' | '\\r\\n' ) | empty )* | empty ) '}' ;

enum_member : name = identifier ( '=' value = expression_ | empty ) ;

type_selector_expression : type = ( ( type_placeholder | type_identifier ) | empty ) '.' field_name = ( reserved_identifier_ | identifier ) ;

interface_declaration : attributes = ( attribute_list | empty ) ( 'pub' | empty ) 'interface' name = ( type_identifier | generic_type ) interface_spec_list ;

interface_spec_list : '{' ( ( struct_field_declaration | interface_spec | interface_field_scope ) ( ( '\\n' | '\\r' | '\\r\\n' ) | empty )* | empty ) '}' ;

interface_field_scope : 'mut' ':' ;

interface_spec : name = field_identifier_ parameters = ( parameter_list | type_only_parameter_list ) result = ( type_ | empty ) ;

hash_statement : '#' '.|\\\\\\\\\\\\r?\\\\n'+ ( '\\n' | '\\r' | '\\r\\n' ) ;

module_clause : attributes = ( attribute_list | empty ) 'module' ' ' immediate_identifier ;

import_declaration : 'import' ' ' path = import_path ( ' ' ( alias = import_alias | symbols = import_symbols ) | empty ) ;

import_path : ( '[a-zA-Zà-?à-êæ]' | '_' ) ( ( '[a-zA-Zà-?à-êæ]' | '_' ) | '[0-9]' | '.' )* ;

import_symbols : '{' import_symbols_list '}' ;

import_symbols_list : ( identifier | type_identifier ) ',' ( identifier | type_identifier )* ;

import_alias : 'as' ' ' name = immediate_identifier ;

match_expression : 'match' condition = ( expression_ | mutable_expression ) '{' expression_case* ( default_case | empty ) '}' ;

case_list : ( expression_ | simple_type_ | definite_range_ ) ',' ( expression_ | simple_type_ | definite_range_ )* ;

expression_case : value = case_list consequence = block ;

default_case : 'else' consequence = block ;

select_expression : 'select' selected_variables = ( expression_list | empty ) '{' select_branch* ( select_default_branch | empty ) '}' ;

select_branch : short_var_declaration block ;

select_default_branch : ( ( '>' | empty ) expression_ | 'else' ) block ;

lock_expression : ( 'lock' | 'rlock' ) locked_variables = ( expression_list | empty ) body = block ;


