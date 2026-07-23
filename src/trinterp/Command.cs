using System;
using System.Collections.Generic;
using System.IO;
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
        "trinterp: Generate .interp and .tokens files from an ANTLRv4 grammar parse tree.\n" +
        "Usage: trparse grammar.g4 | trinterp [options]\n";

    public void Execute(Config config)
    {
        // ---- Read input ----
        string json;
        if (!string.IsNullOrEmpty(config.File))
            json = File.ReadAllText(config.File);
        else
            json = Console.In.ReadToEnd();

        // ---- Deserialize ----
        var serializeOptions = new JsonSerializerOptions();
        serializeOptions.Converters.Add(new ParsingResultSetSerializer());
        serializeOptions.MaxDepth = 10000;
        var sets = JsonSerializer.Deserialize<ParsingResultSet[]>(json, serializeOptions);

        if (sets == null || sets.Length == 0) return;

        // ---- Prepare output directory ----
        var outDir = config.OutputDirectory ?? ".";
        Directory.CreateDirectory(outDir);

        var optimizeNames = new List<string>(config.Optimize ?? Array.Empty<string>());
        var optimize = optimizeNames.Count > 0
            ? OptimizeOptions.FromNames(optimizeNames)
            : OptimizeOptions.All;

        // Pass 1: parse all grammar models.
        var models = new List<GrammarModel>();
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

            EmitGrammar(model, config, outDir, optimize);
        }
    }

    private static void EmitGrammar(GrammarModel grammar, Config config, string outDir, OptimizeOptions optimize)
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

        // ---- Write files ----
        var baseName = grammar.Name;
        var interpPath = Path.Combine(outDir, baseName + ".interp");
        var tokensPath = Path.Combine(outDir, baseName + ".tokens");

        File.WriteAllText(interpPath, interpContent);
        File.WriteAllText(tokensPath, tokensContent);

        if (config.Atn)
            AtnDotWriter.WritePerRule(grammar, atn, outDir);
        if (config.AtnCombined)
            AtnDotWriter.WriteCombined(grammar, atn, outDir);

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
}
