#!/usr/bin/bash
echo $1
trparse Problems.g4 \
   | trxgrep '
      for $i in (
         //parserRuleSpec//TOKEN_REF
            [text()=doc("*")//lexerRuleSpec
               [./lexerRuleBlock/lexerAltList/lexerAlt/lexerCommands/lexerCommand/lexerCommandName/identifier/RULE_REF/text()="skip"]
               /TOKEN_REF/text()])
         return concat("line ", $i/@Line, " col ", $i/@Column, " """, $i/@Text,"""")'
