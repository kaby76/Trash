namespace Options
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class Option
    {
        private static string antlr_options_file_name = "";
        private static readonly string CorpusLocation = s ?? "";
        private static Dictionary<string, object> defaults = new Dictionary<string, object>()
        {
            {"IncrementalReformat", true },
            {"RestrictedDirectory", true },
            {"GenerateVisitorListener", false },
            {"OverrideAntlrPluggins", true },
            {"OptInLogging", false },
            {"CorpusLocation", CorpusLocation },
            {"SemanticHighlighting", false },
            {"AntlrNonterminalDef", "type" },
            {"AntlrNonterminalRef", "symbol definition" },
            {"AntlrTerminalDef", "type" },
            {"AntlrTerminalRef", "symbol definition" },
            {"AntlrComment", "comment" },
            {"AntlrKeyword", "keyword" },
            {"AntlrLiteral", "string" },
            {"AntlrModeDef", "type" },
            {"AntlrModeRef", "field name" },
            {"AntlrChannelDef", "type" },
            {"AntlrChannelRef", "field name" },
            {"AntlrPunctuation", "field name" },
            {"AntlrOperator", "field name" },
        };
        private static readonly string home = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
        private static bool initialized = false;
        private static readonly string s = System.Environment.GetEnvironmentVariable("CORPUS_LOCATION");

        public static bool GetBoolean(string option)
        {
            Initialize();
            defaults.TryGetValue(option, out object value);
            bool default_value;
            if (value == null)
            {
                default_value = false;
            }
            else
            {
                default_value = (bool)value;
            }
            return default_value;
        }

        public static int GetInt32(string option)
        {
            Initialize();
            defaults.TryGetValue(option, out object value);
            int default_value;
            if (value == null)
            {
                default_value = 0;
            }
            else
            {
                default_value = (int)value;
            }
            return default_value;
        }

        public static string GetString(string option)
        {
            Initialize();
            defaults.TryGetValue(option, out object value);
            string default_value;
            if (value == null)
            {
                default_value = "";
            }
            else
            {
                default_value = value.ToString();
            }
            return default_value;
        }

        public static void SetBoolean(string option, bool value)
        {
            Initialize();
            defaults[option] = value;
            Write();
        }

        public static void SetInt32(string option, int value)
        {
            Initialize();
            defaults[option] = value;
            Write();
        }

        public static void SetString(string option, string value)
        {
            Initialize();
            defaults[option] = value;
            Write();
        }

        private static void Initialize()
        {
            if (initialized)
            {
                return;
            }
            Read();
        }

        private static void Read()
        {
            antlr_options_file_name = home
                                      + Path.DirectorySeparatorChar
                                      + ".antlrvsixrc";
            if (Path.IsPathRooted(antlr_options_file_name))
            {
                if (File.Exists(antlr_options_file_name))
                {
                    string jsonString = File.ReadAllText(antlr_options_file_name);
                    var file_values = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
                    // Sum defaults and file_values.
                    var new_list = new Dictionary<string, object>(defaults);
                    foreach (var p in file_values)
                    {
                        new_list[p.Key] = p.Value;
                    }
                    bool same = file_values.Count == new_list.Count && !file_values.Except(new_list).Any();
                    defaults = new_list;
                    if (! same)
                    {
                        Write();
                    }
                }
                else
                {
                    Write();
                }
            }
            initialized = true;
        }

        private static void Write()
        {
            if (Path.IsPathRooted(antlr_options_file_name))
            {
                var ser = Newtonsoft.Json.JsonConvert.SerializeObject(defaults);
                File.WriteAllText(antlr_options_file_name, ser);
                _ = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(ser);
            }
            initialized = true;
        }
    }
}
