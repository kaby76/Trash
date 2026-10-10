using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Trash;

public class Grun
{
    Config config;

    // Accumulated performance stats across all DoParse calls in this run
    private double _totalParseSeconds;
    private long _totalTokens;
    private long _firstFileTokens;
    private double _firstFileParseSeconds;
    private int _fileCount;
    private readonly List<BundleParse> _bundleParses = new();
    private readonly AllStarAtnParser.ParserPredictionCache _parserPredictionCache;
    private readonly AllStarAtnParser.InterpRunTimings _interpTimings = new();
    private readonly AllStarAtnParser.InterpRuntimeCache _interpRuntimeCache = new();
    private readonly Dictionary<string, bool> _grammarContextAwareLexing = new();
    private const string TimeoutWorkerName = "TRPARSE_TIMEOUT_WORKER_NAME";
    private const string TimeoutWorkerRow = "TRPARSE_TIMEOUT_WORKER_ROW";
    private readonly EarleyAtnParser.LexerAtnSimulator.LexerDfaCache
        _lexerDfaCache = new();

    private sealed record BundleParse(string InputName,
        List<AntlrJson.ParsingResultSet> Results, string Diagnostics,
        List<byte[]> SerializedResults = null);

    public Grun(Config co)
    {
        config = co;
        if (!co.NoSharedParserDfa)
        {
            _parserPredictionCache = new AllStarAtnParser.ParserPredictionCache(
                co.ParserDfaCacheStates,
                checked((long)co.ParserDfaCacheMegabytes * 1024 * 1024),
                synchronizeAccess: false);
        }
    }

    private static string JoinArguments(IEnumerable<string> arguments)
    {
        if (arguments == null)
            throw new ArgumentNullException("arguments");

        StringBuilder builder = new StringBuilder();
        foreach (string argument in arguments)
        {
            if (builder.Length > 0)
                builder.Append(' ');

            if (argument.IndexOfAny(new[] { '"', ' ' }) < 0)
            {
                builder.Append(argument);
                continue;
            }

            // escape a backslash appearing before a quote
            string arg = argument.Replace("\\\"", "\\\\\"");
            // escape double quotes
            arg = arg.Replace("\"", "\\\"");

            // wrap the argument in outer quotes
            builder.Append('"').Append(arg).Append('"');
        }

        return builder.ToString();
    }

    private void HandleOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        System.Console.WriteLine(e.Data);
    }

    public int Run(string parser_type)
    {
        int result = 0;
        if (config.TimeoutSeconds < 0)
            throw new ArgumentOutOfRangeException("--timeout",
                "The per-file timeout must be zero or a positive number of seconds.");
        if (config.TimeoutSeconds > 0 && !config.Bundle)
            throw new ArgumentException("--timeout requires bundle output.");
        DateTime overallBefore = DateTime.Now;
        InterpBundle stagedTables = null;
        string originalLib = config.Lib;
        try
        {
            if (config.BundleGlob != null)
            {
                if (!config.Bundle)
                    throw new ArgumentException("--bundle-glob requires bundle output.");
                if (config.Input != null || config.ReadFileNameStdin ||
                    config.ReadFileNameFile != null || (config.Files?.Any() ?? false))
                    throw new ArgumentException(
                        "--bundle-glob reads its inputs from stdin; do not combine it with -i, -x, --xf, or positional files.");
                if (config.AllStar && string.IsNullOrEmpty(config.Lib))
                    throw new ArgumentException(
                        "--bundle-glob with --allstar requires -L; stdin contains source files, not an interpreter bundle.");
                return RunInputBundle(parser_type, overallBefore);
            }
            if (config.AllStar && string.IsNullOrEmpty(config.Lib))
            {
                if (config.ReadFileNameStdin ||
                    (config.Input == null && string.IsNullOrEmpty(config.ReadFileNameFile) &&
                     (config.Files == null || !config.Files.Any())))
                    throw new ArgumentException(
                        "--allstar without -L reads interpreter tables from stdin; specify inputs with -i, positional files, or --xf (not -x or stdin text).");
                stagedTables = InterpBundle.Open(Console.OpenStandardInput());
                config.Lib = stagedTables.DirectoryPath;
            }
            var data = new List<AntlrJson.ParsingResultSet>();
            string txt = config.Input;
            if (config.ReadFileNameStdin)
            {
                List<string> inputs = new List<string>();
                for (; ; )
                {
                    var line = System.Console.In.ReadLine();
                    line = line?.Trim();
                    if (line == null || line == "")
                    {
                        break;
                    }

                    inputs.Add(line);
                }

                for (int f = 0; f < inputs.Count(); ++f)
                {
                    try
                    {
                        txt = File.ReadAllText(inputs[f]);
                    }
                    catch
                    {
                        txt = inputs[f];
                    }

                    var (r, _, _) = ParseOne(parser_type, txt, inputs[f], f, data);
                    result = result == 0 ? r : result;
                }
            }
            else if (config.ReadFileNameFile != null)
            {
                List<string> inputs = new List<string>();
                inputs = File.ReadAllLines(config.ReadFileNameFile).ToList();
                for (int f = 0; f < inputs.Count(); ++f)
                {
                    try
                    {
                        txt = File.ReadAllText(inputs[f]);
                    }
                    catch
                    {
                        txt = inputs[f];
                    }

                    var (r, _, _) = ParseOne(parser_type, txt, inputs[f], f, data);
                    result = result == 0 ? r : result;
                }
            }
            else if (config.Input == null && (config.Files == null || config.Files.Count() == 0))
            {
                var workerName = Environment.GetEnvironmentVariable(TimeoutWorkerName);
                if (workerName != null)
                {
                    txt = System.Console.In.ReadToEnd();
                    int.TryParse(Environment.GetEnvironmentVariable(TimeoutWorkerRow),
                        out int workerRow);
                    (result, _, _) = ParseOne(parser_type, txt,
                        workerName, workerRow, data);
                }
                else
                {
                    string lines = null;
                    for (; ; )
                    {
                        lines = System.Console.In.ReadToEnd();
                        if (lines != null && lines != "") break;
                    }

                    txt = lines;
                    (result, _, _) = ParseOne(parser_type, txt, "stdin", 0, data);
                }
            }
            else if (config.Input != null)
            {
                txt = config.Input;
                (result, _, _) = ParseOne(parser_type, txt, "string", 0, data);
            }
            else if (config.Files != null)
            {
                int f = 0;
                foreach (var file in config.Files)
                {
                    try
                    {
                        txt = File.ReadAllText(file);
                    }
                    catch
                    {
                        txt = file;
                    }

                    var (r, _, _) = ParseOne(parser_type, txt, file, f, data);
                    result = result == 0 ? r : result;
                    f++;
                }
            }

            if (config.Verbose)
            {
                foreach (var d in data)
                {
                    foreach (var t in d.Nodes)
                    {
                        if (config.Verbose)
                            LoggerNs.TimedStderrOutput.WriteLine(new TreeOutput(d.Lexer, d.Parser).OutputTree(t)
                                .ToString());
                    }
                }
            }

            foreach (var d in data)
            {
                if (d.NodeProvider != null && !d.HasMaterializedNodes)
                    continue;
                foreach (var t1 in d.Nodes)
                {
                    var count = 0;
                    foreach (var t2 in d.Nodes)
                    {
                        if (t1 == t2) count++;
                        if (count > 1) throw new Exception();
                    }
                }
            }

            DateTime overallAfter = DateTime.Now;
            PrintPerfSummary((overallAfter - overallBefore).TotalSeconds);

            if (config.NoParsingResultSets || config.NoOutput) return result;
            if (config.Bundle)
            {
                WriteBundle();
                return result;
            }
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting serialization");
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParsingResultSetSerializer());
            serializeOptions.WriteIndented = config.Format;
            serializeOptions.MaxDepth = 10000;
            string js1 = JsonSerializer.Serialize(data.ToArray(), serializeOptions);
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("serialized");
            if (!config.Quiet) System.Console.WriteLine(js1);
        }
        catch (Exception e)
        {
            System.Console.Error.WriteLine(e.ToString());
            result = 1;
            if (!config.Bundle)
                System.Console.Out.WriteLine();
        }
        finally
        {
            config.Lib = originalLib;
            stagedTables?.Dispose();
        }

        return result;
    }

    private sealed record TimedWorkerResult(int ExitCode, bool TimedOut,
        double Seconds, byte[] Output, string Error);

    private (int ExitCode, double ParseSeconds, long TokenCount) ParseOneWithTimeout(
        string parserType, string text, string inputName, int rowNumber)
    {
        CacheGrammarContextAwareLexingForTimeout();
        var worker = RunTimedWorkerAsync(parserType, text, inputName, rowNumber)
            .GetAwaiter().GetResult();
        string diagnostics = worker.Error;
        if (worker.TimedOut)
        {
            WriteFailedFileStatus(inputName, rowNumber);
            var message = $"trparse: '{inputName}' timed out after " +
                $"{config.TimeoutSeconds} seconds; parse process terminated.";
            Console.Error.WriteLine(message);
            diagnostics += message + Environment.NewLine;
        }
        else if (worker.ExitCode != 0 && diagnostics.Length == 0)
        {
            WriteFailedFileStatus(inputName, rowNumber);
            diagnostics = $"trparse: '{inputName}' worker exited with code " +
                $"{worker.ExitCode}." + Environment.NewLine;
            Console.Error.Write(diagnostics);
        }
        else if (diagnostics.Length > 0)
            Console.Error.WriteLine(diagnostics.TrimEnd('\r', '\n'));

        var serialized = worker.ExitCode == 0 && !config.NoOutput &&
            !config.NoParsingResultSets
            ? ReadWorkerResults(worker.Output)
            : new List<byte[]>();
        _bundleParses.Add(new BundleParse(inputName,
            new List<AntlrJson.ParsingResultSet>(),
            worker.ExitCode == 0 ? string.Empty : diagnostics, serialized));
        return (worker.ExitCode, worker.Seconds, 0);
    }

    private async Task<TimedWorkerResult> RunTimedWorkerAsync(
        string parserType, string text, string inputName, int rowNumber)
    {
        var start = CreateTimeoutWorkerStartInfo(parserType, inputName, rowNumber);
        var clock = Stopwatch.StartNew();
        using var process = Process.Start(start)
            ?? throw new InvalidOperationException("Failed to start trparse timeout worker.");
        using var output = new MemoryStream();
        var stdout = process.StandardOutput.BaseStream.CopyToAsync(output);
        var stderr = process.StandardError.ReadToEndAsync();
        var writeInput = Task.Run(async () =>
        {
            await process.StandardInput.WriteAsync(text);
            process.StandardInput.Close();
        });
        bool timedOut = false;
        using var deadline = new System.Threading.CancellationTokenSource(
            TimeSpan.FromSeconds(config.TimeoutSeconds));
        try
        {
            await process.WaitForExitAsync(deadline.Token);
        }
        catch (OperationCanceledException) when (!process.HasExited)
        {
            timedOut = true;
            try { process.Kill(entireProcessTree: true); }
            catch (InvalidOperationException) when (process.HasExited) { }
            await process.WaitForExitAsync();
        }
        try { await writeInput; }
        catch (IOException) when (process.HasExited) { }
        catch (ObjectDisposedException) when (process.HasExited) { }
        await stdout;
        var error = await stderr;
        clock.Stop();
        return new TimedWorkerResult(timedOut ? 1 : process.ExitCode,
            timedOut, clock.Elapsed.TotalSeconds, output.ToArray(), error);
    }

    internal ProcessStartInfo CreateTimeoutWorkerStartInfo(
        string parserType, string inputName, int rowNumber)
    {
        var start = new ProcessStartInfo("dotnet")
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            WorkingDirectory = Environment.CurrentDirectory
        };
        start.ArgumentList.Add(typeof(Grun).Assembly.Location);
        void Flag(bool enabled, string name)
        {
            if (enabled) start.ArgumentList.Add(name);
        }
        void Value(string name, string value)
        {
            if (value == null) return;
            start.ArgumentList.Add(name);
            start.ArgumentList.Add(value);
        }
        Value("-t", parserType);
        Value("-d", config.Dll);
        Value("-p", config.ParserLocation);
        Value("-L", config.Lib);
        Value("--pinterp", config.PInterp);
        Value("--linterp", config.LInterp);
        Value("--start-rule", config.StartRule);
        Value("--xquery-hooks", config.XQueryHooks);
        Value("-g", config.Encoding);
        Flag(config.AllStar, "--allstar");
        Flag(config.IndirectLeftRecursion, "--indirect-left-recursion");
        Flag(config.ContextAwareLexing, "--context-aware-lexing");
        Flag(config.LexerStats, "--lexer-stats");
        Flag(config.LexerOverlaps, "--lexer-overlaps");
        Flag(config.ParserStats, "--parser-stats");
        Flag(config.NoSharedParserDfa, "--no-shared-parser-dfa");
        Value("--parser-dfa-cache-states", config.ParserDfaCacheStates.ToString(
            System.Globalization.CultureInfo.InvariantCulture));
        Value("--parser-dfa-cache-mb", config.ParserDfaCacheMegabytes.ToString(
            System.Globalization.CultureInfo.InvariantCulture));
        Flag(config.InterpTimings, "--interp-timings");
        Flag(config.LineNumbers, "-l");
        Flag(config.Quiet, "-q");
        Flag(config.Verbose, "-v");
        Flag(config.Format, "--fmt");
        Flag(config.GroupBy, "--group");
        Flag(config.NoOutput, "--no-output");
        Flag(config.NoParsingResultSets, "--no-prs");
        Flag(config.PerFilePerformance, "--per-file");
        if (Program.args?.Contains("--tokens") == true)
            start.ArgumentList.Add("--tokens");
        if (Program.args?.Contains("--numeric-token-types") == true)
            start.ArgumentList.Add("--numeric-token-types");
        start.Environment[TimeoutWorkerName] = inputName;
        start.Environment[TimeoutWorkerRow] = rowNumber.ToString();
        return start;
    }

    private static List<byte[]> ReadWorkerResults(byte[] bundle)
    {
        var results = new List<byte[]>();
        using var input = new MemoryStream(bundle, writable: false);
        using var reader = new TarReader(input);
        TarEntry entry;
        while ((entry = reader.GetNextEntry(copyData: false)) != null)
        {
            if (!entry.Name.EndsWith(".pt", StringComparison.Ordinal)) continue;
            using var data = new MemoryStream();
            entry.DataStream.CopyTo(data);
            results.Add(data.ToArray());
        }
        return results;
    }

    private static bool IsParseRejection(Exception exception) =>
        exception is InvalidOperationException &&
        exception.Message.EndsWith(": input rejected by grammar.",
            StringComparison.Ordinal);

    private bool RequiresContextAwareLexing(string parserInterp)
    {
        if (!_grammarContextAwareLexing.TryGetValue(parserInterp, out bool enabled))
        {
            enabled = Atn.InterpFileReader.RequiresContextAwareLexing(parserInterp);
            _grammarContextAwareLexing.Add(parserInterp, enabled);
        }
        return enabled;
    }

    private void CacheGrammarContextAwareLexingForTimeout()
    {
        string parserInterp;
        if (!string.IsNullOrEmpty(config.PInterp) &&
            !string.IsNullOrEmpty(config.LInterp))
            parserInterp = ResolveInterpPath(config.PInterp, config.Lib);
        else if (string.IsNullOrEmpty(config.PInterp) &&
                 string.IsNullOrEmpty(config.LInterp) &&
                 !string.IsNullOrEmpty(config.Lib))
        {
            try { parserInterp = DiscoverInterpPair(config.Lib).pinterp; }
            catch (IOException) { return; } // The worker will report the invalid path.
            catch (InvalidOperationException) { return; }
        }
        else return;

        try { RequiresContextAwareLexing(parserInterp); }
        catch (IOException) { } // Let the worker report the missing table.
    }

    private void WriteFailedFileStatus(string inputName, int rowNumber)
    {
        if (config.Quiet || !config.PerFilePerformance) return;
        string label = config.AllStar || config.IndirectLeftRecursion ||
            config.ContextAwareLexing ||
            _grammarContextAwareLexing.Values.Any(value => value) ? "ALL(*)" :
            !string.IsNullOrEmpty(config.Lib) ? "Earley" : "CSharp";
        Console.Error.WriteLine($"{label} {rowNumber} {inputName} failed");
    }

    private (int ExitCode, double ParseSeconds, long TokenCount) ParseOne(
        string parserType, string text, string inputName, int rowNumber,
        List<AntlrJson.ParsingResultSet> data)
    {
        if (config.TimeoutSeconds > 0)
            return ParseOneWithTimeout(parserType, text, inputName, rowNumber);
        if (!config.Bundle)
        {
            int countBefore = data.Count;
            try
            {
                return DoParse(parserType, text, "", inputName, rowNumber, data);
            }
            catch (Exception exception)
            {
                // A bad input must not abort the remaining positional, -x, or
                // --xf inputs. Keep the overall exit status nonzero instead.
                data.RemoveRange(countBefore, data.Count - countBefore);
                WriteFailedFileStatus(inputName, rowNumber);
                Console.Error.WriteLine(IsParseRejection(exception)
                    ? exception.Message : exception.ToString());
                return (1, 0, 0);
            }
        }

        int start = data.Count;
        var originalError = Console.Error;
        using var captured = new StringWriter();
        (int ExitCode, double ParseSeconds, long TokenCount) outcome;
        try
        {
            Console.SetError(captured);
            outcome = DoParse(parserType, text, "", inputName, rowNumber, data);
        }
        catch (Exception exception)
        {
            data.RemoveRange(start, data.Count - start);
            WriteFailedFileStatus(inputName, rowNumber);
            captured.WriteLine(IsParseRejection(exception)
                ? exception.Message : exception.ToString());
            outcome = (1, 0, 0);
        }
        finally
        {
            Console.SetError(originalError);
        }

        var stderr = captured.ToString();
        originalError.Write(stderr);
        var diagnostics = outcome.ExitCode == 0 ? string.Empty : stderr;
        _bundleParses.Add(new BundleParse(inputName, data.Skip(start).ToList(), diagnostics));
        return outcome;
    }

    private int RunInputBundle(string parserType, DateTime overallBefore)
    {
        var matcher = new BundleMemberGlob(config.BundleGlob);
        var names = new HashSet<string>(StringComparer.Ordinal);
        var parseData = new List<AntlrJson.ParsingResultSet>();
        var emitBundle = !config.NoOutput && !config.NoParsingResultSets;
        var temporaryPath = Path.Combine(Path.GetTempPath(),
            "trparse-" + Guid.NewGuid().ToString("N") + ".tar");
        using var temporary = new FileStream(temporaryPath, FileMode.CreateNew,
            FileAccess.ReadWrite, FileShare.None, 81920,
            FileOptions.SequentialScan | FileOptions.DeleteOnClose);
        using var input = Console.OpenStandardInput();
        using var reader = new TarReader(input);
        int result = 0;
        int rowNumber = 0;

        // The temporary output avoids publishing a partial bundle if a later
        // source member collides with a generated .pt or .errors member.
        using (var writer = emitBundle
            ? new TarWriter(temporary, TarEntryFormat.Pax, leaveOpen: true)
            : null)
        {
            TarEntry entry;
            while ((entry = reader.GetNextEntry(copyData: false)) != null)
            {
                var name = AntlrJson.ArtifactBundle.ValidateMemberName(entry.Name);
                if (!names.Add(name))
                    throw new InvalidDataException($"Duplicate bundle member '{name}'.");

                bool regular = entry.EntryType is TarEntryType.RegularFile or
                    TarEntryType.V7RegularFile;
                bool selected = regular && matcher.IsMatch(name);
                if (regular && !selected && writer == null)
                    continue;
                if (regular)
                {
                    using var sourceData = new MemoryStream();
                    entry.DataStream?.CopyTo(sourceData);
                    string sourceText = null;
                    if (selected)
                    {
                        sourceData.Position = 0;
                        using var source = new StreamReader(sourceData, Encoding.UTF8,
                            detectEncodingFromByteOrderMarks: true,
                            leaveOpen: true);
                        sourceText = source.ReadToEnd();
                    }
                    if (writer != null)
                    {
                        sourceData.Position = 0;
                        writer.WriteEntry(CopyRegularEntry(entry, name, sourceData));
                    }
                    if (selected)
                    {
                        var (r, _, _) = ParseOne(parserType, sourceText,
                            name, rowNumber++, parseData);
                        result = result == 0 ? r : result;
                    }
                    if (selected && writer != null)
                    {
                        var parse = _bundleParses[^1];
                        int resultCount = parse.SerializedResults?.Count ??
                            parse.Results.Count;
                        for (var index = 0; index < resultCount; index++)
                        {
                            var suffix = resultCount == 1 ? "" : $".{index + 1}";
                            WriteAddedMember(writer, names, name + suffix + ".pt",
                                parse.SerializedResults != null
                                    ? parse.SerializedResults[index]
                                    : AntlrJson.ArtifactBundle.SerializeParsingResult(
                                        parse.Results[index], config.Format));
                        }
                        WriteAddedMember(writer, names, name + ".errors",
                            new UTF8Encoding(false).GetBytes(parse.Diagnostics));
                    }
                    if (selected)
                    {
                        _bundleParses.Clear();
                        parseData.Clear();
                    }
                }
                else if (writer != null)
                {
                    writer.WriteEntry(new PaxTarEntry(entry));
                }
            }
        }

        PrintPerfSummary((DateTime.Now - overallBefore).TotalSeconds);
        if (emitBundle)
        {
            temporary.Position = 0;
            using var output = Console.OpenStandardOutput();
            temporary.CopyTo(output);
        }
        return result;
    }

    private static PaxTarEntry CopyRegularEntry(TarEntry source, string name,
        Stream data)
    {
        var copy = new PaxTarEntry(TarEntryType.RegularFile, name)
        {
            DataStream = data,
            Mode = source.Mode,
            ModificationTime = source.ModificationTime,
            Uid = source.Uid,
            Gid = source.Gid
        };
        if (source is PosixTarEntry posix)
        {
            copy.UserName = posix.UserName;
            copy.GroupName = posix.GroupName;
        }
        return copy;
    }

    private static void WriteAddedMember(TarWriter writer, HashSet<string> names,
        string name, byte[] data)
    {
        name = AntlrJson.ArtifactBundle.ValidateMemberName(name);
        if (!names.Add(name))
            throw new InvalidDataException($"Duplicate bundle member '{name}'.");
        writer.WriteEntry(new PaxTarEntry(TarEntryType.RegularFile, name)
        {
            DataStream = new MemoryStream(data, writable: false),
            ModificationTime = DateTimeOffset.UnixEpoch
        });
    }

    private void WriteBundle()
    {
        var paths = AntlrJson.ArtifactBundle.RelativeInputNames(
            _bundleParses.Select(parse => parse.InputName), config.BaseDirectory);
        var baseNames = AntlrJson.ArtifactBundle.ArtifactBaseNames(paths.Values);
        var artifacts = new List<AntlrJson.Artifact>();
        foreach (var parse in _bundleParses)
        {
            var baseName = baseNames[paths[parse.InputName]];
            int resultCount = parse.SerializedResults?.Count ?? parse.Results.Count;
            for (var index = 0; index < resultCount; index++)
            {
                var suffix = resultCount == 1 ? "" : $".{index + 1}";
                artifacts.Add(new AntlrJson.Artifact(
                    baseName + suffix + ".pt",
                    parse.SerializedResults != null
                        ? parse.SerializedResults[index]
                        : AntlrJson.ArtifactBundle.SerializeParsingResult(
                            parse.Results[index], config.Format)));
            }
            artifacts.Add(new AntlrJson.Artifact(
                baseName + ".errors", new UTF8Encoding(false).GetBytes(parse.Diagnostics)));
        }

        using var output = Console.OpenStandardOutput();
        AntlrJson.ArtifactBundle.Write(output, artifacts);
    }

    private void UpdateStats(double parseSeconds, long tokenCount)
    {
        if (_fileCount == 0)
        {
            _firstFileTokens = tokenCount;
            _firstFileParseSeconds = parseSeconds;
        }
        _totalParseSeconds += parseSeconds;
        _totalTokens += tokenCount;
        _fileCount++;
    }

    private void PrintPerfSummary(double overallSeconds)
    {
        if (Environment.GetEnvironmentVariable(TimeoutWorkerName) != null)
            return;
        if (config.Quiet) return;
        if (config.TimeoutSeconds > 0)
        {
            if (config.PerformanceSummary)
                Console.Error.WriteLine(
                    "Per-file timeout uses isolated processes; aggregate PT/PR is unavailable.");
            Console.Error.WriteLine("TT: " + overallSeconds);
            return;
        }
        if (config.InterpTimings && _interpTimings.Files > 0)
            System.Console.Error.WriteLine(_interpTimings.Format());
        if (!config.PerformanceSummary)
        {
            System.Console.Error.WriteLine("TT: " + overallSeconds);
            return;
        }
        var warmTokens = _totalTokens - _firstFileTokens;
        var warmSeconds = _totalParseSeconds - _firstFileParseSeconds;
        var warmTps = (_fileCount > 1 && warmSeconds > 0)
            ? ((long)(warmTokens / warmSeconds)).ToString()
            : "n.a.";
        var firstTps = _firstFileParseSeconds > 0 ? (_firstFileTokens / _firstFileParseSeconds) : 0;
        var speedup = (_fileCount > 1 && warmSeconds > 0 && firstTps > 0)
            ? ((warmTokens / warmSeconds) / firstTps).ToString("F2")
            : "n.a.";
        System.Console.Error.WriteLine("PT: " + _totalParseSeconds);
        System.Console.Error.WriteLine("OT: " + (overallSeconds - _totalParseSeconds));
        System.Console.Error.WriteLine("TT: " + overallSeconds);
        System.Console.Error.WriteLine("PR: " + (_totalParseSeconds > 0 ? (long)(_totalTokens / _totalParseSeconds) : 0));
        System.Console.Error.WriteLine("Post-warmup PR: " + warmTps);
        System.Console.Error.WriteLine("Post-warmup speed up: " + speedup);
    }

    (int ExitCode, double ParseSeconds, long TokenCount) DoParse(string parser_type,
        string txt,
        string prefix,
        string input_name,
        int row_number,
        List<AntlrJson.ParsingResultSet> data)
    {
        // Interp-file-based Earley parsing path
        bool hasPInterp = !string.IsNullOrEmpty(config.PInterp);
        bool hasLInterp = !string.IsNullOrEmpty(config.LInterp);
        if (hasPInterp != hasLInterp)
        {
            System.Console.Error.WriteLine(
                "Error: --pinterp and --linterp must be specified together.");
            return (1, 0, 0);
        }
        string resolvedPInterp = hasPInterp
            ? ResolveInterpPath(config.PInterp, config.Lib)
            : null;
        string resolvedLInterp = hasLInterp
            ? ResolveInterpPath(config.LInterp, config.Lib)
            : null;

        // Auto-discover interp files when --lib is given without explicit --pinterp/--linterp.
        if (resolvedPInterp == null && resolvedLInterp == null && !string.IsNullOrEmpty(config.Lib))
        {
            var discovered = DiscoverInterpPair(config.Lib);
            resolvedPInterp = discovered.pinterp;
            resolvedLInterp = discovered.linterp;
        }

        if (resolvedPInterp != null && resolvedLInterp != null)
        {
            bool grammarContextAwareLexing =
                RequiresContextAwareLexing(resolvedPInterp);
            bool contextAwareLexing = config.ContextAwareLexing ||
                grammarContextAwareLexing;
            DateTime interpBefore = DateTime.Now;
            AntlrJson.ParsingResultSet rs;
            long interpTokenCount;
            string interpLabel;
            if (!string.IsNullOrEmpty(config.XQueryHooks) && !config.AllStar)
                throw new ArgumentException("--xquery-hooks requires --allstar.");
            if (config.AllStar || contextAwareLexing ||
                config.IndirectLeftRecursion)
            {
                AllStarAtnParser.AllStarParser.Trace = config.Verbose;
                var interpTimings = config.InterpTimings
                    ? new AllStarAtnParser.InterpRunTimings()
                    : null;
                var parserStatistics = config.ParserStats
                    ? new AllStarAtnParser.ParserStatistics()
                    : null;
                (rs, interpTokenCount) = AllStarAtnParser.InterpRunner.Run(
                    resolvedPInterp, resolvedLInterp, txt, input_name,
                    config.LineNumbers, contextAwareLexing,
                    config.LexerStats, config.LexerOverlaps, interpTimings,
                    parserStatistics, _parserPredictionCache,
                    _interpRuntimeCache, _lexerDfaCache,
                    config.IndirectLeftRecursion, config.StartRule,
                    config.XQueryHooks);
                if (interpTimings != null)
                    _interpTimings.Add(interpTimings);
                interpLabel = "ALL(*)";
            }
            else
            {
                (rs, interpTokenCount) = EarleyAtnParser.InterpRunner.Run(
                    resolvedPInterp, resolvedLInterp, txt, input_name,
                    config.LineNumbers, config.LexerStats, config.LexerOverlaps,
                    _interpRuntimeCache, _lexerDfaCache, config.StartRule);
                interpLabel = "Earley";
            }
            DateTime interpAfter = DateTime.Now;
            double interpParseSeconds = (interpAfter - interpBefore).TotalSeconds;
            data.Add(rs);
            if (!config.Quiet && config.PerFilePerformance)
            {
                long pr = interpParseSeconds > 0 ? (long)(interpTokenCount / interpParseSeconds) : 0L;
                System.Console.Error.WriteLine(prefix + interpLabel + " " + row_number + " " + input_name + " success "
                    + interpParseSeconds + " s " + interpTokenCount + " tokens " + pr + " pr");
            }
            UpdateStats(interpParseSeconds, interpTokenCount);
            return (0, interpParseSeconds, interpTokenCount);
        }

        if (!string.IsNullOrEmpty(config.StartRule))
            throw new ArgumentException(
                "--start-rule requires interpreted parsing with parser and lexer .interp files.");

        Type type = null;
        if (parser_type == null || parser_type == "")
        {
            var extension = Path.GetExtension(input_name);
            // There are two choices:
            // If a Generated-CSharp directory exists, use that.
            // If the directory does not exist, pick based on
            // file extension.
            parser_type = extension switch
            {
                ".g4" => "ANTLRv4",
                ".g4x" => "G4X",
                ".g3" => "ANTLRv3",
                ".g2" => "ANTLRv2",
                ".peg" => "pegen_v3_10",
                ".rex" => "rex",
                ".y" => "Bison",
                ".lark" => "Lark",
                ".cf" => "LBNF",
                ".ebnf" => "W3CEBNF",
                ".xtext" => "Xtext",
                ".jj" => "Javacc",
                ".jjt" => "Javacc",
                ".abnf" => "ABNF",
                ".iso14977" => "Iso14977",
                ".iso" => "Iso14977",
                ".pegjs" => "Pegjs",
                ".pest" => "Pest",
                ".ixml" => "ixml",
                _ => null
            };
            var subdir = parser_type switch
            {
                "ANTLRv4" => "antlr4",
                "G4X" => "g4x",
                "ANTLRv3" => "antlr3",
                "ANTLRv2" => "antlr2",
                "pegen_v3_10" => "pegen",
                "rex" => "rex",
                "Bison" => "bison",
                "Lark" => "lark",
                "LBNF" => "lbnf",
                "W3CEBNF" => "w3cebnf",
                "Xtext" => "xtext",
                "Javacc" => "javacc",
                "ABNF" => "abnf",
                "Iso14977" => "iso14977",
                "Pegjs" => "pegjs",
                "Pest" => "pest",
                "Grammophone" => "grammophone",
                "Princeton" => "princeton",
                "ixml" => "ixml",
                _ => null
            };
            if (subdir != null)
            {
                // Get this assembly.
                System.Reflection.Assembly a = this.GetType().Assembly;
                string path = a.Location;
                path = Path.GetDirectoryName(path);
                path = path.Replace("\\", "/");
                if (!path.EndsWith("/")) path = path + "/";
                var full_path = path;
                var exists = File.Exists(full_path + subdir + ".dll");
                full_path = Path.GetFullPath(full_path);
                Assembly asm = Assembly.LoadFile(full_path + subdir + ".dll");
                Type[] types = asm.GetTypes();
                type = asm.GetType("Program");
            }
            else
            {
                string path = config.ParserLocation != null
                    ? config.ParserLocation
                    : Environment.CurrentDirectory + Path.DirectorySeparatorChar;
                path = path.Replace("\\", "/");
                if (!path.EndsWith("/")) path = path + "/";
                var full_path = path + "Generated-CSharp/bin/Debug/net10.0/";
                var exists = File.Exists(full_path + "Test.dll");
                if (!exists) full_path = path + "bin/Debug/net10.0/";
                exists = File.Exists(full_path + "Test.dll");
                if (!exists) full_path = path + "Generated-CSharp/bin/Release/net10.0/";
                exists = File.Exists(full_path + "Test.dll");
                if (!exists) full_path = path + "bin/Release/net10.0/";
                exists = File.Exists(full_path + "Test.dll");
                if (exists)
                {
                    full_path = Path.GetFullPath(full_path);
                    Assembly asm = Assembly.LoadFile(full_path + config.Dll + ".dll");
                    Type[] types = asm.GetTypes();
                    type = asm.GetType("Program");
                }
            }
        }
        else if (parser_type == "gen")
        {
            string path = config.ParserLocation != null
                ? config.ParserLocation
                : Environment.CurrentDirectory + Path.DirectorySeparatorChar;
            path = path.Replace("\\", "/");
            if (!path.EndsWith("/")) path = path + "/";
            var candidates = new[]
            {
                path + "Generated-CSharp/bin/Debug/net10.0/",
                path + "bin/Debug/net10.0/",
                path + "Generated-CSharp/bin/Release/net10.0/",
                path + "bin/Release/net10.0/",
            };
            string found_path = null;
            foreach (var candidate in candidates)
            {
                if (File.Exists(candidate + config.Dll + ".dll"))
                {
                    found_path = Path.GetFullPath(candidate);
                    break;
                }
            }
            if (found_path != null)
            {
                Assembly asm = Assembly.LoadFile(found_path + config.Dll + ".dll");
                type = asm.GetType("Program");
            }
            else
            {
                System.Console.Error.WriteLine(
                    "gen: could not find '" + config.Dll + ".dll' in any of:");
                foreach (var c in candidates)
                    System.Console.Error.WriteLine("  " + Path.GetFullPath(c));
                System.Console.Error.WriteLine(
                    "Did you run 'dotnet build' in the Generated-CSharp directory?");
            }
        }
        else
        {
            System.Console.Error.WriteLine("Using built-in parser.");
            var subdir = parser_type switch
            {
                "ANTLRv4" => "antlr4",
                "G4X" => "g4x",
                "ANTLRv3" => "antlr3",
                "ANTLRv2" => "antlr2",
                "pegen_v3_10" => "pegen",
                "rex" => "rex",
                "Bison" => "bison",
                "Lark" => "lark",
                "LBNF" => "lbnf",
                "W3CEBNF" => "w3cebnf",
                "Xtext" => "xtext",
                "Javacc" => "javacc",
                "ABNF" => "abnf",
                "Iso14977" => "iso14977",
                "Pegjs" => "pegjs",
                "Pest" => "pest",
                "Grammophone" => "grammophone",
                "Princeton" => "princeton",
                "ixml" => "ixml",
                _ => throw new Exception(
                    "Unknown built-in parser type. Supported: G4X, ANTLRv4, ANTLRv3, ANTLRv2, Bison, Lark, rex, pegen_v3_10, LBNF, W3CEBNF, Xtext, Javacc, ABNF, Iso14977, Pegjs, Pest, Grammophone, Princeton, ixml, gen.")
            };
            // Get this assembly.
            System.Reflection.Assembly a = this.GetType().Assembly;
            string path = a.Location;
            path = Path.GetDirectoryName(path);
            path = path.Replace("\\", "/");
            if (!path.EndsWith("/")) path = path + "/";
            var full_path = path;
            var exists = File.Exists(full_path + subdir + ".dll");
            full_path = Path.GetFullPath(full_path);
            Assembly asm = Assembly.LoadFile(full_path + subdir + ".dll");
            Type[] types = asm.GetTypes();
            type = asm.GetType("Program");
        }

        if (type == null)
        {
            System.Console.Error.WriteLine(
                "No parser found for input '" + input_name + "'. " +
                "Specify a grammar type with -t, point to a generated parser with -p, " +
                "or use --pinterp / --linterp for Earley ATN-based parsing.");
            return (1, 0, 0);
        }

        MethodInfo methodInfo = type.GetMethod("SetupParse2");
        object[] parm1 = new object[] { txt, input_name, config.Quiet };
        var res = methodInfo.Invoke(null, parm1);

        var result = "";
        object res2 = null;
        DateTime before = DateTime.Now;
        DateTime after = DateTime.Now;
        {
            MethodInfo methodInfo2 = type.GetMethod("Parse2");
            object[] parm2 = new object[] { };
            before = DateTime.Now;
            res2 = methodInfo2.Invoke(null, parm2);
            after = DateTime.Now;

            MethodInfo methodInfo3 = type.GetMethod("AnyErrors");
            object[] parm3 = new object[] { };
            var res3 = methodInfo3.Invoke(null, parm3);
            if ((bool)res3)
            {
                result = "fail";
            }
            else
            {
                result = "success";
            }
        }

        double parseSeconds = (after - before).TotalSeconds;
        var parser = type.GetProperty("Parser").GetValue(null, new object[0]) as Antlr4.Runtime.Parser;
        var lexer = type.GetProperty("Lexer").GetValue(null, new object[0]) as Antlr4.Runtime.Lexer;
        var tokstream = type.GetProperty("TokenStream").GetValue(null, new object[0]) as ITokenStream;
        var charstream = type.GetProperty("CharStream").GetValue(null, new object[0]) as ICharStream;
        var commontokstream = tokstream as CommonTokenStream;
        long tokenCount = commontokstream != null ? (long)commontokstream.Size : 0L;
        var r5 = type.GetProperty("Input").GetValue(null, new object[0]);

        if (!config.Quiet && config.PerFilePerformance)
        {
            long pr = parseSeconds > 0 ? (long)(tokenCount / parseSeconds) : 0L;
            System.Console.Error.WriteLine(prefix + "CSharp " + row_number + " " + input_name + " " + result + " "
                + parseSeconds + " s " + tokenCount + " tokens " + pr + " pr");
        }

        {
            var tuples = res2 as List<Tuple<string, IParseTree>>;
            // Each ambiguous parse tree is for an alt.
            // Two ways to group this:
            // 1) All trees under one file, one decision.
            // 2) Or, each tree under one file, one decision, one alt.
            if (config.GroupBy)
            {
                string decision_str = "";
                var list_of_trees = new List<UnvParseTreeNode>();
                foreach (var tt in tuples)
                {
                    var t1 = tt.Item1 as string;
                    decision_str = t1;
                    var t2 = tt.Item2 as ParserRuleContext;
                    var converted_tree =
                        new ConvertToDOM(config.LineNumbers).BottomUpConvert(t2, null, parser, lexer, commontokstream);
                    list_of_trees.Add(converted_tree);
                }
                var tuple = new AntlrJson.ParsingResultSet()
                {
                    FileName = input_name + "." + decision_str,
                    Nodes = list_of_trees.ToArray(),
                    Parser = parser,
                    Lexer = lexer
                };
                data.Add(tuple);
            }
            else
            {
                foreach (var tt in tuples)
                {
                    var list_of_trees = new List<UnvParseTreeNode>();
                    var t1 = tt.Item1 as string;
                    var t2 = tt.Item2 as ParserRuleContext;
                    var converted_tree =
                        new ConvertToDOM(config.LineNumbers).BottomUpConvert(t2, null, parser, lexer, commontokstream);
                    var tuple = new AntlrJson.ParsingResultSet()
                    {
                        FileName = input_name + (tt.Item1 != null ? "." + tt.Item1 : ""),
                        Nodes = new UnvParseTreeNode[] { converted_tree },
                        Parser = parser,
                        Lexer = lexer
                    };
                    data.Add(tuple);
                }
            }
        }

        UpdateStats(parseSeconds, tokenCount);
        return (result == "success" ? 0 : 1, parseSeconds, tokenCount);
    }

    private static string ResolveInterpPath(string path, string lib)
    {
        if (string.IsNullOrEmpty(lib) || Path.IsPathRooted(path))
            return path;
        return Path.Combine(lib, path);
    }

    /// <summary>
    /// Scans <paramref name="dir"/> for matched parser + lexer .interp pairs.
    /// Handles both naming conventions produced by trinterp:
    ///   • Combined grammar "Foo"  → Foo.interp + FooLexer.interp
    ///   • Explicit grammars "FooParser"/"FooLexer" → FooParser.interp + FooLexer.interp
    /// Returns the pair when exactly one match is found; throws otherwise.
    /// </summary>
    private static (string pinterp, string linterp) DiscoverInterpPair(string dir)
    {
        if (!Directory.Exists(dir))
            throw new DirectoryNotFoundException($"--lib directory not found: '{dir}'");

        var pairs = new List<(string p, string l)>();
        foreach (var pf in Directory.GetFiles(dir, "*.interp"))
        {
            var stem = Path.GetFileNameWithoutExtension(pf);
            if (stem.EndsWith("Lexer")) continue; // skip lexer files

            // Case 1: combined grammar — Foo.interp pairs with FooLexer.interp
            var lf = Path.Combine(dir, stem + "Lexer.interp");
            if (File.Exists(lf)) { pairs.Add((pf, lf)); continue; }

            // Case 2: explicit parser grammar — FooParser.interp pairs with FooLexer.interp
            if (stem.EndsWith("Parser"))
            {
                var baseName = stem.Substring(0, stem.Length - "Parser".Length);
                lf = Path.Combine(dir, baseName + "Lexer.interp");
                if (File.Exists(lf)) pairs.Add((pf, lf));
            }
        }

        if (pairs.Count == 0)
            throw new FileNotFoundException(
                $"No matched parser/lexer .interp pair found in '{dir}'. " +
                "Use --pinterp / --linterp to specify them explicitly.");
        if (pairs.Count > 1)
            throw new InvalidOperationException(
                $"Multiple parser/lexer .interp pairs found in '{dir}': " +
                string.Join(", ", pairs.Select(p => Path.GetFileNameWithoutExtension(p.p))) +
                ". Use --pinterp / --linterp to specify which pair to use.");

        return pairs[0];
    }
}
