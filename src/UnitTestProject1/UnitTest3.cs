using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Workspaces;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest3
    {
        [TestMethod]
        public void Rup1()
        {
            Document document = Setup.OpenAndParse(@"
grammar temp;
foo: (bar)?; // <- can be safely replaced with `bar?`
bar: 'baz';
");
            var result = LanguageServer.Transform.RemoveUselessParentheses(document);
            if (!(result.Count == 1 && result.First().Value == @"
grammar temp;
foo: bar?; // <- can be safely replaced with `bar?`
bar: 'baz';
"))
                throw new Exception();
        }

        [TestMethod]
        public void Rup2()
        {
            Document document = Setup.OpenAndParse(@"
grammar t2;
assignment : ( ( ( ( '=>' ) ) | ( ( '->' ) ) ) ? ( ( validID ) ) ( ( ( '+=' | '=' | '?=' ) ) ) ( ( assignableTerminal ) ) ) ;
// It should be assignment : ( '=>' | '->' ) ? validID ( '+=' | '=' | '?=' ) assignableTerminal ;
");
            var result = LanguageServer.Transform.RemoveUselessParentheses(document);
            if (!(result.Count == 1 && result.First().Value == @"
grammar t2;
assignment : ( '=>' | '->' ) ? validID ( '+=' | '=' | '?=' ) assignableTerminal ;
// It should be assignment : ( '=>' | '->' ) ? validID ( '+=' | '=' | '?=' ) assignableTerminal ;
"))
                throw new Exception();
        }

        [TestMethod]
        public void Rup3()
        {
            Document document = Setup.OpenAndParse(@"
grammar t3;
c : (a b)* | c;
");
            var result = LanguageServer.Transform.RemoveUselessParentheses(document);
            if (result.Count != 0)
                throw new Exception();
        }

        [TestMethod]
        public void Rup4()
        {
            Document document = Setup.OpenAndParse(@"
grammar t3;
a : ((a));
b : ((a b)) c;
c : (a b)* | c;
d : ((a b) | c);
");
            var result = LanguageServer.Transform.RemoveUselessParentheses(document);
            if (!(result.Count == 1 && result.First().Value == @"
grammar t3;
a : a;
b : a b c;
c : (a b)* | c;
d : a b | c;
"))
                throw new Exception();
        }

        [TestMethod]
        public void Rup5()
        {
            Document document = Setup.OpenAndParse(@"
grammar t1;
p : ( '+=' | '?=' ) ;
p : ( ( '+=' | '?=' ) ) ;
p : ( ( ( '+=' | '?=' ) ) ) ;
p : a ( '+=' | '?=' ) ;
p : a ( ( '+=' | '?=' ) ) ;
p : a ( ( ( '+=' | '?=' ) ) ) ;
a : a ('a') a;
a : a ('a')? a;
a : ('a' b);
a : ('a' b)?;
a : c ( 'a' b) d;
a : e ( 'a' | b) f;
a : ( (('=>')) | (('->')) )?;
d : ((a b) | c);
d : ((a b) | c)?;
d : x ((a b) | c) y;
assignment : ( ( ( ( '=>' ) ) | ( ( '->' ) ) ) ? ( ( validID ) ) ( ( ( '+=' | '=' | '?=' ) ) ) ( ( assignableTerminal ) ) ) ;
");
            var result = LanguageServer.Transform.RemoveUselessParentheses(document);
            if (!(result.Count == 1 && result.First().Value == @"
grammar t1;
p : '+=' | '?=' ;
p : '+=' | '?=' ;
p : '+=' | '?=' ;
p : a ( '+=' | '?=' ) ;
p : a ( '+=' | '?=' ) ;
p : a ( '+=' | '?=' ) ;
a : a 'a' a;
a : a 'a'? a;
a : 'a' b;
a : ('a' b)?;
a : c 'a' b d;
a : e ( 'a' | b) f;
a : ( '=>' | '->' )?;
d : a b | c;
d : (a b | c)?;
d : x (a b | c) y;
assignment : ( '=>' | '->' ) ? validID ( '+=' | '=' | '?=' ) assignableTerminal ;
"))
                throw new Exception();
        }
    }
}
