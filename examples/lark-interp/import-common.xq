(: Step 5: Inline the imported common.lark rules into verilog.lark and
   remove all %import statements.
   Input: transformed.pt (output of lark-to-antlr4.xq on the two-file o.pt).
   Uses doc("transformed.pt") for cross-document rule lookup. :)

(: Step 5a: Insert each needed common rule (already Antlr4-transformed) before
   the first %import item in verilog.lark. The 'where' clause restricts this
   to the verilog.lark document context (common.lark has no %import statements). :)
(for $t in doc("transformed.pt")//*[name()="token"]
           [./TOKEN = ("CNAME", "ESCAPED_STRING", "WS", "LETTER",
                       "LCASE_LETTER", "UCASE_LETTER", "DIGIT",
                       "_STRING_INNER", "_STRING_ESC_INNER")]/..
 where exists(//item[statement[IMPORT]])
 return insert node ($t, "
") before (//item[statement[IMPORT]])[1]),

(: Step 5b: Delete all %import statement items from the current document. :)
(delete node //item[statement[IMPORT]])
