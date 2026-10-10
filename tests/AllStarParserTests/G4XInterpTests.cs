using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using EditableAntlrTree;
using ParseTreeEditing.UnvParseTreeDOM;
using trinterp;
using Xunit;

namespace AllStarParserTests;

public sealed class G4XInterpTests
{
    [Theory]
    [InlineData(".g4x")]
    public async Task CliBundlePipelineGeneratesRunnableTables(string extension)
    {
        var directory = Directory.CreateTempSubdirectory("G4XPipeline-").FullName;
        try
        {
            var grammar = Path.Combine(directory, "C" + extension);
            await File.WriteAllTextAsync(grammar, "grammar C; Other:'y'; Start:'x' EOF;");
            var parsed = await RunCli(typeof(Trash.Program).Assembly.Location, null, grammar);
            Assert.True(parsed.Exit == 0, parsed.Error);
            var generated = await RunCli(typeof(trinterp.Program).Assembly.Location, parsed.Output, "-o", directory);
            Assert.True(generated.Exit == 0, generated.Error);
            var (result, _) = AllStarAtnParser.InterpRunner.Run(Path.Combine(directory, "C.interp"),
                Path.Combine(directory, "CLexer.interp"), "x", "input", false);
            Assert.Equal("(Start x <EOF>)", new TreeOutput(result.Lexer, result.Parser)
                .OutputTreeAntlrStyle(Assert.Single(result.Nodes)).ToString());
            Assert.True(File.Exists(Path.Combine(directory, "C.tokens")));
        }
        finally { Directory.Delete(directory, true); }
    }

    [Fact]
    public async Task CliSetDifferenceCompilesAndSelectsKeywordAndLiteralFallbacks()
    {
        var directory = Directory.CreateTempSubdirectory("G4XDifference-").FullName;
        try
        {
            var example = Path.Combine(AppContext.BaseDirectory, "TestData", "g4x-set-difference");
            var parsed = await RunCli(typeof(Trash.Program).Assembly.Location, null,
                Path.Combine(example, "SetDiffLexer.g4x"),
                Path.Combine(example, "SetDiffParser.g4x"));
            Assert.True(parsed.Exit == 0, parsed.Error);
            var generated = await RunCli(typeof(trinterp.Program).Assembly.Location, parsed.Output, "-o", directory);
            Assert.True(generated.Exit == 0, generated.Error);
            var lexerPath = Path.Combine(directory, "SetDiffLexer.interp");
            var lexerData = Atn.InterpFileReader.Read(File.ReadAllText(lexerPath));
            Assert.Single(lexerData.G4XExclusions);
            var lexerAtn = Atn.AtnDeserializer.Deserialize(lexerData.AtnData);
            lexerAtn.G4XExclusions = lexerData.G4XExclusions;
            var input = await File.ReadAllTextAsync(Path.Combine(example, "input.txt"));
            var tokens = new EarleyAtnParser.LexerAtnSimulator(lexerAtn).Tokenize(input);
            Assert.Equal(new[] { "Keyword", "Identifier", "NullLiteral", "Keyword",
                "Identifier", "Identifier", "Identifier" },
                tokens.Where(t => t.Type > 0 && t.Channel == 0)
                    .Select(t => lexerData.SymbolicNames[t.Type]));
            var (result, _) = AllStarAtnParser.InterpRunner.Run(
                Path.Combine(directory, "SetDiffParser.interp"), lexerPath,
                input, "input.txt", false);
            Assert.Single(result.Nodes);
        }
        finally { Directory.Delete(directory, true); }
    }

    [Fact]
    public async Task G4XOptionSelectsContextAwareAllStarWithoutCliFlag()
    {
        var directory = Directory.CreateTempSubdirectory("G4XContextOption-").FullName;
        try
        {
            var lexer = Path.Combine(directory, "L.g4x");
            var parser = Path.Combine(directory, "P.g4x");
            var input = Path.Combine(directory, "input.txt");
            await File.WriteAllTextAsync(lexer,
                "lexer grammar L; KEY:'key'; WORD:[a-z]+;");
            await File.WriteAllTextAsync(parser,
                "parser grammar P; options{tokenVocab=L; contextAwareLexing=true;} Start:WORD EOF;");
            await File.WriteAllTextAsync(input, "key");
            var parsed = await RunCli(typeof(Trash.Program).Assembly.Location,
                null, lexer, parser);
            Assert.True(parsed.Exit == 0, parsed.Error);
            var generated = await RunCli(typeof(trinterp.Program).Assembly.Location,
                parsed.Output, "-o", directory);
            Assert.True(generated.Exit == 0, generated.Error);
            var parserInterp = Path.Combine(directory, "P.interp");
            Assert.True(Atn.InterpFileReader.Read(File.ReadAllText(parserInterp))
                .ContextAwareLexing);
            Assert.True(Atn.InterpFileReader.RequiresContextAwareLexing(parserInterp));
            var (direct, _) = AllStarAtnParser.InterpRunner.Run(
                parserInterp, Path.Combine(directory, "L.interp"),
                "key", "input.txt", false);
            Assert.Single(direct.Nodes);
            var run = await RunCli(typeof(Trash.Program).Assembly.Location,
                null, "-L", directory, "--pinterp", "P.interp",
                "--linterp", "L.interp", "--no-output", "--per-file", input);
            Assert.True(run.Exit == 0, run.Error);
            Assert.Contains("ALL(*)", run.Error);
        }
        finally { Directory.Delete(directory, true); }
    }

    [Fact]
    public async Task SetDifferenceRejectsCompleteMatchButAllowsShorterMatches()
    {
        var directory = Directory.CreateTempSubdirectory("G4XNoFallback-").FullName;
        try
        {
            var grammar = Path.Combine(directory, "L.g4x");
            await File.WriteAllTextAsync(grammar, "lexer grammar L; word:[a-z]+ - 'if';");
            var parsed = await RunCli(typeof(Trash.Program).Assembly.Location, null, grammar);
            Assert.True(parsed.Exit == 0, parsed.Error);
            var generated = await RunCli(typeof(trinterp.Program).Assembly.Location,
                parsed.Output, "-o", directory);
            Assert.True(generated.Exit == 0, generated.Error);
            var data = Atn.InterpFileReader.Read(File.ReadAllText(Path.Combine(directory, "L.interp")));
            var atn = Atn.AtnDeserializer.Deserialize(data.AtnData);
            atn.G4XExclusions = data.G4XExclusions;
            var lexer = new EarleyAtnParser.LexerAtnSimulator(atn);
            Assert.Equal(data.SymbolicNames.ToList().IndexOf("word"), lexer.Tokenize("iffy")[0].Type);
            // Excluding the complete match does not exclude shorter accepted lexemes.
            Assert.Equal(new[] { "i", "f" }, lexer.Tokenize("if")
                .Where(t => t.Type > 0).Select(t => t.Text));
        }
        finally { Directory.Delete(directory, true); }
    }

    [Fact]
    public async Task IndirectParserWithoutContextAwareLexingEnforcesExclusion()
    {
        var directory = Directory.CreateTempSubdirectory("G4XIndirectExclusion-").FullName;
        try
        {
            var lexer = Path.Combine(directory, "L.g4x");
            var parser = Path.Combine(directory, "P.g4x");
            await File.WriteAllTextAsync(lexer,
                "lexer grammar L; WORD: [a-z]+;");
            await File.WriteAllTextAsync(parser,
                "parser grammar P; options {tokenVocab=L;} Start: word EOF; word: WORD - 'key';");
            var parsed = await RunCli(typeof(Trash.Program).Assembly.Location,
                null, lexer, parser);
            Assert.True(parsed.Exit == 0, parsed.Error);
            var generated = await RunCli(typeof(trinterp.Program).Assembly.Location,
                parsed.Output, "-o", directory);
            Assert.True(generated.Exit == 0, generated.Error);
            var parserInterp = Path.Combine(directory, "P.interp");
            var lexerInterp = Path.Combine(directory, "L.interp");
            Assert.Single(AllStarAtnParser.InterpRunner.Run(parserInterp,
                lexerInterp, "other", "input.txt", false,
                indirectLeftRecursion: true).Result.Nodes);
            Assert.Throws<InvalidOperationException>(() =>
                AllStarAtnParser.InterpRunner.Run(parserInterp, lexerInterp,
                    "key", "input.txt", false, indirectLeftRecursion: true));
        }
        finally { Directory.Delete(directory, true); }
    }

    [Theory]
    [InlineData("😀")]
    [InlineData("\\uD83D\\uDE00")]
    public async Task SetDifferenceDecodesSupplementaryLiteral(string excluded)
    {
        var directory = Directory.CreateTempSubdirectory("G4XEmojiExclusion-").FullName;
        try
        {
            var grammar = Path.Combine(directory, "L.g4x");
            await File.WriteAllTextAsync(grammar,
                $"lexer grammar L; WORD: [a-z]+ - '{excluded}';");
            var parsed = await RunCli(typeof(Trash.Program).Assembly.Location,
                null, grammar);
            Assert.True(parsed.Exit == 0, parsed.Error);
            var generated = await RunCli(typeof(trinterp.Program).Assembly.Location,
                parsed.Output, "-o", directory);
            Assert.True(generated.Exit == 0, generated.Error);
            var data = Atn.InterpFileReader.Read(File.ReadAllText(
                Path.Combine(directory, "L.interp")));
            var operand = Assert.Single(Assert.Single(data.G4XExclusions).Value);
            Assert.Equal("😀", operand.Value);
        }
        finally { Directory.Delete(directory, true); }
    }

    [Fact(Timeout = 30000)]
    public async Task GrammarSelectedContextAwareTimeoutUsesAllStarStatus()
    {
        var directory = Directory.CreateTempSubdirectory("G4XContextTimeout-").FullName;
        try
        {
            var lexer = Path.Combine(directory, "DemoLexer.g4x");
            var parser = Path.Combine(directory, "DemoParser.g4x");
            var input = Path.Combine(directory, "input.txt");
            await File.WriteAllTextAsync(lexer,
                "lexer grammar DemoLexer; WORD: [a-z]+; WS: [ \\t\\r\\n]+ -> skip;");
            await File.WriteAllTextAsync(parser,
                "parser grammar DemoParser; options {tokenVocab=DemoLexer; contextAwareLexing=true;} Start: WORD+ EOF;");
            await File.WriteAllTextAsync(input,
                string.Concat(Enumerable.Repeat("word ", 500_000)));
            var parsed = await RunCli(typeof(Trash.Program).Assembly.Location,
                null, lexer, parser);
            Assert.True(parsed.Exit == 0, parsed.Error);
            var generated = await RunCli(typeof(trinterp.Program).Assembly.Location,
                parsed.Output, "-o", directory);
            Assert.True(generated.Exit == 0, generated.Error);
            var run = await RunCli(typeof(Trash.Program).Assembly.Location,
                null, "-L", directory, "--timeout", "1", "--no-output",
                "--per-file", input);
            Assert.NotEqual(0, run.Exit);
            Assert.Contains($"ALL(*) 0 {input} failed", run.Error);
            Assert.Contains("timed out after 1 seconds", run.Error);
        }
        finally { Directory.Delete(directory, true); }
    }

    private static async Task<(int Exit, byte[] Output, string Error)> RunCli(string assembly, byte[]? input, params string[] args)
    {
        var start = new System.Diagnostics.ProcessStartInfo("dotnet")
        {
            RedirectStandardInput = true, RedirectStandardOutput = true,
            RedirectStandardError = true, UseShellExecute = false
        };
        start.ArgumentList.Add(assembly);
        foreach (var arg in args) start.ArgumentList.Add(arg);
        using var process = System.Diagnostics.Process.Start(start)!;
        using var output = new MemoryStream();
        var read = process.StandardOutput.BaseStream.CopyToAsync(output);
        var error = process.StandardError.ReadToEndAsync();
        try
        {
            if (input != null) await process.StandardInput.BaseStream.WriteAsync(input);
            process.StandardInput.Close();
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30));
            await read;
            return (process.ExitCode, output.ToArray(), await error);
        }
        finally { if (!process.HasExited) process.Kill(entireProcessTree: true); }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("Start", true)]
    [InlineData("Missing", false)]
    public async Task CliStartRuleSelectionDiagnosesAmbiguity(string? startRule, bool success)
    {
        var directory = Directory.CreateTempSubdirectory("G4XStart-").FullName;
        try
        {
            var grammar = Path.Combine(directory, "C.g4x");
            await File.WriteAllTextAsync(grammar, "grammar C; Other:'y' EOF; Start:'x' EOF;");
            var parsed = await RunCli(typeof(Trash.Program).Assembly.Location, null, grammar);
            Assert.True(parsed.Exit == 0, parsed.Error);
            var args = new List<string> { "-o", directory };
            if (startRule != null) args.AddRange(new[] { "--start-rule", startRule });
            var generated = await RunCli(typeof(trinterp.Program).Assembly.Location, parsed.Output, args.ToArray());
            Assert.Equal(success, generated.Exit == 0);
            if (!success)
            {
                Assert.Contains(startRule == null ? "Multiple EOF-terminated" : "not found", generated.Error);
                Assert.Empty(Directory.GetFiles(directory, "*.interp"));
            }
            else
            {
                var (result, _) = AllStarAtnParser.InterpRunner.Run(Path.Combine(directory, "C.interp"),
                    Path.Combine(directory, "CLexer.interp"), "x", "input", false);
                Assert.Equal("(Start x <EOF>)", new TreeOutput(result.Lexer, result.Parser)
                    .OutputTreeAntlrStyle(Assert.Single(result.Nodes)).ToString());
            }
        }
        finally { Directory.Delete(directory, true); }
    }

    private static GrammarModel Model(string source, bool plus = true)
    {
        Lexer lexer = plus ? new G4XLexer(new AntlrInputStream(source)) : new ANTLRv4Lexer(new AntlrInputStream(source));
        var tokens = new CommonTokenStream(lexer);
        Parser parser = plus ? new G4XParser(tokens) : new ANTLRv4Parser(tokens);
        IParseTree tree = plus ? ((G4XParser)parser).grammarSpec() : ((ANTLRv4Parser)parser).grammarSpec();
        Assert.Equal(0, parser.NumberOfSyntaxErrors);
        var dom = new ConvertToDOM().BottomUpConvert(tree, null, parser, lexer, tokens);
        return new GrammarParser().Parse(dom, plus ? "test.g4x" : "test.g4");
    }

    private static List<GrammarModel> Batch(params GrammarModel[] grammars)
    {
        var models = grammars.SelectMany(m => m.ImplicitLexer == null ? new[] { m } : new[] { m, m.ImplicitLexer }).ToList();
        GrammarBinding.Bind(models);
        return models;
    }

    private static string Tables(GrammarModel model)
    {
        ParserAtnFactory factory = model.IsLexer ? new LexerAtnFactory(model) : new ParserAtnFactory(model);
        var atn = factory.CreateATN();
        var text = InterpFormatter.FormatInterp(model, atn, false);
        if (!model.IsLexer)
        {
            var name = Assert.Single(trinterp.Command.FindEofTerminatedRules(model));
            text += "\nstart-rule:\n" + atn.ruleToStartState[model.GetRule(name).Index].stateNumber + "\n";
        }
        return text;
    }

    private static string Parse(List<GrammarModel> models, string parserName, string lexerName,
        string input, bool indirect = false, bool contextAware = false)
    {
        var directory = Directory.CreateTempSubdirectory("G4XInterp-").FullName;
        try
        {
            foreach (var model in models) File.WriteAllText(Path.Combine(directory, model.Name + ".interp"), Tables(model));
            var (result, _) = AllStarAtnParser.InterpRunner.Run(
                Path.Combine(directory, parserName + ".interp"), Path.Combine(directory, lexerName + ".interp"),
                input, "input.txt", false, contextAwareLexing: contextAware,
                indirectLeftRecursion: indirect);
            return new TreeOutput(result.Lexer, result.Parser).OutputTreeAntlrStyle(Assert.Single(result.Nodes)).ToString();
        }
        finally { Directory.Delete(directory, true); }
    }

    [Fact]
    public void CombinedGrammarKeepsUppercaseRulesAndBuildsLiteralLexer()
    {
        var model = Model("grammar C; Start : Expr EOF; Expr : Expr '+' Atom | Atom; Atom : 'x';");
        Assert.Equal(new[] { "Start", "Expr", "Atom" }, model.Rules.Select(r => r.Name));
        var batch = Batch(model);
        Assert.Equal("(Start (Expr (Expr (Atom x)) + (Atom x)) <EOF>)", Parse(batch, "C", "CLexer", "x+x"));
        Assert.Throws<InvalidOperationException>(() => Parse(batch, "C", "CLexer", "x+"));
    }

    [Fact]
    public void IndirectRecursionRetainsGrammarRuleNesting()
    {
        var batch = Batch(Model("grammar C; Start:Expr EOF; Expr:Addition | 'x'; Addition:Expr '+' 'x';"));
        Assert.Equal("(Start (Expr (Addition (Expr x) + x)) <EOF>)", Parse(batch, "C", "CLexer", "x+x", indirect: true));
        Assert.Contains("Indirect left recursion", Assert.Throws<InvalidOperationException>(() => Parse(batch, "C", "CLexer", "x+x")).Message);
    }

    [Fact]
    public void SeparateGrammarResolvesLowercaseTokensAndFragments()
    {
        var lexer = Model("lexer grammar L; fragment digit : [0-9]; number : digit+; space : [ \\t]+ -> skip;");
        var parser = Model("parser grammar P; options { tokenVocab=L; } Start : number (',' number)* EOF;");
        // A parser literal must have a corresponding token in its lexer.
        Assert.Throws<InvalidOperationException>(() => Batch(parser, lexer));
        lexer = Model("lexer grammar L; fragment digit : [0-9]; number : digit+; comma : ','; space : [ \\t]+ -> skip;");
        Assert.Equal("(Start 12 , 34 <EOF>)", Parse(Batch(parser, lexer), "P", "L", "12, 34"));
    }

    [Fact]
    public void ParserSetDifferenceRejectsContextuallyLexedKeyword()
    {
        const string lexer = "lexer grammar L; PERMITS:'permits'; Identifier:[a-z]+; WS:[ \\t]+ -> skip;";
        const string parser = "parser grammar P; options{tokenVocab=L;} " +
            "start:typeIdentifier EOF; typeIdentifier:identifier - ('permits' | 'record'); " +
            "identifier:Identifier;";
        List<GrammarModel> BatchFresh() => Batch(Model(parser), Model(lexer));
        var withoutExclusion = parser.Replace(
            "identifier - ('permits' | 'record')", "identifier");
        Assert.Equal("(start (typeIdentifier (identifier permits)) <EOF>)",
            Parse(Batch(Model(withoutExclusion), Model(lexer)), "P", "L",
                "permits", contextAware: true));
        Assert.Equal("(start (typeIdentifier (identifier normal)) <EOF>)",
            Parse(BatchFresh(), "P", "L", "normal", contextAware: true));
        Assert.Equal("(start (typeIdentifier (identifier normal)) <EOF>)",
            Parse(BatchFresh(), "P", "L", "normal"));
        Assert.Throws<InvalidOperationException>(() =>
            Parse(BatchFresh(), "P", "L", "permits", contextAware: true));
        Assert.Throws<InvalidOperationException>(() =>
            Parse(BatchFresh(), "P", "L", "record", contextAware: true));
        Assert.Throws<InvalidOperationException>(() =>
            Parse(BatchFresh(), "P", "L", "record"));
    }

    [Fact]
    public void ParserSetDifferenceParticipatesInAlternativePrediction()
    {
        const string lexer = "lexer grammar L; Identifier:[a-z]+;";
        const string parser = "parser grammar P; options{tokenVocab=L;} " +
            "start:typeIdentifier EOF | Identifier EOF; " +
            "typeIdentifier:identifier - 'permits'; identifier:Identifier;";
        List<GrammarModel> BatchFresh() => Batch(Model(parser), Model(lexer));
        Assert.Equal("(start (typeIdentifier (identifier normal)) <EOF>)",
            Parse(BatchFresh(), "P", "L", "normal"));
        Assert.Equal("(start permits <EOF>)",
            Parse(BatchFresh(), "P", "L", "permits"));
    }

    [Fact]
    public void EarleyExplicitlyRejectsParserSetDifference()
    {
        var models = Batch(
            Model("parser grammar P; options{tokenVocab=L;} start:word EOF; word:Identifier - 'if';"),
            Model("lexer grammar L; Identifier:[a-z]+;"));
        var directory = Directory.CreateTempSubdirectory("G4XEarleyDifference-").FullName;
        try
        {
            foreach (var model in models)
                File.WriteAllText(Path.Combine(directory, model.Name + ".interp"), Tables(model));
            var exception = Assert.Throws<NotSupportedException>(() =>
                EarleyAtnParser.InterpRunner.Run(
                    Path.Combine(directory, "P.interp"),
                    Path.Combine(directory, "L.interp"), "if", "input.txt", false));
            Assert.Contains("--allstar", exception.Message);
        }
        finally { Directory.Delete(directory, true); }
    }

    [Fact]
    public void ContextAwareIndirectRecursionHonorsParserSetDifference()
    {
        const string lexer = "lexer grammar L; KEY:'key'; WORD:[a-z]+; PLUS:'+';";
        const string parser = "parser grammar P; options{tokenVocab=L;} " +
            "start:expr EOF; expr:bridge | word; bridge:expr PLUS word; " +
            "word:WORD - 'key';";
        List<GrammarModel> BatchFresh() => Batch(Model(parser), Model(lexer));
        Assert.Equal(
            "(start (expr (bridge (expr (word alpha)) + (word beta))) <EOF>)",
            Parse(BatchFresh(), "P", "L", "alpha+beta",
                indirect: true, contextAware: true));
        Assert.Throws<InvalidOperationException>(() =>
            Parse(BatchFresh(), "P", "L", "key+beta",
                indirect: true, contextAware: true));
    }

    [Fact]
    public void ModesAndCommandsSurviveLowering()
    {
        var lexer = Model("lexer grammar L; open : '<' -> pushMode(inside); mode inside; word : [a-z]+; close : '>' -> popMode;");
        var parser = Model("parser grammar P; options {tokenVocab=L;} Start : open word close EOF;");
        Assert.Equal("(Start < abc > <EOF>)", Parse(Batch(parser, lexer), "P", "L", "<abc>"));
    }

    [Fact]
    public void CombinedGrammarCanSelectExternalLexer()
    {
        var lexer = Model("lexer grammar L; number:[0-9]+;");
        var parser = Model("grammar P; options{tokenVocab=L;} Start:number EOF;");
        Assert.Null(parser.ImplicitLexer);
        Assert.Equal("(Start 12 <EOF>)", Parse(Batch(parser, lexer), "P", "L", "12"));
    }

    [Fact]
    public void MoreIsSerializedButRetainsTheRuntimeUnsupportedDiagnostic()
    {
        var lexer = Model("""
            lexer grammar L;
            tokens { text }
            begin : '"' -> more, pushMode(inside);
            mode inside;
            chars : ~["]+ -> more;
            end : '"' -> type(text), popMode;
            """);
        var parser = Model("parser grammar P; options{tokenVocab=L;} Start:text EOF;");
        var batch = Batch(parser, lexer);
        var interp = Atn.InterpFileReader.Read(Tables(lexer));
        var atn = Atn.AtnDeserializer.Deserialize(interp.AtnData);
        Assert.Contains(atn.lexerActions, action => action.ActionType == Atn.MyLexerActionType.More);
        Assert.Contains("More", Assert.Throws<NotSupportedException>(() => Parse(batch, "P", "L", "\"abc\"")).Message);
    }

    [Fact]
    public void TypeCommandUsesDeclaredCaseNeutralToken()
    {
        var lexer = Model("lexer grammar L; tokens{text} letters:[a-z]+ -> type(text);");
        var parser = Model("parser grammar P; options{tokenVocab=L;} Start:text EOF;");
        Assert.Equal("(Start abc <EOF>)", Parse(Batch(parser, lexer), "P", "L", "abc"));
    }

    [Fact]
    public void RuleAndTokenWithSameNameAreDiagnosed()
    {
        var lexer = Model("lexer grammar L; word:'x';");
        var parser = Model("parser grammar P; options{tokenVocab=L;} Start:word EOF; word:'x';");
        Assert.Contains("Ambiguous symbol", Assert.Throws<InvalidOperationException>(() => Batch(parser, lexer)).Message);
    }

    [Fact]
    public void ExternalTokenFileIsResolvedBesideSource()
    {
        var directory = Directory.CreateTempSubdirectory("G4XVocab-").FullName;
        try
        {
            File.WriteAllText(Path.Combine(directory, "L.tokens"), "number=1\n'+'=2\nplus=2\n");
            var parser = Model("parser grammar P; options{tokenVocab=L;} Start:number '+' number EOF;");
            parser.FileName = Path.Combine(directory, "P.g4x");
            Batch(parser);
            Assert.Equal(2, parser.StringLiteralToType["'+'"]);
            Assert.Contains("rule names:", Tables(parser));
        }
        finally { Directory.Delete(directory, true); }
    }

    [Theory]
    [InlineData("grammar C; Start : ('x' | 'y')+ 'z'? EOF;", "xyyz", "(Start x y y z <EOF>)")]
    [InlineData("grammar C; Start : Empty 'x' EOF; Empty : ;", "x", "(Start Empty x <EOF>)")]
    public void BlocksRepetitionAndEmptyRules(string source, string input, string expected)
    {
        Assert.Equal(expected, Parse(Batch(Model(source)), "C", "CLexer", input));
    }

    [Fact]
    public void VocabularyBindingIsScopedToEachPair()
    {
        var l1 = Model("lexer grammar L1; a:'a'; word:'x';");
        var l2 = Model("lexer grammar L2; word:'y';");
        var p1 = Model("parser grammar P1; options{tokenVocab=L1;} Start:word EOF;");
        var p2 = Model("parser grammar P2; options{tokenVocab=L2;} Start:word EOF;");
        var batch = Batch(p1, l1, p2, l2);
        Assert.Equal(2, p1.TokenNameToType["word"]);
        Assert.Equal(1, p2.TokenNameToType["word"]);
        Assert.Equal("(Start x <EOF>)", Parse(batch, "P1", "L1", "x"));
        Assert.Equal("(Start y <EOF>)", Parse(batch, "P2", "L2", "y"));
    }

    [Theory]
    [InlineData("lexer grammar L; A: 'a'..'z'+; B: ('0'|'1')* 'b'; W:[ \\t]+ -> skip;")]
    [InlineData("lexer grammar L; fragment D:[0-9]; N:D+; C:'/*' .*? '*/' -> channel(HIDDEN);")]
    public void ExistingAntlrLexerProducesIdenticalTablesThroughEitherFrontend(string source)
    {
        var antlr = Model(source, false);
        var plus = Model(source);
        Batch(antlr); Batch(plus);
        Assert.Equal(Tables(antlr), Tables(plus));
    }

    [Fact]
    public void ExistingAntlrParserProducesIdenticalTablesThroughEitherFrontend()
    {
        const string source = "parser grammar P; options{tokenVocab=L;} start:expr EOF; expr:expr PLUS expr | NUM;";
        var lexer = Model("lexer grammar L; PLUS:'+'; NUM:[0-9]+;", false);
        var antlr = Model(source, false);
        var plus = Model(source);
        Batch(antlr, lexer); Batch(plus, lexer);
        Assert.Equal(Tables(antlr), Tables(plus));
    }

    [Theory]
    [InlineData("grammar C; Start : Missing EOF;", "Undefined")]
    [InlineData("lexer grammar L; word : missing;", "Undefined")]
    [InlineData("parser grammar P; options{tokenVocab=Missing;} Start:EOF;", "vocabulary")]
    [InlineData("lexer grammar L; a:'a' -> pushMode(missing);", "Invalid lexer command")]
    public void UnresolvedSymbolsFailBeforeEmission(string source, string message)
    {
        var exception = Assert.Throws<InvalidOperationException>(() => Batch(Model(source)));
        Assert.Contains(message, exception.Message);
    }

    [Theory]
    [InlineData("lexer grammar L; word:[a-z]+ - [ab];", "character sets or ranges")]
    [InlineData("grammar C; Start:'x' EOF - 'y';", "consumes exactly one token")]
    [InlineData("grammar C; Start:'x' - Missing;", "string-literal exclusions")]
    [InlineData("lexer grammar L; word:'x' - Missing;", "Undefined G4X set-difference")]
    [InlineData("grammar C; Start:[a-z] EOF;", "scannerless")]
    [InlineData("grammar C; Start:'x' -> skip;", "Lexer commands")]
    [InlineData("lexer grammar L; fragment a:'a'; b:~a;", "set expansion")]
    [InlineData("grammar C; import Other; Start:'x' EOF;", "imports")]
    public void UnsupportedSemanticsAreNotSilentlyIgnored(string source, string message)
    {
        var exception = Assert.ThrowsAny<Exception>(() => Model(source));
        Assert.Contains(message, exception.Message);
    }
}
