using System.Text.Json;

namespace Trash;

internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length > 1 || (args.Length == 1 && args[0] is not "--json" and not "--help" and not "-h"))
        {
            Console.Error.WriteLine("Usage: dotnet trash machine [--json]");
            return 2;
        }

        if (args.Length == 1 && args[0] is "--help" or "-h")
        {
            Console.WriteLine("Usage: dotnet trash machine [--json]");
            Console.WriteLine("Print hardware and OS details for reproducible performance reports.");
            return 0;
        }

        var report = MachineReport.Collect();
        if (args.Length == 1)
            Console.WriteLine(JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        else
            report.Print(Console.Out);
        return 0;
    }
}
