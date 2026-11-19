using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Trash;

public sealed class MachineReport
{
    public string Os { get; set; } = RuntimeInformation.OSDescription;
    public string OsVersion { get; set; } = Environment.OSVersion.Version.ToString();
    public string Architecture { get; set; } = RuntimeInformation.OSArchitecture.ToString();
    public string DotnetRuntime { get; set; } = RuntimeInformation.FrameworkDescription;
    public CpuDetails Cpu { get; set; } = new();
    public MemoryDetails Memory { get; set; } = new();
    public MotherboardDetails Motherboard { get; set; } = new();
    public List<DiskDetails> Disks { get; set; } = new();
    public List<GpuDetails> Gpus { get; set; } = new();
    public List<string> Notes { get; set; } = new();

    public static MachineReport Collect()
    {
        var report = new MachineReport();
        report.Cpu.LogicalProcessors = Environment.ProcessorCount;
        try
        {
            if (OperatingSystem.IsWindows()) CollectWindows(report);
            else if (OperatingSystem.IsLinux()) CollectLinux(report);
            else if (OperatingSystem.IsMacOS()) CollectMac(report);
            else report.Notes.Add("Hardware collection is unavailable on this operating system.");
        }
        catch (Exception ex)
        {
            report.Notes.Add($"Some hardware details could not be read: {ex.Message}");
        }
        return report;
    }

    public void Print(TextWriter writer)
    {
        static string Show(string? value) => string.IsNullOrWhiteSpace(value) ? "unknown" : value;
        static string Bytes(long? value) => value is null ? "unknown" : $"{value.Value / 1073741824.0:F2} GiB ({value.Value:N0} bytes)";
        writer.WriteLine($"OS: {Show(Os)}");
        writer.WriteLine($"OS version: {Show(OsVersion)}");
        writer.WriteLine($"Architecture: {Show(Architecture)}");
        writer.WriteLine($".NET runtime: {Show(DotnetRuntime)}");
        writer.WriteLine($"CPU: {Show(Cpu.Model)}");
        writer.WriteLine($"CPU vendor: {Show(Cpu.Vendor)}");
        writer.WriteLine($"CPU logical processors: {Cpu.LogicalProcessors}");
        writer.WriteLine($"CPU physical cores: {Cpu.PhysicalCores?.ToString() ?? "unknown"}");
        writer.WriteLine($"CPU clock: {(Cpu.ClockMHz is double mhz ? $"{mhz:F0} MHz ({Show(Cpu.ClockKind)})" : "unknown")}");
        writer.WriteLine($"RAM total: {Bytes(Memory.TotalBytes)}");
        writer.WriteLine($"RAM available now: {Bytes(Memory.AvailableBytes)}");
        writer.WriteLine($"Motherboard: {Show(Motherboard.Manufacturer)} {Show(Motherboard.Product)}".Trim());
        writer.WriteLine($"Motherboard version: {Show(Motherboard.Version)}");
        if (Disks.Count == 0) writer.WriteLine("Disks: unknown");
        foreach (var disk in Disks)
            writer.WriteLine($"Disk: {Show(disk.Name)} | {Show(disk.Model)} | {Show(disk.Type)} | {Bytes(disk.SizeBytes)}");
        if (Gpus.Count == 0) writer.WriteLine("GPU: unknown or unavailable");
        foreach (var gpu in Gpus)
            writer.WriteLine($"GPU: {Show(gpu.Name)}{(string.IsNullOrWhiteSpace(gpu.Vendor) ? "" : $" ({gpu.Vendor})")}");
        foreach (var note in Notes) writer.WriteLine($"Note: {note}");
    }

    private static void CollectWindows(MachineReport report)
    {
        // CIM supplies rated clock, physical memory, and device metadata. An empty
        // or failed query leaves the corresponding field unknown.
        const string query = """
            $ErrorActionPreference = 'SilentlyContinue'
            $cpu = @(Get-CimInstance Win32_Processor | Select-Object Name,Manufacturer,NumberOfLogicalProcessors,NumberOfCores,MaxClockSpeed)
            $os = Get-CimInstance Win32_OperatingSystem | Select-Object Caption,Version,FreePhysicalMemory
            $computer = Get-CimInstance Win32_ComputerSystem | Select-Object TotalPhysicalMemory
            $board = Get-CimInstance Win32_BaseBoard | Select-Object -First 1 Manufacturer,Product,Version
            $disks = @(Get-PhysicalDisk | Select-Object FriendlyName,MediaType,BusType,Size)
            if ($disks.Count -eq 0) { $disks = @(Get-CimInstance Win32_DiskDrive | Select-Object @{N='FriendlyName';E={$_.Model}},@{N='MediaType';E={'Unknown'}},@{N='BusType';E={$_.InterfaceType}},Size) }
            $gpus = @(Get-CimInstance Win32_VideoController | Select-Object Name,AdapterCompatibility)
            [pscustomobject]@{cpu=$cpu;os=$os;computer=$computer;board=$board;disks=$disks;gpus=$gpus} | ConvertTo-Json -Compress -Depth 5
            """;
        var output = Run("powershell.exe", ["-NoProfile", "-NonInteractive", "-Command", query], 30000);
        if (output is null)
        {
            report.Notes.Add("Windows CIM queries were unavailable.");
            return;
        }
        using var doc = JsonDocument.Parse(output);
        var root = doc.RootElement;
        var cpus = Items(root, "cpu").ToArray();
        report.Cpu.Model = string.Join("; ", cpus.Select(x => String(x, "Name")?.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct());
        report.Cpu.Vendor = cpus.Select(x => String(x, "Manufacturer")).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
        if (cpus.Length > 0)
        {
            report.Cpu.LogicalProcessors = cpus.Sum(x => Number(x, "NumberOfLogicalProcessors") ?? 0) is var logical && logical > 0 ? (int)logical : report.Cpu.LogicalProcessors;
            var cores = cpus.Sum(x => Number(x, "NumberOfCores") ?? 0);
            if (cores > 0) report.Cpu.PhysicalCores = (int)cores;
            var maxClock = cpus.Max(x => Number(x, "MaxClockSpeed") ?? 0);
            if (maxClock > 0) { report.Cpu.ClockMHz = maxClock; report.Cpu.ClockKind = "rated maximum"; }
        }
        var os = Property(root, "os");
        report.Os = String(os, "Caption") ?? report.Os;
        report.OsVersion = String(os, "Version") ?? report.OsVersion;
        report.Memory.TotalBytes = Number(Property(root, "computer"), "TotalPhysicalMemory");
        var freeKb = Number(os, "FreePhysicalMemory");
        if (freeKb is > 0) report.Memory.AvailableBytes = freeKb * 1024;
        var board = Property(root, "board");
        report.Motherboard.Manufacturer = String(board, "Manufacturer");
        report.Motherboard.Product = String(board, "Product");
        report.Motherboard.Version = String(board, "Version");
        foreach (var disk in Items(root, "disks"))
        {
            var media = String(disk, "MediaType");
            var bus = String(disk, "BusType");
            var type = bus?.Contains("NVMe", StringComparison.OrdinalIgnoreCase) == true ? "NVMe SSD" :
                media is "SSD" or "HDD" ? media : "unknown";
            report.Disks.Add(new DiskDetails { Name = String(disk, "FriendlyName"), Model = null, Type = type, SizeBytes = Number(disk, "Size") });
        }
        foreach (var gpu in Items(root, "gpus"))
            report.Gpus.Add(new GpuDetails { Name = String(gpu, "Name"), Vendor = String(gpu, "AdapterCompatibility") });
    }

    private static void CollectLinux(MachineReport report)
    {
        report.Os = OsRelease() ?? report.Os;
        report.OsVersion = Read("/proc/sys/kernel/osrelease") ?? report.OsVersion;
        var cpuinfo = File.Exists("/proc/cpuinfo") ? File.ReadAllText("/proc/cpuinfo") : "";
        report.Cpu.Model = Match(cpuinfo, @"(?m)^model name\s*:\s*(.+)$") ?? Match(cpuinfo, @"(?m)^Hardware\s*:\s*(.+)$");
        report.Cpu.Vendor = Match(cpuinfo, @"(?m)^vendor_id\s*:\s*(.+)$");
        var physicalCores = Regex.Split(cpuinfo, @"\r?\n\r?\n")
            .Select(block => (Socket: Match(block, @"(?m)^physical id\s*:\s*(\d+)$"), Core: Match(block, @"(?m)^core id\s*:\s*(\d+)$")))
            .Where(pair => pair.Socket is not null && pair.Core is not null)
            .Distinct().Count();
        if (physicalCores > 0) report.Cpu.PhysicalCores = physicalCores;
        else if (int.TryParse(Match(cpuinfo, @"(?m)^cpu cores\s*:\s*(\d+)"), out var cores)) report.Cpu.PhysicalCores = cores;
        var maxKhz = Read("/sys/devices/system/cpu/cpu0/cpufreq/cpuinfo_max_freq");
        if (double.TryParse(maxKhz, CultureInfo.InvariantCulture, out var khz))
        {
            report.Cpu.ClockMHz = khz / 1000;
            report.Cpu.ClockKind = "reported maximum";
        }
        else if (double.TryParse(Match(cpuinfo, @"(?m)^cpu MHz\s*:\s*([\d.]+)"), CultureInfo.InvariantCulture, out var current))
        {
            report.Cpu.ClockMHz = current;
            report.Cpu.ClockKind = "current at sampling";
        }
        var meminfo = File.Exists("/proc/meminfo") ? File.ReadAllText("/proc/meminfo") : "";
        if (long.TryParse(Match(meminfo, @"(?m)^MemTotal:\s*(\d+)"), out var total)) report.Memory.TotalBytes = total * 1024;
        if (long.TryParse(Match(meminfo, @"(?m)^MemAvailable:\s*(\d+)"), out var available)) report.Memory.AvailableBytes = available * 1024;
        report.Motherboard.Manufacturer = Read("/sys/class/dmi/id/board_vendor");
        report.Motherboard.Product = Read("/sys/class/dmi/id/board_name");
        report.Motherboard.Version = Read("/sys/class/dmi/id/board_version");
        const string block = "/sys/block";
        if (Directory.Exists(block))
            foreach (var path in Directory.EnumerateDirectories(block).OrderBy(x => x))
            {
                var name = Path.GetFileName(path);
                if (!Regex.IsMatch(name, @"^(sd[a-z]+|hd[a-z]+|vd[a-z]+|xvd[a-z]+|nvme\d+n\d+|mmcblk\d+)$")) continue;
                var rotational = Read(Path.Combine(path, "queue/rotational"));
                var type = name.StartsWith("nvme", StringComparison.Ordinal) ? "NVMe SSD" : rotational switch { "0" => "SSD", "1" => "HDD", _ => "unknown" };
                var size = long.TryParse(Read(Path.Combine(path, "size")), out var sectors) ? sectors * 512 : (long?)null;
                report.Disks.Add(new DiskDetails { Name = name, Model = Read(Path.Combine(path, "device/model")), Type = type, SizeBytes = size });
            }
        var pci = Run("lspci", ["-mm"], 5000);
        if (pci is not null)
            foreach (var line in pci.Split('\n'))
                if (line.Contains("VGA compatible controller", StringComparison.OrdinalIgnoreCase) || line.Contains("3D controller", StringComparison.OrdinalIgnoreCase) || line.Contains("Display controller", StringComparison.OrdinalIgnoreCase))
                    report.Gpus.Add(new GpuDetails { Name = line.Trim() });
        if (report.Gpus.Count == 0 && Directory.Exists("/sys/class/drm"))
            foreach (var card in Directory.EnumerateDirectories("/sys/class/drm", "card*"))
                if (Regex.IsMatch(Path.GetFileName(card), @"^card\d+$") && Read(Path.Combine(card, "device/vendor")) is { } vendor)
                    report.Gpus.Add(new GpuDetails { Name = Path.GetFileName(card), Vendor = vendor });
    }

    private static void CollectMac(MachineReport report)
    {
        report.Cpu.Model = Run("sysctl", ["-n", "machdep.cpu.brand_string"], 5000)?.Trim();
        report.Cpu.Vendor = Run("sysctl", ["-n", "machdep.cpu.vendor"], 5000)?.Trim();
        if (int.TryParse(Run("sysctl", ["-n", "hw.physicalcpu"], 5000), out var cores)) report.Cpu.PhysicalCores = cores;
        if (long.TryParse(Run("sysctl", ["-n", "hw.cpufrequency"], 5000), out var hz))
        {
            report.Cpu.ClockMHz = hz / 1_000_000.0;
            report.Cpu.ClockKind = "nominal";
        }
        if (long.TryParse(Run("sysctl", ["-n", "hw.memsize"], 5000), out var mem)) report.Memory.TotalBytes = mem;
        report.Motherboard.Product = Run("sysctl", ["-n", "hw.model"], 5000)?.Trim();
        var vm = Run("vm_stat", [], 5000);
        if (vm is not null)
        {
            var pageSize = long.TryParse(Match(vm, @"page size of (\d+) bytes"), out var bytesPerPage) ? bytesPerPage : 4096;
            long pages = 0;
            foreach (var label in new[] { "Pages free", "Pages inactive", "Pages speculative" })
                if (long.TryParse(Match(vm, @"(?m)^" + label + @":\s*(\d+)"), out var count)) pages += count;
            if (pages > 0)
            {
                report.Memory.AvailableBytes = pages * pageSize;
                report.Notes.Add("Available RAM on macOS is an estimate from free, inactive, and speculative pages.");
            }
        }
        var gpuJson = Run("system_profiler", ["SPDisplaysDataType", "-json"], 15000);
        if (gpuJson is not null)
            try
            {
                using var doc = JsonDocument.Parse(gpuJson);
                foreach (var gpu in Items(doc.RootElement, "SPDisplaysDataType"))
                    report.Gpus.Add(new GpuDetails { Name = String(gpu, "sppci_model") ?? String(gpu, "_name"), Vendor = String(gpu, "spdisplays_vendor") });
            }
            catch (JsonException) { report.Notes.Add("GPU information could not be decoded."); }
        var disk = Run("diskutil", ["info", "/"], 10000);
        if (disk is not null)
        {
            var solid = Match(disk, @"(?m)^\s*Solid State:\s*(Yes|No)");
            var size = Match(disk, @"(?m)^\s*Disk Size:\s*[^(]*\(([\d,]+) Bytes\)");
            report.Disks.Add(new DiskDetails
            {
                Name = "system volume",
                Type = solid switch { "Yes" => "SSD", "No" => "HDD", _ => "unknown" },
                SizeBytes = long.TryParse(size?.Replace(",", ""), out var bytes) ? bytes : null
            });
        }
    }

    private static string? OsRelease()
    {
        if (!File.Exists("/etc/os-release")) return null;
        var line = File.ReadLines("/etc/os-release").FirstOrDefault(x => x.StartsWith("PRETTY_NAME=", StringComparison.Ordinal));
        return line is null ? null : line["PRETTY_NAME=".Length..].Trim('"');
    }

    private static string? Read(string path)
    {
        try { return File.Exists(path) ? File.ReadAllText(path).Trim() : null; }
        catch (IOException) { return null; }
        catch (UnauthorizedAccessException) { return null; }
    }

    private static string? Match(string text, string pattern)
    {
        var match = Regex.Match(text, pattern);
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }

    private static string? Run(string file, IEnumerable<string> args, int timeoutMs)
    {
        try
        {
            var start = new ProcessStartInfo(file) { RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true };
            foreach (var arg in args) start.ArgumentList.Add(arg);
            using var process = Process.Start(start);
            if (process is null) return null;
            var read = process.StandardOutput.ReadToEndAsync();
            if (!process.WaitForExit(timeoutMs)) { process.Kill(entireProcessTree: true); return null; }
            return process.ExitCode == 0 ? read.GetAwaiter().GetResult().Trim() : null;
        }
        catch (System.ComponentModel.Win32Exception) { return null; }
        catch (InvalidOperationException) { return null; }
    }

    private static JsonElement Property(JsonElement parent, string key) => parent.ValueKind == JsonValueKind.Object && parent.TryGetProperty(key, out var child) ? child : default;
    private static string? String(JsonElement parent, string key)
    {
        var value = Property(parent, key);
        return value.ValueKind is JsonValueKind.String or JsonValueKind.Number ? value.ToString() : null;
    }
    private static long? Number(JsonElement parent, string key)
    {
        var value = Property(parent, key);
        return value.ValueKind == JsonValueKind.Number && value.TryGetInt64(out var number) ? number :
            value.ValueKind == JsonValueKind.String && long.TryParse(value.GetString(), out number) ? number : null;
    }
    private static IEnumerable<JsonElement> Items(JsonElement parent, string key)
    {
        var value = Property(parent, key);
        if (value.ValueKind == JsonValueKind.Array) return value.EnumerateArray().ToArray();
        return value.ValueKind == JsonValueKind.Object ? [value] : [];
    }
}

public sealed class CpuDetails
{
    public string? Model { get; set; }
    public string? Vendor { get; set; }
    public int LogicalProcessors { get; set; }
    public int? PhysicalCores { get; set; }
    public double? ClockMHz { get; set; }
    public string? ClockKind { get; set; }
}
public sealed class MemoryDetails
{
    public long? TotalBytes { get; set; }
    public long? AvailableBytes { get; set; }
}
public sealed class MotherboardDetails
{
    public string? Manufacturer { get; set; }
    public string? Product { get; set; }
    public string? Version { get; set; }
}
public sealed class DiskDetails
{
    public string? Name { get; set; }
    public string? Model { get; set; }
    public string? Type { get; set; }
    public long? SizeBytes { get; set; }
}
public sealed class GpuDetails
{
    public string? Name { get; set; }
    public string? Vendor { get; set; }
}
