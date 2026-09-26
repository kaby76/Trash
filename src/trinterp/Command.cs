using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;

namespace trinterp;

/// <summary>
/// Reads a <see cref="AntlrJson.ParsingResultSet[]"/> from stdin (or a file),
/// builds the ATN for each grammar parse tree, and writes .interp / .tokens files.
/// </summary>
public class Command
{
    public string Help() =>
        "trinterp: Generate .interp and .tokens files from ANTLRv4 or G4Plus grammar parse trees.\n" +
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
                throw new InvalidOperationException($"[trinterp] Failed to parse grammar model for {fileName}: {ex.Message}", ex);
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
                    var eofRules = FindEofTerminatedRules(model);
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

        // Bind and validate the entire batch before writing any tables.
        GrammarBinding.Bind(models);
        foreach (var model in models)
        {
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
            throw new InvalidOperationException($"[trinterp] ATN construction failed for {grammar.Name}: {ex.Message}", ex);
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
            throw new InvalidOperationException($"[trinterp] Formatting failed for {grammar.Name}: {ex.Message}", ex);
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
    /// Finds parser rules whose compiled body explicitly references EOF.
    /// An "EOF-terminated" rule is one that contains at least one terminalDef whose
    /// TOKEN_REF child has the text "EOF".  Returns the list of matching rule names.
    /// </summary>
    public static List<string> FindEofTerminatedRules(GrammarModel model)
    {
        return model.Rules.Where(r => !r.IsLexerRule && r.BodyNode != null &&
            r.BodyNode.DescendantsAndSelf().Any(n => n.LocalName == "terminalDef" &&
                GrammarParser.GetText(GrammarParser.ChildTerminal(n, "TOKEN_REF")).Trim() == "EOF"))
            .Select(r => r.Name).ToList();
    }
}
