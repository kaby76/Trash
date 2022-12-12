grammar foo;
rsource_file :  (  (  (  r_top_level_declaration  |  r_statement  )  |  (  r_automatic_separator  |  empty  )  )  )*  ;

r_top_level_declaration :  (  rconst_declaration  |  rglobal_var_declaration  |  rfunction_declaration  |  rtype_declaration  |  rstruct_declaration  |  (  r_binded_struct_declaration  )  |  renum_declaration  |  rinterface_declaration  |  rimport_declaration  |  rmodule_clause  )  ;

r_expression :  (  rempty_literal_value  |  rint_literal  |  rfloat_literal  |  r_string_literal  |  rrune_literal  |  r_reserved_identifier  |  rbinded_identifier  |  ridentifier  |  r_single_line_expression  |  rtype_initializer  |  rmap  |  rarray  |  rfixed_array  |  runary_expression  |  rbinary_expression  |  ris_expression  |  rindex_expression  |  rslice_expression  |  rtype_cast_expression  |  ras_type_cast_expression  |  rcall_expression  |  rspecial_call_expression  |  rfn_literal  |  rselector_expression  |  rparenthesized_expression  |  r_expression_with_blocks  )  ;

rparenthesized_expression :  (  '('  |  r_expression  |  ')'  )  ;

runary_expression :  (  (  operator =  (  '+'  |  '-'  |  '!'  |  '~'  |  '^'  |  '*'  |  '&'  |  '<-'  )  |  operand =  (  r_expression  )  )  )  ;

rbinary_expression :  (  (  (  left =  r_expression  |  operator =  (  '*'  |  '/'  |  '%'  |  '<<'  |  '>>'  |  '>>>'  |  '&'  |  '&^'  )  |  right =  r_expression  )  )  |  (  (  left =  r_expression  |  operator =  (  '+'  |  '-'  |  '|'  |  '^'  )  |  right =  r_expression  )  )  |  (  (  left =  r_expression  |  operator =  (  '=='  |  '!='  |  '<'  |  '<='  |  '>'  |  '>='  |  'in'  |  '!in'  )  |  right =  r_expression  )  )  |  (  (  left =  r_expression  |  operator =  '&&'  |  right =  r_expression  )  )  |  (  (  left =  r_expression  |  operator =  '||'  |  right =  r_expression  )  )  )  ;

ras_type_cast_expression :  (  r_expression  |  'as'  |  r_simple_type  )  ;

rtype_cast_expression :  (  type =  r_simple_type  |  '('  |  operand =  r_expression  |  ')'  )  ;

rcall_expression :  (  (  function =  (  ridentifier  |  rbinded_identifier  |  rcomptime_identifier  |  rselector_expression  |  rcomptime_selector_expression  )  |  type_parameters =  (  rtype_parameters  |  empty  )  |  arguments =  rargument_list  |  (  roption_propagator  |  empty  )  )  )  ;

rspecial_argument_list :  (  '('  |  (  r_simple_type  |  roption_type  )  |  (  (  ','  |  r_expression  )  |  empty  )  |  ')'  )  ;

rspecial_call_expression :  (  (  function =  (  ridentifier  |  rselector_expression  )  |  arguments =  rspecial_argument_list  |  (  roption_propagator  |  empty  )  )  )  ;

rcomptime_identifier :  (  '$'  |  ridentifier  )  ;

rcomptime_selector_expression :  (  '$'  |  (  '('  |  rselector_expression  |  ')'  )  )  ;

roption_propagator :  (  (  (  '?'  )  |  ror_block  )  )  ;

ror_block :  (  'or'  |  rblock  )  ;

r_expression_with_blocks :  (  rif_expression  |  rmatch_expression  |  rselect_expression  |  rsql_expression  |  rlock_expression  |  runsafe_expression  |  rcomptime_if_expression  )  ;

r_single_line_expression :  (  (  rpseudo_comptime_identifier  |  rtype_selector_expression  |  rnone  |  rtrue  |  rfalse  )  )  ;

rcomment :  r_comment  ;

rescape_sequence :  (  (  (  '\\'  |  (  'u[a-fA-F\[0-9]]{4}'  |  'U[a-fA-F\[0-9]]{8}'  |  'x[a-fA-F\[0-9]]{2}'  |  '\[0-9]{3}'  |  '\\r?\\n'  |  '[\'\abfrntv\\$\\\\]'  |  '\\S'  )  )  )  )  ;

rnone :  'none'  ;

rtrue :  'true'  ;

rfalse :  'false'  ;

rspread_operator :  (  (  '...'  |  r_expression  )  )  ;

rtype_initializer :  (  (  type =  (  rbuiltin_type  |  rtype_identifier  |  rtype_placeholder  |  rgeneric_type  |  r_binded_type  |  rqualified_type  |  rpointer_type  |  rarray_type  |  rfixed_array_type  |  rmap_type  |  rchannel_type  )  |  body =  rliteral_value  )  )  ;

rliteral_value :  (  '{'  |  (  (  (  (  (  rspread_operator  |  rkeyed_element  )  |  (  (  ','  |  (  '\n'  |  '\r'  |  '\r\n'  )  )  |  empty  )  )  )*  |  (  relement  |  (  (  ','  |  relement  )  )*  )  )  |  empty  )  |  '}'  )  ;

relement :  r_expression  ;

rkeyed_element :  (  name =  r_element_key  |  ':'  |  value =  r_expression  )  ;

r_element_key :  (  (  r_field_identifier  )  |  rtype_identifier  |  r_string_literal  |  rint_literal  |  rcall_expression  |  rselector_expression  |  rtype_selector_expression  |  rindex_expression  )  ;

rmap :  (  (  '{'  |  (  (  rkeyed_element  |  (  (  ','  |  (  '\n'  |  '\r'  |  '\r\n'  )  )  |  empty  )  )  )+  |  '}'  )  )  ;

rarray :  (  r_non_empty_array  )  ;

rfixed_array :  (  (  r_non_empty_array  |  '!'  )  )  ;

r_non_empty_array :  (  '['  |  (  (  r_expression  |  (  ','  |  empty  )  )  )*  |  ']'  )  ;

rfixed_array_type :  (  '['  |  size =  (  rint_literal  |  ridentifier  )  |  ']'  |  element =  r_simple_type  )  ;

rarray_type :  (  (  '['  |  ']'  |  element =  r_simple_type  )  )  ;

rvariadic_type :  (  '...'  |  r_simple_type  )  ;

rpointer_type :  (  (  '&'  |  r_simple_type  )  )  ;

rmap_type :  (  'map['  |  key =  r_simple_type  |  ']'  |  value =  r_type  )  ;

rchannel_type :  (  (  'chan'  |  value =  r_simple_type  )  )  ;

rshared_type :  (  'shared'  |  r_simple_type  )  ;

rthread_type :  (  'thread'  |  r_simple_type  )  ;

rint_literal :  (  (  (  '0'  |  (  'b'  |  'B'  )  |  (  '_'  |  empty  )  |  (  '[01]'  |  (  (  (  '_'  |  empty  )  |  '[01]'  )  )*  )  )  |  (  '0'  |  (  '[1-9]'  |  (  (  (  '_'  |  empty  )  |  (  '[0-9]'  |  (  (  (  '_'  |  empty  )  |  '[0-9]'  )  )*  )  )  |  empty  )  )  )  |  (  '0'  |  (  (  'o'  |  'O'  )  |  empty  )  |  (  '_'  |  empty  )  |  (  '[0-7]'  |  (  (  (  '_'  |  empty  )  |  '[0-7]'  )  )*  )  )  |  (  '0'  |  (  'x'  |  'X'  )  |  (  '_'  |  empty  )  |  (  '[0-9a-fA-F]'  |  (  (  (  '_'  |  empty  )  |  '[0-9a-fA-F]'  )  )*  )  )  )  )  ;

rfloat_literal :  (  (  (  (  (  '[0-9]'  |  (  (  (  '_'  |  empty  )  |  '[0-9]'  )  )*  )  |  '.'  |  (  '[0-9]'  |  (  (  (  '_'  |  empty  )  |  '[0-9]'  )  )*  )  |  (  (  (  'e'  |  'E'  )  |  (  (  '+'  |  '-'  )  |  empty  )  |  (  '[0-9]'  |  (  (  (  '_'  |  empty  )  |  '[0-9]'  )  )*  )  )  |  empty  )  )  |  (  (  '[0-9]'  |  (  (  (  '_'  |  empty  )  |  '[0-9]'  )  )*  )  |  (  (  'e'  |  'E'  )  |  (  (  '+'  |  '-'  )  |  empty  )  |  (  '[0-9]'  |  (  (  (  '_'  |  empty  )  |  '[0-9]'  )  )*  )  )  )  |  (  '.'  |  (  '[0-9]'  |  (  (  (  '_'  |  empty  )  |  '[0-9]'  )  )*  )  |  (  (  (  'e'  |  'E'  )  |  (  (  '+'  |  '-'  )  |  empty  )  |  (  '[0-9]'  |  (  (  (  '_'  |  empty  )  |  '[0-9]'  )  )*  )  )  |  empty  )  )  )  |  (  '0'  |  (  'x'  |  'X'  )  |  (  (  (  '_'  |  empty  )  |  (  '[0-9a-fA-F]'  |  (  (  (  '_'  |  empty  )  |  '[0-9a-fA-F]'  )  )*  )  |  '.'  |  (  (  '[0-9a-fA-F]'  |  (  (  (  '_'  |  empty  )  |  '[0-9a-fA-F]'  )  )*  )  |  empty  )  )  |  (  (  '_'  |  empty  )  |  (  '[0-9a-fA-F]'  |  (  (  (  '_'  |  empty  )  |  '[0-9a-fA-F]'  )  )*  )  )  |  (  '.'  |  (  '[0-9a-fA-F]'  |  (  (  (  '_'  |  empty  )  |  '[0-9a-fA-F]'  )  )*  )  )  )  |  (  (  'p'  |  'P'  )  |  (  (  '+'  |  '-'  )  |  empty  )  |  (  '[0-9]'  |  (  (  (  '_'  |  empty  )  |  '[0-9]'  )  )*  )  )  )  )  )  ;

rrune_literal :  (  (  '`'  |  (  '[^\'\\\\]'  |  '\''  |  '\'  |  (  '\\'  |  (  '0'  |  '`'  |  (  'x'  |  '[0-9a-fA-F]'  |  '[0-9a-fA-F]'  )  |  (  '[0-7]'  |  '[0-7]'  |  '[0-7]'  )  |  (  'u'  |  '[0-9a-fA-F]'  |  '[0-9a-fA-F]'  |  '[0-9a-fA-F]'  |  '[0-9a-fA-F]'  )  |  (  'U'  |  '[0-9a-fA-F]'  |  '[0-9a-fA-F]'  |  '[0-9a-fA-F]'  |  '[0-9a-fA-F]'  |  '[0-9a-fA-F]'  |  '[0-9a-fA-F]'  |  '[0-9a-fA-F]'  |  '[0-9a-fA-F]'  )  |  (  (  'a'  |  'b'  |  'e'  |  'f'  |  'n'  |  'r'  |  't'  |  'v'  |  '\\'  |  '\''  |  '\'  )  )  )  )  )  |  '`'  )  )  ;

r_string_literal :  (  rc_string_literal  |  rraw_string_literal  |  rinterpreted_string_literal  )  ;

rc_string_literal :  (  (  r_c_string_opening  )  |  (  (  (  r_string_content  )  |  rescape_sequence  |  rstring_interpolation  |  ( /* no preceeding ws */  '$'  )  )  )*  |  r_string_closing  )  ;

rraw_string_literal :  (  (  r_raw_string_opening  )  |  (  (  (  r_string_content  )  )  )*  |  r_string_closing  )  ;

rinterpreted_string_literal :  (  (  r_string_opening  )  |  (  (  (  r_string_content  )  |  rescape_sequence  |  rstring_interpolation  |  ( /* no preceeding ws */  '$'  )  )  )*  |  r_string_closing  )  ;

rstring_interpolation :  (  (  r_braced_interpolation_opening  |  r_expression  |  (  rformat_specifier  |  empty  )  |  r_interpolation_closing  )  |  (  r_unbraced_interpolation_opening  |  (  ridentifier  |  rselector_expression  )  )  )  ;

rformat_specifier :  (  (  ':'  )  |  (  (  '[gGeEfFcdoxXpsSc]'  )  |  (  (  (  '[+-0]'  )  |  empty  )  |  rint_literal  |  (  (  '.'  |  rint_literal  )  |  empty  )  |  (  (  '[gGeEfFcdoxXpsSc]'  )  |  empty  )  )  )  )  ;

r_reserved_identifier :  (  (  'array'  |  'string'  |  'char'  |  'sql'  )  )  ;

ridentifier :  (  (  (  (  '[a-zα-ωµ]'  |  '_'  )  |  (  (  (  '[a-zA-Zα-ωΑ-Ωµ]'  |  '_'  )  |  '[0-9]'  )  )*  )  |  (  '@'  |  (  ( /* no preceeding ws */  'pub'  )  |  ( /* no preceeding ws */  'none'  )  |  ( /* no preceeding ws */  'const'  )  |  ( /* no preceeding ws */  'mut'  )  |  ( /* no preceeding ws */  '__global'  )  |  ( /* no preceeding ws */  'fn'  )  |  ( /* no preceeding ws */  'assert'  )  |  ( /* no preceeding ws */  'as'  )  |  ( /* no preceeding ws */  'go'  )  |  ( /* no preceeding ws */  'asm'  )  |  ( /* no preceeding ws */  'return'  )  |  ( /* no preceeding ws */  'type'  )  |  ( /* no preceeding ws */  'if'  )  |  ( /* no preceeding ws */  'else'  )  |  ( /* no preceeding ws */  'for'  )  |  ( /* no preceeding ws */  'in'  )  |  ( /* no preceeding ws */  'is'  )  |  ( /* no preceeding ws */  'union'  )  |  ( /* no preceeding ws */  'struct'  )  |  ( /* no preceeding ws */  'enum'  )  |  ( /* no preceeding ws */  'interface'  )  |  ( /* no preceeding ws */  'defer'  )  |  ( /* no preceeding ws */  'unsafe'  )  |  ( /* no preceeding ws */  'import'  )  |  ( /* no preceeding ws */  'match'  )  |  ( /* no preceeding ws */  'lock'  )  |  ( /* no preceeding ws */  'rlock'  )  |  ( /* no preceeding ws */  'select'  )  |  ( /* no preceeding ws */  'voidptr'  )  |  ( /* no preceeding ws */  'byteptr'  )  |  ( /* no preceeding ws */  'charptr'  )  |  ( /* no preceeding ws */  'i8'  )  |  ( /* no preceeding ws */  'i16'  )  |  ( /* no preceeding ws */  'i32'  )  |  ( /* no preceeding ws */  'int'  )  |  ( /* no preceeding ws */  'i64'  )  |  ( /* no preceeding ws */  'byte'  )  |  ( /* no preceeding ws */  'u8'  )  |  ( /* no preceeding ws */  'u16'  )  |  ( /* no preceeding ws */  'u32'  )  |  ( /* no preceeding ws */  'u64'  )  |  ( /* no preceeding ws */  'f32'  )  |  ( /* no preceeding ws */  'f64'  )  |  ( /* no preceeding ws */  'char'  )  |  ( /* no preceeding ws */  'bool'  )  |  ( /* no preceeding ws */  'string'  )  |  ( /* no preceeding ws */  'rune'  )  |  ( /* no preceeding ws */  'array'  )  |  ( /* no preceeding ws */  'map'  )  |  ( /* no preceeding ws */  'mapnode'  )  |  ( /* no preceeding ws */  'chan'  )  |  ( /* no preceeding ws */  'size_t'  )  |  ( /* no preceeding ws */  'usize'  )  |  ( /* no preceeding ws */  'isize'  )  |  ( /* no preceeding ws */  'float_literal'  )  |  ( /* no preceeding ws */  'int_literal'  )  |  ( /* no preceeding ws */  'thread'  )  |  ( /* no preceeding ws */  'IError'  )  )  )  )  )  ;

rimmediate_identifier :  ( /* no preceeding ws */  (  (  '[a-zα-ωµ]'  )  |  (  (  (  '[a-zA-Zα-ωΑ-Ωµ]'  |  '_'  )  |  '[0-9]'  |  '_'  )  )*  )  )  ;

r_old_identifier :  (  (  (  '[a-zA-Zα-ωΑ-Ωµ]'  |  '_'  )  |  (  (  (  '[a-zA-Zα-ωΑ-Ωµ]'  |  '_'  )  |  '[0-9]'  )  )*  )  )  ;

r_mutable_prefix :  (  (  (  'mut'  |  (  'static'  |  empty  )  )  |  'shared'  )  )  ;

rmutable_identifier :  (  (  r_mutable_prefix  |  (  ridentifier  |  r_reserved_identifier  )  )  )  ;

r_mutable_expression_2 :  (  (  r_mutable_prefix  |  (  rselector_expression  |  rindex_expression  )  )  )  ;

rmutable_expression :  (  (  r_mutable_prefix  |  r_expression  )  )  ;

rbinded_identifier :  (  language =  (  'C'  |  'JS'  )  |  ( /* no preceeding ws */  '.'  )  |  name =  (  ridentifier  |  (  r_old_identifier  )  )  )  ;

ridentifier_list :  (  (  (  rmutable_identifier  |  ridentifier  |  r_reserved_identifier  )  |  (  (  ','  |  (  rmutable_identifier  |  ridentifier  |  r_reserved_identifier  )  )  )*  )  )  ;

rexpression_list :  (  (  (  r_expression  |  rmutable_expression  )  |  (  (  ','  |  (  r_expression  |  rmutable_expression  )  )  )*  )  )  ;

r_expression_list_repeat1 :  (  (  r_expression  |  rmutable_expression  )  |  (  (  ','  |  (  r_expression  |  rmutable_expression  )  )  )+  )  ;

rparameter_declaration :  (  name =  (  rmutable_identifier  |  ridentifier  |  r_reserved_identifier  )  |  type =  (  r_simple_type  |  roption_type  |  rvariadic_type  )  )  ;

rparameter_list :  (  (  '('  |  (  (  rparameter_declaration  |  (  (  ','  |  rparameter_declaration  )  )*  )  |  empty  )  |  ')'  )  )  ;

rempty_literal_value :  (  (  '{'  |  '}'  )  )  ;

rargument_list :  (  '('  |  (  (  (  r_expression  |  rmutable_expression  |  rkeyed_element  |  rspread_operator  )  |  (  (  (  ','  |  r_automatic_separator  )  |  (  r_expression  |  rmutable_expression  |  rkeyed_element  |  rspread_operator  )  )  )*  |  (  r_automatic_separator  |  empty  )  )  |  empty  )  |  ')'  )  ;

r_type :  (  r_simple_type  |  roption_type  |  rmulti_return_type  )  ;

roption_type :  (  (  '?'  |  (  (  r_simple_type  |  rmulti_return_type  )  |  empty  )  )  )  ;

rmulti_return_type :  (  '('  |  (  r_simple_type  |  (  (  ','  |  r_simple_type  )  )*  )  |  ')'  )  ;

rtype_list :  (  r_simple_type  |  (  (  ','  |  r_simple_type  )  )*  )  ;

r_simple_type :  (  rbuiltin_type  |  rtype_identifier  |  rtype_placeholder  |  r_binded_type  |  rqualified_type  |  rpointer_type  |  rarray_type  |  rfixed_array_type  |  rfunction_type  |  rgeneric_type  |  rmap_type  |  rchannel_type  |  rshared_type  |  rthread_type  )  ;

rtype_parameters :  (  (  ( /* no preceeding ws */  '<'  )  |  (  r_simple_type  |  (  (  ','  |  r_simple_type  )  )*  )  |  ( /* no preceeding ws */  '>'  )  )  )  ;

rbuiltin_type :  (  (  'voidptr'  |  'byteptr'  |  'charptr'  |  'i8'  |  'i16'  |  'i32'  |  'int'  |  'i64'  |  'byte'  |  'u8'  |  'u16'  |  'u32'  |  'u64'  |  'f32'  |  'f64'  |  'char'  |  'bool'  |  'string'  |  'rune'  |  'array'  |  'map'  |  'mapnode'  |  'chan'  |  'size_t'  |  'usize'  |  'isize'  |  'float_literal'  |  'int_literal'  |  'thread'  |  'IError'  )  )  ;

r_binded_type :  (  (  rbinded_identifier  )  )  ;

rgeneric_type :  (  (  rqualified_type  |  rtype_identifier  )  |  rtype_parameters  )  ;

rqualified_type :  (  module =  r_module_identifier  |  '.'  |  name =  rtype_identifier  )  ;

rtype_placeholder :  (  '[A-ZΑ-Ω]'  )  ;

rpseudo_comptime_identifier :  (  '@'  |  (  '[A-Z][A-Z0-9_]+'  )  )  ;

rtype_identifier :  (  (  '[A-ZΑ-Ω]'  |  (  (  (  '[a-zA-Zα-ωΑ-Ωµ]'  |  '_'  )  |  '[0-9]'  )  )+  )  )  ;

r_module_identifier :  (  ridentifier  )  ;

r_field_identifier :  (  ridentifier  )  ;

r_statement_list :  (  (  r_statement  |  (  r_automatic_separator  |  empty  )  )  )+  ;

r_statement :  (  r_simple_statement  |  rassert_statement  |  rcontinue_statement  |  rbreak_statement  |  rreturn_statement  |  rasm_statement  |  rgo_statement  |  rgoto_statement  |  rlabeled_statement  |  rdefer_statement  |  rfor_statement  |  rcomptime_for_statement  |  rsend_statement  |  rblock  |  rhash_statement  )  ;

r_simple_statement :  (  r_expression  |  rinc_statement  |  rdec_statement  |  rassignment_statement  |  rshort_var_declaration  )  ;

rinc_statement :  (  r_expression  |  '++'  )  ;

rdec_statement :  (  r_expression  |  '--'  )  ;

rsend_statement :  (  (  channel =  r_expression  |  '<-'  |  value =  r_expression  )  )  ;

rshort_var_declaration :  (  (  left =  rexpression_list  |  ':='  |  right =  rexpression_list  )  )  ;

rassignment_statement :  (  left =  rexpression_list  |  operator =  (  '*='  |  '/='  |  '%='  |  '<<='  |  '>>='  |  '>>>='  |  '&='  |  '&^='  |  '+='  |  '-='  |  '|='  |  '^='  |  '='  )  |  right =  rexpression_list  )  ;

rassert_statement :  (  'assert'  |  r_expression  )  ;

rblock :  (  '{'  |  (  (  r_statement_list  |  (  r_expression_list_repeat1  )  |  (  rempty_labeled_statement  )  |  (  r_statement_list  |  (  (  r_expression_list_repeat1  )  |  (  rempty_labeled_statement  )  )  )  )  |  empty  )  |  '}'  )  ;

rdefer_statement :  (  'defer'  |  rblock  )  ;

runsafe_expression :  (  'unsafe'  |  rblock  )  ;

roverloadable_operator :  (  (  '+'  )  |  (  '-'  )  |  (  '*'  )  |  (  '/'  )  |  (  '%'  )  |  (  '<'  )  |  (  '>'  )  |  (  '=='  )  |  (  '!='  )  |  (  '<='  )  |  (  '>='  )  )  ;

rexposed_variables_list :  (  '['  |  rexpression_list  |  ']'  )  ;

rfunction_declaration :  (  (  attributes =  (  rattribute_list  |  empty  )  |  (  'pub'  |  empty  )  |  'fn'  |  receiver =  (  rparameter_list  |  empty  )  |  exposed_variables =  (  rexposed_variables_list  |  empty  )  |  name =  (  rbinded_identifier  |  ridentifier  |  roverloadable_operator  )  |  type_parameters =  (  rtype_parameters  |  empty  )  |  parameters =  (  rparameter_list  |  rtype_only_parameter_list  )  |  result =  (  r_type  |  empty  )  |  body =  (  rblock  |  empty  )  )  )  ;

rfunction_type :  (  (  'fn'  |  parameters =  (  rparameter_list  |  rtype_only_parameter_list  )  |  result =  (  r_type  |  empty  )  )  )  ;

rtype_only_parameter_list :  (  '('  |  (  (  rtype_parameter_declaration  |  (  (  ','  |  rtype_parameter_declaration  )  )*  )  |  empty  )  |  ')'  )  ;

rtype_parameter_declaration :  (  (  'mut'  |  empty  )  |  type =  (  r_simple_type  |  roption_type  |  rvariadic_type  )  )  ;

rfn_literal :  (  (  'fn'  |  exposed_variables =  (  rexposed_variables_list  |  empty  )  |  parameters =  rparameter_list  |  result =  (  r_type  |  empty  )  |  body =  rblock  |  arguments =  (  rargument_list  |  empty  )  )  )  ;

rglobal_var_declaration :  (  '__global'  |  (  r_global_var_spec  |  rglobal_var_type_initializer  |  (  '('  |  (  (  (  r_global_var_spec  |  rglobal_var_type_initializer  )  |  (  '\n'  |  '\r'  |  '\r\n'  )  )  )*  |  ')'  )  )  )  ;

r_global_var_spec :  (  rconst_spec  )  ;

rglobal_var_type_initializer :  (  name =  ridentifier  |  type =  r_type  )  ;

rconst_declaration :  (  (  'pub'  |  empty  )  |  'const'  |  (  rconst_spec  |  (  '('  |  (  (  rconst_spec  |  (  '\n'  |  '\r'  |  '\r\n'  )  )  )+  |  ')'  )  )  )  ;

rconst_spec :  (  name =  (  ridentifier  |  (  r_old_identifier  )  )  |  '='  |  value =  r_expression  )  ;

rasm_statement :  (  'asm'  |  ridentifier  |  r_content_block  )  ;

rsql_expression :  (  (  'sql'  |  (  ridentifier  |  empty  )  |  r_content_block  )  )  ;

r_content_block :  (  '{'  |  ( /* no preceeding ws */  (  '[^{}]+'  )  )  |  '}'  )  ;

rbreak_statement :  (  (  'break'  |  (  (  ridentifier  )  |  empty  )  )  )  ;

rcontinue_statement :  (  (  'continue'  |  (  (  ridentifier  )  |  empty  )  )  )  ;

rreturn_statement :  (  (  'return'  |  (  rexpression_list  |  empty  )  )  )  ;

rtype_declaration :  (  (  'pub'  |  empty  )  |  'type'  |  name =  (  rtype_identifier  |  rbuiltin_type  )  |  type_parameters =  (  rtype_parameters  |  empty  )  |  '='  |  types =  (  rsum_type_list  )  )  ;

rsum_type_list :  (  r_simple_type  |  (  (  '|'  |  r_simple_type  )  )*  )  ;

rgo_statement :  (  'go'  |  r_expression  )  ;

rgoto_statement :  (  'goto'  |  (  ridentifier  )  )  ;

rlabeled_statement :  (  label =  (  ridentifier  )  |  ':'  |  r_statement  )  ;

rempty_labeled_statement :  (  (  label =  (  ridentifier  )  |  ':'  )  )  ;

rfor_statement :  (  'for'  |  (  (  rfor_in_operator  |  rcstyle_for_clause  |  r_expression  )  |  empty  )  |  body =  rblock  )  ;

rcomptime_for_statement :  (  '$for'  |  rfor_in_operator  |  body =  rblock  )  ;

rfor_in_operator :  (  (  left =  (  r_expression  |  ridentifier_list  )  |  'in'  |  right =  (  (  r_definite_range  )  |  r_expression  )  )  )  ;

r_definite_range :  (  (  start =  r_expression  |  (  '..'  |  '...'  )  |  end =  r_expression  )  )  ;

r_range :  (  (  start =  (  r_expression  |  empty  )  |  '..'  |  end =  (  r_expression  |  empty  )  )  )  ;

rselector_expression :  (  (  operand =  (  r_expression  |  rcomptime_identifier  )  |  '.'  |  field =  (  ridentifier  |  rtype_identifier  |  (  rtype_placeholder  )  |  r_reserved_identifier  |  rcomptime_identifier  |  rcomptime_selector_expression  )  )  )  ;

rindex_expression :  (  (  operand =  r_expression  |  '['  |  index =  r_expression  |  ']'  |  (  roption_propagator  |  empty  )  )  )  ;

rslice_expression :  (  (  operand =  r_expression  |  '['  |  r_range  |  ']'  )  )  ;

rcstyle_for_clause :  (  (  initializer =  (  r_simple_statement  |  empty  )  |  ';'  |  condition =  (  r_expression  |  empty  )  |  ';'  |  update =  (  r_simple_statement  |  empty  )  )  )  ;

rcomptime_if_expression :  (  '$if'  |  condition =  (  r_expression  |  (  '?'  |  empty  )  )  |  consequence =  rblock  |  (  (  '$else'  |  alternative =  (  rblock  |  rcomptime_if_expression  )  )  |  empty  )  )  ;

rif_expression :  (  'if'  |  (  condition =  r_expression  |  initializer =  rshort_var_declaration  )  |  consequence =  rblock  |  (  (  'else'  |  alternative =  (  rblock  |  rif_expression  )  )  |  empty  )  )  ;

ris_expression :  (  (  left =  (  rtype_placeholder  |  rmutable_identifier  |  (  r_mutable_expression_2  )  |  rmutable_expression  |  r_expression  )  |  (  'is'  |  '!is'  )  |  right =  (  roption_type  |  r_simple_type  |  rnone  )  )  )  ;

rattribute_spec :  (  (  (  'if'  |  ridentifier  |  (  '?'  |  empty  )  )  |  (  'unsafe'  )  |  ridentifier  |  rinterpreted_string_literal  |  (  name =  (  (  'unsafe'  )  |  ridentifier  )  |  ':'  |  value =  (  r_string_literal  |  ridentifier  )  )  )  )  ;

rattribute_declaration :  (  '['  |  (  rattribute_spec  |  (  (  ';'  |  rattribute_spec  )  )*  )  |  ']'  )  ;

rattribute_list :  (  (  rattribute_declaration  |  (  (  '\n'  |  '\r'  |  '\r\n'  )  |  empty  )  )  )+  ;

rstruct_declaration :  (  attributes =  (  rattribute_list  |  empty  )  |  (  'pub'  |  empty  )  |  (  'struct'  |  'union'  )  |  name =  (  (  rtype_identifier  |  rbuiltin_type  |  rgeneric_type  )  )  |  rstruct_field_declaration_list  )  ;

rstruct_field_declaration_list :  (  '{'  |  (  (  (  rstruct_field_scope  |  rstruct_field_declaration  )  |  (  (  '\n'  |  '\r'  |  '\r\n'  )  |  empty  )  )  )*  |  '}'  )  ;

rstruct_field_scope :  (  (  'pub'  |  'mut'  |  (  'pub'  |  'mut'  )  |  '__global'  )  |  ( /* no preceeding ws */  ':'  )  )  ;

rstruct_field_declaration :  (  (  (  name =  r_field_identifier  |  type =  (  r_simple_type  |  roption_type  )  |  attributes =  (  rattribute_declaration  |  empty  )  |  (  (  '='  |  default_value =  r_expression  )  |  empty  )  |  (  (  '\n'  |  '\r'  |  '\r\n'  )  |  empty  )  )  |  type =  (  (  rtype_identifier  |  rqualified_type  )  |  (  (  '\n'  |  '\r'  |  '\r\n'  )  |  empty  )  )  )  )  ;

r_binded_struct_declaration :  (  attributes =  (  rattribute_list  |  empty  )  |  (  'pub'  |  empty  )  |  (  'struct'  |  'union'  )  |  name =  (  r_binded_type  )  |  (  r_binded_struct_field_declaration_list  )  )  ;

r_binded_struct_field_declaration_list :  (  '{'  |  (  (  (  rstruct_field_scope  |  (  r_binded_struct_field_declaration  )  )  |  (  (  '\n'  |  '\r'  |  '\r\n'  )  |  empty  )  )  )*  |  '}'  )  ;

r_binded_struct_field_declaration :  (  (  name =  (  r_field_identifier  |  (  r_old_identifier  )  )  |  type =  (  r_simple_type  |  roption_type  )  |  attributes =  (  rattribute_declaration  |  empty  )  |  (  (  '='  |  default_value =  r_expression  )  |  empty  )  |  (  (  '\n'  |  '\r'  |  '\r\n'  )  |  empty  )  )  )  ;

renum_declaration :  (  (  rattribute_list  |  empty  )  |  (  'pub'  |  empty  )  |  'enum'  |  name =  rtype_identifier  |  renum_member_declaration_list  )  ;

renum_member_declaration_list :  (  '{'  |  (  (  (  (  renum_member  |  (  (  '\n'  |  '\r'  |  '\r\n'  )  |  empty  )  )  )*  )  |  empty  )  |  '}'  )  ;

renum_member :  (  name =  ridentifier  |  (  (  '='  |  value =  r_expression  )  |  empty  )  )  ;

rtype_selector_expression :  (  type =  (  (  rtype_placeholder  |  rtype_identifier  )  |  empty  )  |  '.'  |  field_name =  (  r_reserved_identifier  |  ridentifier  )  )  ;

rinterface_declaration :  (  attributes =  (  rattribute_list  |  empty  )  |  (  'pub'  |  empty  )  |  'interface'  |  name =  (  rtype_identifier  |  rgeneric_type  )  |  rinterface_spec_list  )  ;

rinterface_spec_list :  (  '{'  |  (  (  (  (  rstruct_field_declaration  |  rinterface_spec  |  rinterface_field_scope  )  |  (  (  '\n'  |  '\r'  |  '\r\n'  )  |  empty  )  )  )*  |  empty  )  |  '}'  )  ;

rinterface_field_scope :  (  'mut'  |  ( /* no preceeding ws */  ':'  )  )  ;

rinterface_spec :  (  (  name =  r_field_identifier  |  parameters =  (  rparameter_list  |  rtype_only_parameter_list  )  |  result =  (  r_type  |  empty  )  )  )  ;

rhash_statement :  (  '#'  |  ( /* no preceeding ws */  (  '.|\\\\\\r?\\n'  )+  )  |  (  '\n'  |  '\r'  |  '\r\n'  )  )  ;

rmodule_clause :  (  attributes =  (  rattribute_list  |  empty  )  |  'module'  |  ' '  |  (  rimmediate_identifier  )  )  ;

rimport_declaration :  (  (  'import'  |  ' '  |  path =  rimport_path  |  (  (  ' '  |  (  alias =  rimport_alias  |  symbols =  rimport_symbols  )  )  |  empty  )  )  )  ;

rimport_path :  ( /* no preceeding ws */  (  (  '[a-zA-Zα-ωΑ-Ωµ]'  |  '_'  )  |  (  (  (  '[a-zA-Zα-ωΑ-Ωµ]'  |  '_'  )  |  '[0-9]'  |  '.'  )  )*  )  )  ;

rimport_symbols :  (  ( /* no preceeding ws */  '{'  )  |  rimport_symbols_list  |  '}'  )  ;

rimport_symbols_list :  (  (  ridentifier  |  (  rtype_identifier  )  )  |  (  (  ','  |  (  ridentifier  |  (  rtype_identifier  )  )  )  )*  )  ;

rimport_alias :  (  'as'  |  ' '  |  name =  (  rimmediate_identifier  )  )  ;

rmatch_expression :  (  'match'  |  condition =  (  r_expression  |  rmutable_expression  )  |  '{'  |  (  rexpression_case  )*  |  (  rdefault_case  |  empty  )  |  '}'  )  ;

rcase_list :  (  (  r_expression  |  r_simple_type  |  (  r_definite_range  )  )  |  (  (  ','  |  (  r_expression  |  r_simple_type  |  (  r_definite_range  )  )  )  )*  )  ;

rexpression_case :  (  value =  rcase_list  |  consequence =  rblock  )  ;

rdefault_case :  (  'else'  |  consequence =  rblock  )  ;

rselect_expression :  (  'select'  |  selected_variables =  (  rexpression_list  |  empty  )  |  '{'  |  (  rselect_branch  )*  |  (  rselect_default_branch  |  empty  )  |  '}'  )  ;

rselect_branch :  (  (  rshort_var_declaration  )  |  rblock  )  ;

rselect_default_branch :  (  (  (  (  (  '>'  |  empty  )  |  r_expression  )  )  |  'else'  )  |  rblock  )  ;

rlock_expression :  (  (  'lock'  |  'rlock'  )  |  locked_variables =  (  rexpression_list  |  empty  )  |  body =  rblock  )  ;

