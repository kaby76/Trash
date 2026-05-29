using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Trash;

public class Program
{
    private const string Version = "0.23.45";

    // Maps both full names (e.g. "trgen") and short aliases (e.g. "gen") to the
    // sub-tool directory name that lives next to trash.dll in the package.
    private static readonly Dictionary<string, string> CommandMap =
        new(StringComparer.OrdinalIgnoreCase)
    {
        // full names
        { "tranalyze",    "tranalyze"    },
        { "trcaret",      "trcaret"      },
        { "trclonereplace", "trclonereplace" },
        { "trcombine",    "trcombine"    },
        { "trconvert",    "trconvert"    },
        { "trcover",      "trcover"      },
//      { "trdistill",    "trdistill"    },
        { "trdot",        "trdot"        },
//      { "trenum",       "trenum"       },
        { "trextract",    "trextract"    },
        { "trff",         "trff"         },
//      { "trfold",       "trfold"       },
        { "trfoldlit",    "trfoldlit"    },
//      { "trformat",     "trformat"     },
        { "trgen",        "trgen"        },
        { "trgenvsc",     "trgenvsc"     },
        { "trglob",       "trglob"       },
//      { "trgroup",      "trgroup"      },
        { "triconv",      "triconv"      },
        { "tritext",      "tritext"      },
        { "trjson",       "trjson"       },
//      { "trkleene",     "trkleene"     },
        { "trnullable",   "trnullable"   },
        { "trparse",      "trparse"      },
        { "trperf",       "trperf"       },
//      { "trpiggy",      "trpiggy"      },
        { "trquery",      "trquery"      },
        { "trrename",     "trrename"     },
//      { "trrr",         "trrr"         },
//      { "trrup",        "trrup"        },
//      { "trsem",        "trsem"        },
        { "trsort",       "trsort"       },
        { "trsplit",      "trsplit"      },
        { "trsponge",     "trsponge"     },
        { "trtext",       "trtext"       },
//      { "trthompson",   "trthompson"   },
        { "trtokens",     "trtokens"     },
        { "trtree",       "trtree"       },
//      { "trull",        "trull"        },
        { "trunfold",     "trunfold"     },
        { "trunfoldlit",  "trunfoldlit"  },
        { "trungroup",    "trungroup"    },
        { "trwdog",       "trwdog"       },
        { "trxgrep",      "trxgrep"      },
        { "trxml",        "trxml"        },
        { "trxml2",       "trxml2"       },
        // short aliases (strip the "tr" / "tri" prefix)
        { "analyze",      "tranalyze"    },
        { "caret",        "trcaret"      },
        { "clonereplace", "trclonereplace" },
        { "combine",      "trcombine"    },
        { "convert",      "trconvert"    },
        { "cover",        "trcover"      },
//      { "distill",      "trdistill"    },
        { "dot",          "trdot"        },
//      { "enum",         "trenum"       },
        { "extract",      "trextract"    },
        { "ff",           "trff"         },
//      { "fold",         "trfold"       },
        { "foldlit",      "trfoldlit"    },
//      { "format",       "trformat"     },
        { "gen",          "trgen"        },
        { "genvsc",       "trgenvsc"     },
        { "glob",         "trglob"       },
//      { "group",        "trgroup"      },
        { "iconv",        "triconv"      },
        { "itext",        "tritext"      },
        { "json",         "trjson"       },
//      { "kleene",       "trkleene"     },
        { "nullable",     "trnullable"   },
        { "parse",        "trparse"      },
        { "perf",         "trperf"       },
//      { "piggy",        "trpiggy"      },
        { "query",        "trquery"      },
        { "rename",       "trrename"     },
//      { "rr",           "trrr"         },
//      { "rup",          "trrup"        },
//      { "sem",          "trsem"        },
        { "sort",         "trsort"       },
        { "split",        "trsplit"      },
        { "sponge",       "trsponge"     },
        { "text",         "trtext"       },
//      { "thompson",     "trthompson"   },
        { "tokens",       "trtokens"     },
        { "tree",         "trtree"       },
//      { "ull",          "trull"        },
        { "unfold",       "trunfold"     },
        { "unfoldlit",    "trunfoldlit"  },
        { "ungroup",      "trungroup"    },
        { "wdog",         "trwdog"       },
        { "xgrep",        "trxgrep"      },
        { "xml",          "trxml"        },
        { "xml2",         "trxml2"       },
    };

    public static int Main(string[] args)
    {
        if (args.Length == 0 || args[0] is "--help" or "-h")
        {
            PrintHelp();
            return 0;
        }

        if (args[0] is "--version" or "-v")
        {
            Console.WriteLine(Version);
            return 0;
        }

        var name = args[0];
        if (!CommandMap.TryGetValue(name, out var toolDir))
        {
            Console.Error.WriteLine($"trash: unknown command '{name}'");
            Console.Error.WriteLine("Run 'trash --help' to see available commands.");
            return 1;
        }

        var baseDir = AppContext.BaseDirectory;
        var dllPath = Path.Combine(baseDir, toolDir, $"{toolDir}.dll");
        if (!File.Exists(dllPath))
        {
            Console.Error.WriteLine($"trash: sub-tool '{toolDir}' was not found at: {dllPath}");
            return 1;
        }

        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            UseShellExecute = false,
        };
        psi.ArgumentList.Add(dllPath);
        foreach (var a in args.Skip(1))
            psi.ArgumentList.Add(a);

        using var process = Process.Start(psi)!;
        process.WaitForExit();
        return process.ExitCode;
    }

    static void PrintHelp()
    {
        Console.WriteLine($"trash {Version} - The Trash toolkit");
        Console.WriteLine();
        Console.WriteLine("Usage:  trash <command> [options]");
        Console.WriteLine("        trash <alias>   [options]");
        Console.WriteLine();
        Console.WriteLine("Commands (full name / short alias):");
        Console.WriteLine();

        // Canonical entries are those where key == value (full names)
        var canonicals = CommandMap
            .Where(kv => kv.Key == kv.Value)
            .Select(kv => kv.Key)
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase);

        foreach (var cmd in canonicals)
        {
            // Derive the alias: strip leading "tr" (handles tr*, tri*)
            var alias = cmd.StartsWith("tr", StringComparison.OrdinalIgnoreCase)
                ? cmd[2..]
                : cmd;
            Console.WriteLine($"  {cmd,-22} alias: {alias}");
        }

        Console.WriteLine();
        Console.WriteLine("Run 'trash <command> --help' for help on a specific command.");
    }
}
