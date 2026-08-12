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
using XQuery.Parser;

namespace Trash;

class Command
{
    public string Help()
    {
        using Stream stream = GetType().Assembly.GetManifestResourceStream("trxquery.readme.md");
        using StreamReader reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public void Execute(Config config)
    {
        string query;
        if (config.QueryFile != null && config.QueryFile != "")
        {
            try
            {
                query = File.ReadAllText(config.QueryFile).Trim();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading query file '{config.QueryFile}': {ex.Message}");
                return;
            }
            if (string.IsNullOrWhiteSpace(query))
            {
                Console.Error.WriteLine($"Error: query file '{config.QueryFile}' is empty.");
                return;
            }
        }
        else if (config.Query != null && config.Query.Any())
        {
            query = config.Query.First();
        }
        else
        {
            Console.Error.WriteLine("Error: provide an XQuery expression as an argument or via -q <file>.");
            return;
        }

        if (config.Verbose)
            Console.Error.WriteLine("Query = >>>" + query + "<<<");

        string lines;
        if (config.File != null && config.File != "")
        {
            if (config.Verbose)
                Console.Error.WriteLine("reading from file >>>" + config.File + "<<<");
            lines = File.ReadAllText(config.File);
        }
        else
        {
            if (config.Verbose)
                Console.Error.WriteLine("reading from stdin");
            for (;;)
            {
                lines = Console.In.ReadToEnd();
                if (lines != null && lines != "") break;
            }
            lines = lines!.Trim();
        }

        var serializeOptions = new JsonSerializerOptions();
        serializeOptions.Converters.Add(new AntlrJson.ParsingResultSetSerializer());
        serializeOptions.WriteIndented = config.Format;
        serializeOptions.MaxDepth = 10000;

        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting deserialization");
        var data = JsonSerializer.Deserialize<ParsingResultSet[]>(lines, serializeOptions)!;
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("deserialized");

        // Parse the XQuery module.
        var xqueryParser = new XQueryParser(query);
        var moduleNode = xqueryParser.ParseModule();

        // Build a base context that includes fn:doc() so queries can load external
        // parse tree files and navigate them with standard XPath axes.
        AdapterDocument LoadDocFile(string path)
        {
            var json2  = File.ReadAllText(path);
            var data2  = JsonSerializer.Deserialize<ParsingResultSet[]>(json2, serializeOptions)!;
            return AdapterDocument.Build(data2.SelectMany(prs => prs.Nodes));
        }

        var docFn = new XdmFunction(
            new XdmQName(XdmQName.FnNamespace, "doc", "fn"),
            1,
            args => new XdmSequence(LoadDocFile(args[0].StringValue)));

        // exec(cmd) / exec(cmd, input) — run a shell command and return its stdout as a string.
        // The optional second argument is written to the process's stdin (avoids shell quoting issues).
        // On Windows/MSYS2 this uses bash; on Unix it uses sh.
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

        var baseCtx = new EvaluationContext();
        baseCtx.RegisterFunction(new XdmQName("doc"), docFn);
        baseCtx.RegisterFunction(new XdmQName(XdmQName.FnNamespace, "doc", "fn"), docFn);
        baseCtx.RegisterFunction(new XdmQName("exec"), execFn);
        baseCtx.RegisterFunction(new XdmQName("exec"), exec2Fn);

        var results = new List<ParsingResultSet>();

        foreach (var parse_info in data)
        {
            var fn     = parse_info.FileName;
            var atrees = parse_info.Nodes;
            var parser = parse_info.Parser;
            var lexer  = parse_info.Lexer;

            // Wrap the parse tree as an XDM document.
            var adapterDoc = AdapterDocument.Build(atrees);

            // Build evaluation context with the document as the context item.
            // Inherits fn:doc() and any other base-context registrations.
            var ctx       = baseCtx.WithContextItem(adapterDoc);
            var evaluator = new ParseTreeUpdateEvaluator(ctx);

            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("evaluating XQuery module");
            var xdmResult = evaluator.EvaluateModule(moduleNode);
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("evaluation complete");

            // If the expression returned a non-empty sequence of adapter nodes, treat it
            // as a select and output only those nodes (like trxpath).
            // If empty (update expression), output the whole (mutated) tree.
            UnvParseTreeNode[] outNodes;
            var selected = new List<UnvParseTreeNode>();
            foreach (var item in xdmResult)
            {
                if (item is AdapterElement ae) selected.Add(ae.Source);
                else if (item is AdapterText at) selected.Add(at.Source);
                else if (item is AdapterAttribute aa) selected.Add(aa.Source);
                else
                {
                    // Scalar result — print to stdout and don't emit parse tree.
                    Console.WriteLine(item.StringValue);
                }
            }
            outNodes = selected.Count > 0 ? selected.ToArray() : atrees;

            var parse_info_out = new ParsingResultSet
            {
                FileName = fn,
                Lexer    = lexer,
                Parser   = parser,
                Nodes    = outNodes
            };
            results.Add(parse_info_out);
        }

        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting serialization");
        string js = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("serialized");
        Console.WriteLine(js);
    }
}
