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

        foreach (var set in sets)
        {
            if (set.Nodes == null || set.Nodes.Length == 0) continue;

            // The root of the parse tree is the first node cast as an element.
            var root = set.Nodes[0] as UnvParseTreeElement;
            if (root == null) continue;

            var fileName = set.FileName ?? "grammar";

            // ---- Parse grammar model ----
            GrammarModel model;
            try
            {
                model = new GrammarParser().Parse(root, fileName);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[trinterp] Failed to parse grammar model for {fileName}: {ex.Message}");
                continue;
            }

            // ---- Emit for lexer/combined and parser halves ----
            EmitGrammar(model, config, outDir);

            if (model.ImplicitLexer != null)
                EmitGrammar(model.ImplicitLexer, config, outDir);
        }
    }

    private static void EmitGrammar(GrammarModel grammar, Config config, string outDir)
    {
        // ---- Build ATN ----
        ParserAtnFactory factory = grammar.IsLexer
            ? new LexerAtnFactory(grammar)
            : new ParserAtnFactory(grammar);

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

        if (config.Verbose)
        {
            Console.Error.WriteLine($"[trinterp] Wrote {interpPath}");
            Console.Error.WriteLine($"[trinterp] Wrote {tokensPath}");
        }
    }
}
