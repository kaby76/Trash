using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using AllStarAtnParser;
using Atn;
using EarleyAtnParser;

namespace Trash;

internal sealed class DfaCacheLoadException : IOException
{
    public DfaCacheLoadException(string message, Exception inner)
        : base(message, inner) { }
}

/// <summary>A versioned snapshot of learned interpreter DFAs, tied to both .interp files.</summary>
internal static class DfaCacheFile
{
    private const int Version = 2;
    private const string Magic = "TrashDFA";

    public static byte[] Write(string parserInterp, string lexerInterp,
        ParserPredictionCache parser, LexerAtnSimulator.LexerDfaCache lexer)
    {
        if (lexer._atn == null)
            throw new InvalidOperationException("No interpreted lexer has been parsed yet.");
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
        writer.Write(Magic);
        writer.Write(Version);
        writer.Write(SHA256.HashData(File.ReadAllBytes(parserInterp)));
        writer.Write(SHA256.HashData(File.ReadAllBytes(lexerInterp)));
        // A saturated parser cache cannot be reused by the current simulator.
        bool hasParser = parser?.BoundAtn != null &&
            parser.SharingEnabled && !parser.IsSaturated;
        writer.Write(hasParser);
        if (hasParser) WriteSection(writer, parser.Write);
        WriteSection(writer, lexer.Write);
        writer.Flush();
        return stream.ToArray();
    }

    public static TimeSpan Read(byte[] data, string parserInterp,
        string lexerInterp, Atn.MyATN parserAtn, int[] serializedParserAtn,
        Atn.MyATN lexerAtn, ParserPredictionCache parser,
        LexerAtnSimulator.LexerDfaCache lexer)
    {
        try
        {
            return ReadCore(data, parserInterp, lexerInterp, parserAtn,
                serializedParserAtn, lexerAtn, parser, lexer);
        }
        catch (Exception exception) when (exception is InvalidDataException or
            EndOfStreamException or ArgumentException or InvalidOperationException)
        {
            throw new DfaCacheLoadException(
                "Could not load interpreter DFA cache: " + exception.Message,
                exception);
        }
    }

    private static TimeSpan ReadCore(byte[] data, string parserInterp,
        string lexerInterp, Atn.MyATN parserAtn, int[] serializedParserAtn,
        Atn.MyATN lexerAtn, ParserPredictionCache parser,
        LexerAtnSimulator.LexerDfaCache lexer)
    {
        var timer = Stopwatch.StartNew();
        using var stream = new MemoryStream(data, writable: false);
        using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
        if (reader.ReadString() != Magic || reader.ReadInt32() != Version)
            throw new InvalidDataException("Unsupported interpreter DFA cache format.");
        var parserHash = reader.ReadBytes(32);
        var lexerHash = reader.ReadBytes(32);
        if (parserHash.Length != 32 || lexerHash.Length != 32 ||
            !parserHash.AsSpan().SequenceEqual(SHA256.HashData(
                File.ReadAllBytes(parserInterp))) ||
            !lexerHash.AsSpan().SequenceEqual(SHA256.HashData(
                File.ReadAllBytes(lexerInterp))))
            throw new InvalidDataException(
                "Interpreter DFA cache does not match the parser and lexer .interp files.");
        bool hasParser = reader.ReadBoolean();
        if (hasParser)
        {
            var parserData = ReadSection(reader);
            if (parser != null)
                ParseSection(parserData, section =>
                    parser.Read(section, parserAtn, serializedParserAtn));
        }
        ParseSection(ReadSection(reader), section => lexer.Read(section, lexerAtn));
        if (stream.Position != stream.Length)
            throw new InvalidDataException("Trailing data in interpreter DFA cache.");
        timer.Stop();
        return timer.Elapsed;
    }

    private static void WriteSection(BinaryWriter writer, Action<BinaryWriter> write)
    {
        using var stream = new MemoryStream();
        using (var section = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
            write(section);
        writer.Write(checked((int)stream.Length));
        writer.Write(stream.GetBuffer(), 0, checked((int)stream.Length));
    }

    private static byte[] ReadSection(BinaryReader reader)
    {
        int size = reader.ReadInt32();
        if (size < 0 || size > 512 * 1024 * 1024)
            throw new InvalidDataException("Invalid interpreter DFA cache section length.");
        var section = reader.ReadBytes(size);
        if (section.Length != size)
            throw new EndOfStreamException("Truncated interpreter DFA cache section.");
        return section;
    }

    private static void ParseSection(byte[] data, Action<BinaryReader> parse)
    {
        using var stream = new MemoryStream(data, writable: false);
        using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
        parse(reader);
        if (stream.Position != stream.Length)
            throw new InvalidDataException("Trailing data in interpreter DFA cache section.");
    }
}
