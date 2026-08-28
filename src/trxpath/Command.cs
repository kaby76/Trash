using Antlr4.Runtime;
using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using XQuery.DataModel;
using XQuery.Engine;
using XPathParser = XQuery.Parser.XPathParser;

namespace Trash;

class Command
{
    public string Help()
    {
        using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trxpath.readme.md"))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    public void Execute(Config config)
    {
        string expr;
        if (config.QueryFile != null && config.QueryFile != "")
        {
            try
            {
                expr = System.IO.File.ReadAllText(config.QueryFile).Trim();
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine($"Error reading query file '{config.QueryFile}': {ex.Message}");
                return;
            }
            if (string.IsNullOrWhiteSpace(expr))
            {
                System.Console.Error.WriteLine($"Error: query file '{config.QueryFile}' is empty.");
                return;
            }
        }
        else if (config.Expr != null && config.Expr.Any())
        {
            expr = config.Expr.First();
        }
        else
        {
            System.Console.Error.WriteLine("Error: provide an XPath expression as an argument or via --query <file>.");
            return;
        }

        if (config.Verbose)
        {
            System.Console.Error.WriteLine("Expr = >>>" + expr + "<<<");
        }

        UnvParseTreeNode[] atrees;
        Parser parser;
        Lexer lexer;
        string fn;
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting deserialization");
        var input = ParsingResultIO.Read(config.File);
        var data = input.Results;
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("deserialized");

        var exprNode = new XPathParser(expr).Parse();

        AdapterDocument LoadDocFile(string path)
        {
            var data2 = ParsingResultIO.Read(path).Results;
            return AdapterDocument.Build(data2.SelectMany(prs => prs.Nodes));
        }

        var docFn = new XdmFunction(
            new XdmQName(XdmQName.FnNamespace, "doc", "fn"),
            1,
            args => new XdmSequence(LoadDocFile(args[0].StringValue)));

        static XdmSequence ExecShell(XdmSequence[] args)
        {
            var cmd   = args[0].StringValue;
            var input = args.Length >= 2 ? args[1].StringValue : null;
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                System.Runtime.InteropServices.OSPlatform.Windows);
            var (shell, flag) = isWindows ? ("bash", "-c") : ("sh", "-c");
            var psi = new ProcessStartInfo(shell, $"{flag} \"{cmd.Replace("\"", "\\\"")}\"")
            {
                RedirectStandardOutput = true,
                RedirectStandardInput  = input is not null,
                RedirectStandardError  = false,
                UseShellExecute        = false,
            };
            using var proc = Process.Start(psi)
                ?? throw new InvalidOperationException($"exec: failed to start '{shell}'");
            if (input is not null)
            {
                proc.StandardInput.Write(input);
                proc.StandardInput.Close();
            }
            var output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            if (proc.ExitCode != 0)
                throw new InvalidOperationException($"exec: command exited with code {proc.ExitCode}: {cmd}");
            return new XdmSequence(new XdmAtomicValue(output.TrimEnd('\r', '\n')));
        }

        var execFn  = new XdmFunction(new XdmQName("exec"), 1, ExecShell);
        var exec2Fn = new XdmFunction(new XdmQName("exec"), 2, ExecShell);

        var baseCtx = EvaluationContext.CreateDefault();
        baseCtx.RegisterFunction(new XdmQName("doc"), docFn);
        baseCtx.RegisterFunction(new XdmQName(XdmQName.FnNamespace, "doc", "fn"), docFn);
        baseCtx.RegisterFunction(new XdmQName("exec"), execFn);
        baseCtx.RegisterFunction(new XdmQName("exec"), exec2Fn);

        var evaluator = new XPathEvaluator(baseCtx);

        var results = new List<ParsingResultSet>();
        bool do_rs = !config.NoParsingResultSets;

        foreach (var parse_info in data)
        {
            fn = parse_info.FileName;
            atrees = parse_info.Nodes;
            parser = parse_info.Parser;
            lexer = parse_info.Lexer;

            var adapterDoc = AdapterDocument.Build(atrees);
            var xdmResults = evaluator.Evaluate(exprNode, adapterDoc);

            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + xdmResults.Count + " nodes.");

            List<UnvParseTreeNode> res = new List<UnvParseTreeNode>();
            foreach (var item in xdmResults)
            {
                if (item is AdapterElement ae)
                {
                    res.Add(ae.Source);
                }
                else if (item is AdapterText at)
                {
                    do_rs = false;
                    System.Console.WriteLine(at.Source.Data);
                }
                else if (item is AdapterAttribute aa)
                {
                    do_rs = false;
                    System.Console.WriteLine(aa.Source.StringValue);
                }
                else if (item is AdapterDocument)
                {
                    do_rs = false;
                    System.Console.WriteLine(item);
                }
                else
                {
                    do_rs = false;
                    System.Console.WriteLine(item.StringValue);
                }
            }

            var parse_info_out = new AntlrJson.ParsingResultSet()
                { FileName = fn, Lexer = lexer, Parser = parser, Nodes = res.ToArray() };
            results.Add(parse_info_out);
        }

        if (do_rs)
        {
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting serialization");
            using var output = Console.OpenStandardOutput();
            ParsingResultIO.WriteBundle(output, input, results, config.Format);
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("serialized");
        }
    }
}
