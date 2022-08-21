using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Workspaces;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void Kleene1()
        {
            var document = Setup.OpenAndParse(@"
grammar kleene;
a : | 'a' a;
");
            var result = LanguageServer.Transform.ConvertRecursionToKleeneOperator(document);
            if (!(result.Count == 1 && result.First().Value == @"
grammar kleene;
a : 'a' * ;
")) throw new Exception();
        }

        [TestMethod]
        public void Kleene2()
        {
            Document document = Setup.OpenAndParse(@"
grammar kleene;
b : | b 'b';
");
            var result = LanguageServer.Transform.ConvertRecursionToKleeneOperator(document);
            if (!(result.Count == 1 && result.First().Value == @"
grammar kleene;
b : 'b' * ;
")) throw new Exception();
        }

        [TestMethod]
        public void Kleene3()
        {
            var document = Setup.OpenAndParse(@"
grammar kleene;
xx  : xx yy | ;
yy: 'b' ;
");
            var result = LanguageServer.Transform.ConvertRecursionToKleeneOperator(document);
            if (!(result.Count == 1 && result.First().Value == @"
grammar kleene;
xx : yy * ;
yy: 'b' ;
")) throw new Exception();
        }

        [TestMethod]
        public void Kleene4()
        {
            var document = Setup.OpenAndParse(@"
grammar kleene;
xx : 'a' xx | 'a';
yy : yy 'b' | 'b' ;
zz : | 'a' | 'a' zz;
z2 : | 'b' | z2 'b';
");
            var result = LanguageServer.Transform.ConvertRecursionToKleeneOperator(document);
            if (!(result.Count == 1 && result.First().Value == @"
grammar kleene;
xx : 'a' + ;
yy : 'b' + ;
zz : 'a' * ;
z2 : 'b' * ;
")) throw new Exception();
        }
    }
}
