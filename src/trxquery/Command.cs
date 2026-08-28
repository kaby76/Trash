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
using XQuery.Parser.Ast;

namespace Trash;

class Command
{
    public string Help()
    {
        using Stream stream = GetType().Assembly.GetManifestResourceStream("trxquery.readme.md")!;
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

        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting deserialization");
        var input = ParsingResultIO.Read(config.File);
        var data = input.Results;
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("deserialized");

        // Parse the XQuery module.
        var xqueryParser = new XQueryParser(query);
        var moduleNode = xqueryParser.ParseModule();

        // Collect external variable bindings: positional CLI args map to
        // declare variable $name external; declarations in the module prolog,
        // in declaration order.  This supports the idiom:
        //
        //   declare variable $name external;   ← in the .xq file
        //   dotnet trash xquery --query file.xq value1 value2 ...   ← on the CLI
        //
        // When --query is used, all positional args are params.
        // When an inline query is used, config.Query[0] is the query itself,
        // so params start at index 1.
        var externalVarDecls = (moduleNode.Prolog?.VariableDecls ?? [])
            .Where(v => v.IsExternal)
            .ToList();
        var positionalParams = (config.QueryFile != null && config.QueryFile != "")
            ? config.Query.ToList()
            : config.Query.Skip(1).ToList();

        // Build a base context that includes fn:doc() so queries can load external
        // parse tree files and navigate them with standard XPath axes.
        AdapterDocument LoadDocFile(string path)
        {
            var data2  = ParsingResultIO.Read(path).Results;
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

        // Bind external variables to the positional args collected above.
        for (int i = 0; i < Math.Min(externalVarDecls.Count, positionalParams.Count); i++)
        {
            var varName  = externalVarDecls[i].Name;
            var varValue = new XdmSequence(new XdmAtomicValue(positionalParams[i]));
            if (config.Verbose)
                Console.Error.WriteLine($"binding external variable ${varName} = '{positionalParams[i]}'");
            baseCtx.SetVariable(varName, varValue);
        }

        var results = new List<ParsingResultSet>();
        var do_rs = true;

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

            // If the expression returned a non-empty sequence of element nodes, treat it
            // as a select and output only those nodes. Text, attribute, document, and
            // scalar results are written directly and suppress parse-result serialization,
            // matching trxpath. If empty (update expression), output the whole mutated tree.
            UnvParseTreeNode[] outNodes;
            var selected = new List<UnvParseTreeNode>();
            foreach (var item in xdmResult)
            {
                if (item is AdapterElement ae) selected.Add(ae.Source);
                else if (item is XdmElement xe) selected.Add(XdmToUnv(xe));
                else if (item is AdapterText at)
                {
                    do_rs = false;
                    Console.WriteLine(at.Source.Data);
                }
                else if (item is AdapterAttribute aa)
                {
                    do_rs = false;
                    Console.WriteLine(aa.Source.StringValue);
                }
                else if (item is AdapterDocument)
                {
                    do_rs = false;
                    Console.WriteLine(item);
                }
                else
                {
                    do_rs = false;
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

        if (do_rs)
        {
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting serialization");
            using var output = Console.OpenStandardOutput();
            ParsingResultIO.WriteBundle(output, input, results, config.Format);
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("serialized");
        }
    }

    // Converts a constructed XdmElement tree (from an element constructor) back
    // to a UnvParseTreeElement so it can be serialised as a JSON parse tree.
    static UnvParseTreeElement XdmToUnv(XdmElement elem)
    {
        var result = new UnvParseTreeElement { LocalName = elem.NodeName?.LocalName ?? "" };
        var list = (UnvParseTreeNodeList)result.ChildNodes;

        foreach (var attr in elem.Attributes)
        {
            list.Add(new UnvParseTreeAttr
            {
                LocalName = attr.LocalName,
                Name      = attr.LocalName,
                StringValue = attr.Value,
                NamespaceURI = attr.NamespaceUri,
                Prefix    = attr.Prefix,
            });
        }

        foreach (var child in elem.Children)
        {
            if (child is XdmElement childElem)
                list.Add(XdmToUnv(childElem));
            else if (child is XdmText childText)
                list.Add(new UnvParseTreeText { Data = childText.Value });
        }

        return result;
    }
}
