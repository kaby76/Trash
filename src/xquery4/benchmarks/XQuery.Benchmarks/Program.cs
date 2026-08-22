using System.Diagnostics;
using XQuery.DataModel;
using XQuery.Engine;
using XQuery.Parser;

const int warmupIterations = 5;
const int iterations = 20;
int[] sizes = [1_000, 2_000, 4_000, 8_000];
var expression = new XPathParser("(/root/item | /root/item)").Parse();

Console.WriteLine("XPath4 overlapping-union scaling benchmark");
Console.WriteLine($"Iterations per size: {iterations}");
Console.WriteLine("Nodes\tElapsed (ms)\tns/node");

foreach (var size in sizes)
{
    var document = BuildDocument(size);
    var evaluator = new XPathEvaluator();

    // Warm the JIT and the document-order index outside the measurement.
    for (var iteration = 0; iteration < warmupIterations; iteration++)
    {
        var warmup = evaluator.Evaluate(expression, document);
        if (warmup.Count != size)
            throw new InvalidOperationException($"Expected {size} nodes, got {warmup.Count}.");
    }

    var stopwatch = Stopwatch.StartNew();
    for (var iteration = 0; iteration < iterations; iteration++)
    {
        var result = evaluator.Evaluate(expression, document);
        if (result.Count != size)
            throw new InvalidOperationException($"Expected {size} nodes, got {result.Count}.");
    }
    stopwatch.Stop();

    var elapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
    var nanosecondsPerNode = stopwatch.Elapsed.TotalNanoseconds / (size * iterations);
    Console.WriteLine($"{size}\t{elapsedMilliseconds:F3}\t\t{nanosecondsPerNode:F1}");
}

static XdmDocument BuildDocument(int childCount)
{
    var document = new XdmDocument();
    var root = new XdmElement("root");
    document.AppendChild(root);
    for (var i = 0; i < childCount; i++)
        root.AppendChild(new XdmElement("item"));
    return document;
}
