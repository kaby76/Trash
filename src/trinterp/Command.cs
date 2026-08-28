using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;
using XQuery.Engine;
using XPathParser = XQuery.Parser.XPathParser;

namespace trinterp;

/// <summary>
/// Reads a <see cref="AntlrJson.ParsingResultSet[]"/> from stdin (or a file),
/// builds the ATN for each grammar parse tree, and writes .interp / .tokens files.
/// </summary>
public class Command
{
    public string Help() =>
        "trinterp: Generate .interp and .tokens files from an ANTLRv4 grammar parse tree.\n" +
        "Usage: dotnet trash parse grammar.g4 | dotnet trash interp [options]\n";

    public void Execute(Config config)
    {
        var sets = ParsingResultIO.Read(config.File).Results;

        if (sets == null || sets.Length == 0) return;

        // ---- Prepare output directory ----
        var outDir = config.OutputDirectory ?? ".";
        Directory.CreateDirectory(outDir);

        var optimizeNames = new List<string>(config.Optimize ?? Array.Empty<string>());
        var optimize = optimizeNames.Count > 0
            ? OptimizeOptions.FromNames(optimizeNames)
            : OptimizeOptions.All;

        // Pass 1: parse all grammar models and detect EOF-terminated start rules.
        var models = new List<GrammarModel>();
        var startRuleIndices = new Dictionary<GrammarModel, int>(); // parser models only
        foreach (var set in sets)
        {
            if (set.Nodes == null || set.Nodes.Length == 0) continue;
            var root = set.Nodes[0] as UnvParseTreeElement;
            if (root == null) continue;
            var fileName = set.FileName ?? "grammar";
            GrammarModel model;
            try { model = new GrammarParser().Parse(root, fileName); }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[trinterp] Failed to parse grammar model for {fileName}: {ex.Message}");
                continue;
            }
            models.Add(model);
            if (model.ImplicitLexer != null) models.Add(model.ImplicitLexer);

            // For parser grammars, determine the start rule.
            if (!model.IsLexer)
            {
                string startRuleName;
                if (!string.IsNullOrEmpty(config.StartRule))
                {
                    startRuleName = config.StartRule;
                }
                else
                {
                    var eofRules = FindEofTerminatedRules(root);
                    if (eofRules.Count == 0)
                        throw new Exception(
                            $"[trinterp] No EOF-terminated parser rule found in {model.Name}. " +
                            "Add EOF to exactly one rule or use --start-rule to designate the start rule.");
                    if (eofRules.Count > 1)
                        throw new Exception(
                            $"[trinterp] Multiple EOF-terminated parser rules found in {model.Name}: " +
                            string.Join(", ", eofRules) + ". Use --start-rule to specify which one.");
                    startRuleName = eofRules[0];
                }
                var startRuleModel = model.Rules.FirstOrDefault(r => r.Name == startRuleName);
                if (startRuleModel == null)
                    throw new Exception(
                        $"[trinterp] Start rule '{startRuleName}' not found in grammar {model.Name}.");
                startRuleIndices[model] = startRuleModel.Index;
            }
        }

        // Collect the string-literal vocabulary from every lexer model in this batch.
        // When a parser grammar is paired with a separate lexer grammar (e.g. CParser +
        // CLexer), the parser has no StringLiteralToType of its own; merging the lexer's
        // vocabulary lets TokenLabel show 'while' instead of While.
        var batchLiterals = new Dictionary<string, int>();
        foreach (var m in models)
            if (m.IsLexer)
                foreach (var kv in m.StringLiteralToType)
                    if (!batchLiterals.ContainsKey(kv.Key))
                        batchLiterals[kv.Key] = kv.Value;

        // Collect the token name→type vocabulary from every lexer model in this batch.
        var batchTokenNames = new Dictionary<string, int>();
        foreach (var m in models)
            if (m.IsLexer)
                foreach (var kv in m.TokenNameToType)
                    if (!batchTokenNames.ContainsKey(kv.Key) || batchTokenNames[kv.Key] == 0)
                        batchTokenNames[kv.Key] = kv.Value;

        // Pass 2: apply shared vocabulary to parser models then emit.
        foreach (var model in models)
        {
            if (!model.IsLexer)
            {
                // Merge string literal vocabulary (for display labels).
                if (batchLiterals.Count > 0)
                    foreach (var kv in batchLiterals)
                        if (!model.StringLiteralToType.ContainsKey(kv.Key))
                            model.StringLiteralToType[kv.Key] = kv.Value;

                // Merge token name→type so ResolveTokenType returns correct types
                // (parser grammar uses 0 as placeholder until lexer vocab is available).
                if (batchTokenNames.Count > 0)
                    foreach (var kv in batchTokenNames)
                        if (!model.TokenNameToType.ContainsKey(kv.Key) ||
                            model.TokenNameToType[kv.Key] == 0)
                            model.TokenNameToType[kv.Key] = kv.Value;
            }

            int? startRuleIdx = startRuleIndices.TryGetValue(model, out var sri) ? sri : (int?)null;
            EmitGrammar(model, config, outDir, optimize, startRuleIdx);
        }
    }

    private static void EmitGrammar(GrammarModel grammar, Config config, string outDir, OptimizeOptions optimize, int? startRuleIndex = null)
    {
        // ---- Build ATN ----
        ParserAtnFactory factory = grammar.IsLexer
            ? new LexerAtnFactory(grammar, optimize)
            : new ParserAtnFactory(grammar, optimize);

        ATN atn;
        try
        {
            atn = factory.CreateATN();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[trinterp] ATN construction failed for {grammar.Name}: {ex.Message}");
            return;
        }

        // ---- Build location map (needed for --state-map, --atn, --atn-combined) ----
        StateLocationMap lm = (config.Atn || config.AtnCombined || config.StateMap)
            ? StateLocationMap.Build(atn)
            : null;

        // ---- Format content ----
        string interpContent;
        string tokensContent;
        try
        {
            interpContent = InterpFormatter.FormatInterp(grammar, atn, config.ActionsInInterp);
            tokensContent = InterpFormatter.FormatTokens(grammar);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[trinterp] Formatting failed for {grammar.Name}: {ex.Message}");
            return;
        }

        if (config.StateMap)
            interpContent += AtnDotWriter.FormatStateMap(grammar, atn, lm);

        // Append start-rule section for parser grammars (gives the ATN start-state number).
        if (!grammar.IsLexer && startRuleIndex.HasValue)
        {
            int stateNumber = atn.ruleToStartState[startRuleIndex.Value].stateNumber;
            interpContent += "\nstart-rule:\n" + stateNumber + "\n";
        }

        // ---- Write files ----
        var baseName = grammar.Name;
        var interpPath = Path.Combine(outDir, baseName + ".interp");
        var tokensPath = Path.Combine(outDir, baseName + ".tokens");

        File.WriteAllText(interpPath, interpContent);
        File.WriteAllText(tokensPath, tokensContent);

        if (config.Atn)
            AtnDotWriter.WritePerRule(grammar, atn, outDir, lm);
        if (config.AtnCombined)
            AtnDotWriter.WriteCombined(grammar, atn, outDir, lm);

        if (config.Verbose)
        {
            Console.Error.WriteLine($"[trinterp] Wrote {interpPath}");
            Console.Error.WriteLine($"[trinterp] Wrote {tokensPath}");
            if (config.Atn)
                foreach (var rule in grammar.Rules)
                    Console.Error.WriteLine($"[trinterp] Wrote {Path.Combine(outDir, rule.Name + ".dot")}");
            if (config.AtnCombined)
                Console.Error.WriteLine($"[trinterp] Wrote {Path.Combine(outDir, grammar.Name + ".atn.dot")}");
        }
    }

    /// <summary>
    /// Uses the XPath4 engine to find parser rules whose body explicitly references EOF.
    /// An "EOF-terminated" rule is one that contains at least one terminalDef whose
    /// TOKEN_REF child has the text "EOF".  Returns the list of matching rule names.
    /// </summary>
    private static List<string> FindEofTerminatedRules(UnvParseTreeElement root)
    {
        var adapterDoc = AdapterDocument.Build(new UnvParseTreeNode[] { root });
        // Find parserRuleSpec nodes that contain a terminalDef/TOKEN_REF whose
        // normalised text is "EOF".
        const string xpathExpr =
            "//parserRuleSpec[.//terminalDef/TOKEN_REF[normalize-space(.) = 'EOF']]";
        var exprNode = new XPathParser(xpathExpr).Parse();
        var evaluator = new XPathEvaluator();
        var results = evaluator.Evaluate(exprNode, adapterDoc);

        var names = new List<string>();
        foreach (var item in results)
        {
            if (item is AdapterElement ae)
            {
                var ruleRef = GrammarParser.ChildTerminal(ae.Source, "RULE_REF");
                if (ruleRef != null)
                    names.Add(GrammarParser.GetText(ruleRef).Trim());
            }
        }
        return names;
    }
}
