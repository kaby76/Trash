
( grammarSpec int:0..52
  ( grammarDecl int:0..3
    ( grammarType int:0..0
      ( GRAMMAR int:0..0 text:grammar tt:19 chnl:DEFAULT_TOKEN_CHANNEL text:grammar chnl:DEFAULT_TOKEN_CHANNEL l:1 c:0 si:0 ei:6 ti:0 tstext:grammar
    ) ) 
    ( identifier int:2..2
      ( TOKEN_REF int:2..2 text:Repeat tt:1 chnl:DEFAULT_TOKEN_CHANNEL text:Repeat chnl:DEFAULT_TOKEN_CHANNEL l:1 c:8 si:8 ei:13 ti:2 tstext:Repeat
    ) ) 
    ( SEMI int:3..3 text:; tt:32 chnl:DEFAULT_TOKEN_CHANNEL text:; chnl:DEFAULT_TOKEN_CHANNEL l:1 c:14 si:14 ei:14 ti:3 tstext:;
  ) ) 
  ( rules int:5..50
    ( ruleSpec int:5..22
      ( parserRuleSpec int:5..22
        ( RULE_REF int:5..5 text:file_ tt:2 chnl:DEFAULT_TOKEN_CHANNEL text:file_ chnl:DEFAULT_TOKEN_CHANNEL l:2 c:0 si:17 ei:21 ti:5 tstext:file_
        ) 
        ( COLON int:6..6 text:: tt:29 chnl:DEFAULT_TOKEN_CHANNEL text:: chnl:DEFAULT_TOKEN_CHANNEL l:2 c:5 si:22 ei:22 ti:6 tstext::
        ) 
        ( ruleBlock int:8..21
          ( ruleAltList int:8..21
            ( labeledAlt int:8..21
              ( alternative int:8..21
                ( element int:8..19
                  ( ebnf int:8..19
                    ( block int:8..18
                      ( LPAREN int:8..8 text:( tt:33 chnl:DEFAULT_TOKEN_CHANNEL text:( chnl:DEFAULT_TOKEN_CHANNEL l:2 c:7 si:24 ei:24 ti:8 tstext:(
                      ) 
                      ( altList int:13..13
                        ( alternative int:2147483647..-1
                          ( element int:2147483647..-1
                            ( atom int:2147483647..-1
                              ( ruleref int:2147483647..-1
                        ) ) ) ) 
                        ( OR int:13..13 text:| tt:45 chnl:DEFAULT_TOKEN_CHANNEL text:| chnl:DEFAULT_TOKEN_CHANNEL l:2 c:19 si:36 ei:36 ti:13 tstext:|
                        ) 
                        ( alternative int:2147483647..-1
                          ( element int:2147483647..-1
                            ( atom int:2147483647..-1
                              ( ruleref int:2147483647..-1
                      ) ) ) ) ) 
                      ( RPAREN int:18..18 text:) tt:34 chnl:DEFAULT_TOKEN_CHANNEL text:) chnl:DEFAULT_TOKEN_CHANNEL l:2 c:31 si:48 ei:48 ti:18 tstext:)
                    ) ) 
                    ( blockSuffix int:19..19
                      ( ebnfSuffix int:19..19
                        ( STAR int:19..19 text:* tt:42 chnl:DEFAULT_TOKEN_CHANNEL text:* chnl:DEFAULT_TOKEN_CHANNEL l:2 c:32 si:49 ei:49 ti:19 tstext:*
                ) ) ) ) ) 
                ( element int:21..21
                  ( atom int:21..21
                    ( terminal int:21..21
                      ( TOKEN_REF int:21..21 text:EOF tt:1 chnl:DEFAULT_TOKEN_CHANNEL text:EOF chnl:DEFAULT_TOKEN_CHANNEL l:2 c:34 si:51 ei:53 ti:21 tstext:EOF
        ) ) ) ) ) ) ) ) 
        ( SEMI int:22..22 text:; tt:32 chnl:DEFAULT_TOKEN_CHANNEL text:; chnl:DEFAULT_TOKEN_CHANNEL l:2 c:37 si:54 ei:54 ti:22 tstext:;
        ) 
        ( exceptionGroup int:2147483647..-1
    ) ) ) 
    ( ruleSpec int:24..29
      ( parserRuleSpec int:24..29
        ( RULE_REF int:24..24 text:a tt:2 chnl:DEFAULT_TOKEN_CHANNEL text:a chnl:DEFAULT_TOKEN_CHANNEL l:3 c:0 si:57 ei:57 ti:24 tstext:a
        ) 
        ( COLON int:26..26 text:: tt:29 chnl:DEFAULT_TOKEN_CHANNEL text:: chnl:DEFAULT_TOKEN_CHANNEL l:3 c:2 si:59 ei:59 ti:26 tstext::
        ) 
        ( ruleBlock int:28..28
          ( ruleAltList int:28..28
            ( labeledAlt int:28..28
              ( alternative int:28..28
                ( element int:28..28
                  ( atom int:28..28
                    ( terminal int:28..28
                      ( STRING_LITERAL int:28..28 text:'a' tt:8 chnl:DEFAULT_TOKEN_CHANNEL text:'a' chnl:DEFAULT_TOKEN_CHANNEL l:3 c:4 si:61 ei:63 ti:28 tstext:'a'
        ) ) ) ) ) ) ) ) 
        ( SEMI int:29..29 text:; tt:32 chnl:DEFAULT_TOKEN_CHANNEL text:; chnl:DEFAULT_TOKEN_CHANNEL l:3 c:7 si:64 ei:64 ti:29 tstext:;
        ) 
        ( exceptionGroup int:2147483647..-1
    ) ) ) 
    ( ruleSpec int:31..36
      ( parserRuleSpec int:31..36
        ( RULE_REF int:31..31 text:b tt:2 chnl:DEFAULT_TOKEN_CHANNEL text:b chnl:DEFAULT_TOKEN_CHANNEL l:4 c:0 si:67 ei:67 ti:31 tstext:b
        ) 
        ( COLON int:33..33 text:: tt:29 chnl:DEFAULT_TOKEN_CHANNEL text:: chnl:DEFAULT_TOKEN_CHANNEL l:4 c:2 si:69 ei:69 ti:33 tstext::
        ) 
        ( ruleBlock int:35..35
          ( ruleAltList int:35..35
            ( labeledAlt int:35..35
              ( alternative int:35..35
                ( element int:35..35
                  ( atom int:35..35
                    ( terminal int:35..35
                      ( STRING_LITERAL int:35..35 text:'b' tt:8 chnl:DEFAULT_TOKEN_CHANNEL text:'b' chnl:DEFAULT_TOKEN_CHANNEL l:4 c:4 si:71 ei:73 ti:35 tstext:'b'
        ) ) ) ) ) ) ) ) 
        ( SEMI int:36..36 text:; tt:32 chnl:DEFAULT_TOKEN_CHANNEL text:; chnl:DEFAULT_TOKEN_CHANNEL l:4 c:7 si:74 ei:74 ti:36 tstext:;
        ) 
        ( exceptionGroup int:2147483647..-1
    ) ) ) 
    ( ruleSpec int:38..50
      ( lexerRuleSpec int:38..50
        ( TOKEN_REF int:38..38 text:WS tt:1 chnl:DEFAULT_TOKEN_CHANNEL text:WS chnl:DEFAULT_TOKEN_CHANNEL l:5 c:0 si:77 ei:78 ti:38 tstext:WS
        ) 
        ( COLON int:39..39 text:: tt:29 chnl:DEFAULT_TOKEN_CHANNEL text:: chnl:DEFAULT_TOKEN_CHANNEL l:5 c:2 si:79 ei:79 ti:39 tstext::
        ) 
        ( lexerRuleBlock int:41..49
          ( lexerAltList int:41..49
            ( lexerAlt int:41..49
              ( lexerElements int:41..42
                ( lexerElement int:41..42
                  ( lexerAtom int:41..41
                    ( LEXER_CHAR_SET int:41..41 text:[ \\t\\n\\r] tt:3 chnl:DEFAULT_TOKEN_CHANNEL text:[ \\t\\n\\r] chnl:DEFAULT_TOKEN_CHANNEL l:5 c:4 si:81 ei:89 ti:41 tstext:[ \\t\\n\\r]
                  ) ) 
                  ( ebnfSuffix int:42..42
                    ( PLUS int:42..42 text:+ tt:44 chnl:DEFAULT_TOKEN_CHANNEL text:+ chnl:DEFAULT_TOKEN_CHANNEL l:5 c:13 si:90 ei:90 ti:42 tstext:+
              ) ) ) ) 
              ( lexerCommands int:44..49
                ( RARROW int:44..44 text:-> tt:37 chnl:DEFAULT_TOKEN_CHANNEL text:-> chnl:DEFAULT_TOKEN_CHANNEL l:5 c:15 si:92 ei:93 ti:44 tstext:->
                ) 
                ( lexerCommand int:46..49
                  ( lexerCommandName int:46..46
                    ( identifier int:46..46
                      ( RULE_REF int:46..46 text:channel tt:2 chnl:DEFAULT_TOKEN_CHANNEL text:channel chnl:DEFAULT_TOKEN_CHANNEL l:5 c:18 si:95 ei:101 ti:46 tstext:channel
                  ) ) ) 
                  ( LPAREN int:47..47 text:( tt:33 chnl:DEFAULT_TOKEN_CHANNEL text:( chnl:DEFAULT_TOKEN_CHANNEL l:5 c:25 si:102 ei:102 ti:47 tstext:(
                  ) 
                  ( lexerCommandExpr int:48..48
                    ( identifier int:48..48
                      ( TOKEN_REF int:48..48 text:HIDDEN tt:1 chnl:DEFAULT_TOKEN_CHANNEL text:HIDDEN chnl:DEFAULT_TOKEN_CHANNEL l:5 c:26 si:103 ei:108 ti:48 tstext:HIDDEN
                  ) ) ) 
                  ( RPAREN int:49..49 text:) tt:34 chnl:DEFAULT_TOKEN_CHANNEL text:) chnl:DEFAULT_TOKEN_CHANNEL l:5 c:32 si:109 ei:109 ti:49 tstext:)
        ) ) ) ) ) ) 
        ( SEMI int:50..50 text:; tt:32 chnl:DEFAULT_TOKEN_CHANNEL text:; chnl:DEFAULT_TOKEN_CHANNEL l:5 c:33 si:110 ei:110 ti:50 tstext:;
  ) ) ) ) 
  ( EOF int:52..52 text: tt:-1 chnl:DEFAULT_TOKEN_CHANNEL text: chnl:DEFAULT_TOKEN_CHANNEL l:6 c:0 si:113 ei:112 ti:52 tstext:
) ) 


