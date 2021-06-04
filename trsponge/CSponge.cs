namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using LanguageServer;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    class CSponge
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trsponge.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

		public void Execute(Config config)
        {
            string lines = null;
            if (!(config.File != null && config.File != ""))
            {
                for (; ; )
                {
                    lines = System.Console.In.ReadToEnd();
                    if (lines != null && lines != "") break;
                }
            }
            else
            {
                lines = File.ReadAllText(config.File);
            }
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            foreach (var parse_info in data)
            {
                var nodes = parse_info.Nodes;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                var code = parse_info.Text;
                var fn = parse_info.FileName;
                if (config.OutputDirectory != null)
                {
                    if (!Directory.Exists(config.OutputDirectory))
                    {
                        throw new System.Exception("Diectory " + config.OutputDirectory
                            + " does not exist. Quiting.");
                    }
                    if (!(config.OutputDirectory.EndsWith("\\") || config.OutputDirectory.EndsWith("/")))
                        config.OutputDirectory = config.OutputDirectory + "/";
                    fn = config.OutputDirectory + Path.GetFileName(fn);
                }
                if (File.Exists(fn) && (config.Clobber == null || !(bool)config.Clobber ))
                    throw new System.Exception("Attempting to overwrite '" + fn + "'. Use --overwrite option if it is intended.");
                System.Console.Error.WriteLine("Writing to " + fn);
                File.WriteAllText(fn, code);
            }
        }
    }
}
