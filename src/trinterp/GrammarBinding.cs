using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace trinterp;

/// <summary>Resolve vocabularies independently of the grammar source notation.</summary>
public static class GrammarBinding
{
    public static void Bind(IReadOnlyList<GrammarModel> models)
    {
        foreach (var duplicate in models.GroupBy(m => m.Name).Where(g => g.Count() > 1))
            throw new InvalidOperationException($"Duplicate grammar '{duplicate.Key}'.");
        foreach (var model in models.Where(m => !m.IsLexer))
        {
            var lexer = model.ImplicitLexer;
            if (lexer == null && model.TokenVocab != null)
            {
                lexer = models.SingleOrDefault(m => m.IsLexer && m.Name == model.TokenVocab);
                if (lexer == null) lexer = ReadVocabulary(model);
            }
            else if (lexer == null)
            {
                var lexers = models.Where(m => m.IsLexer).ToList();
                if (lexers.Count > 1)
                    throw new InvalidOperationException($"Grammar '{model.Name}' must select a tokenVocab when multiple lexers are supplied.");
                lexer = lexers.SingleOrDefault();
            }
            if (lexer != null)
            {
                foreach (var pair in lexer.TokenNameToType) model.TokenNameToType[pair.Key] = pair.Value;
                foreach (var pair in lexer.StringLiteralToType) model.StringLiteralToType[pair.Key] = pair.Value;
            }
        }
        foreach (var model in models)
        {
            if (model.Rules.GroupBy(r => r.Name).Any(g => g.Count() > 1))
                throw new InvalidOperationException($"Duplicate rules in '{model.Name}'.");
            foreach (var rule in model.Rules)
            {
                if (model.IsG4Plus && !model.IsLexer && model.TokenNameToType.ContainsKey(rule.Name))
                    throw new InvalidOperationException($"Ambiguous symbol '{rule.Name}' in '{model.Name}': both a parser rule and a token.");
                if (rule.BodyNode == null) continue;
                foreach (var node in rule.BodyNode.DescendantsAndSelf())
                {
                    if (model.IsG4Plus && model.IsLexer && node.LocalName == "lexerCommand")
                        ValidateCommand(model, node);
                    if (node.LocalName == "ruleref")
                    {
                        var name = GrammarParser.GetText(GrammarParser.ChildTerminal(node, "RULE_REF")).Trim();
                        if (model.GetRule(name) == null)
                            throw new InvalidOperationException($"Undefined rule '{name}' in '{model.Name}.{rule.Name}'.");
                    }
                    if (!model.IsG4Plus || model.IsLexer || node.LocalName is not ("terminalDef" or "setElement")) continue;
                    foreach (var token in node.Children.Where(c => c.Terminal))
                    {
                        if (token.LocalName == "TOKEN_REF" &&
                            (!model.TokenNameToType.TryGetValue(token.GetText(), out int type) || type == 0))
                            throw new InvalidOperationException($"Undefined token or rule '{token.GetText()}' in '{model.Name}.{rule.Name}'.");
                        if (token.LocalName == "STRING_LITERAL" && !model.StringLiteralToType.ContainsKey(token.GetText()))
                            throw new InvalidOperationException($"Literal {token.GetText()} has no token in '{model.Name}.{rule.Name}'.");
                    }
                }
            }
        }
    }

    private static void ValidateCommand(GrammarModel model, GrammarNode node)
    {
        var name = GrammarParser.GetText(GrammarParser.Child(node, "lexerCommandName"));
        var argument = GrammarParser.Child(node, "lexerCommandExpr");
        var value = GrammarParser.GetText(argument);
        bool valid = name switch
        {
            "skip" or "more" or "popMode" => argument == null,
            "mode" or "pushMode" => argument != null && model.ModeNames.Contains(value),
            "type" => argument != null && model.TokenNameToType.TryGetValue(value, out int type) && type > 0,
            "channel" => argument != null && (value is "HIDDEN" or "DEFAULT_TOKEN_CHANNEL" or "DEFAULT_CHANNEL" || model.ExtraChannelNames.Contains(value)),
            _ => false
        };
        if (!valid) throw new InvalidOperationException($"Invalid lexer command '{node.GetText()}' in '{model.Name}': unknown command, argument, or missing declaration.");
    }

    private static GrammarModel ReadVocabulary(GrammarModel parser)
    {
        var path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(parser.FileName)), parser.TokenVocab + ".tokens");
        if (!File.Exists(path))
            throw new InvalidOperationException($"Token vocabulary '{parser.TokenVocab}' for '{parser.Name}' was not supplied and '{path}' does not exist.");
        var vocabulary = new GrammarModel();
        foreach (var line in File.ReadLines(path).Where(l => !string.IsNullOrWhiteSpace(l)))
        {
            int eq = line.LastIndexOf('=');
            if (eq < 1 || !int.TryParse(line[(eq + 1)..], out int type) || type <= 0)
                throw new InvalidOperationException($"Invalid token vocabulary entry in '{path}': {line}");
            var name = line[..eq];
            (name.StartsWith("'") ? vocabulary.StringLiteralToType : vocabulary.TokenNameToType).Add(name, type);
        }
        return vocabulary;
    }
}
