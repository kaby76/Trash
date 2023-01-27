namespace Trash
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class Command
	{
		Config _config;
		
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("triconv.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Execute(Config config)
        {
			_config = config;
			
            if (config.Files == null || ! config.Files.Any())
            {
                Process();
            }
            else
            {
                foreach (var arg in config.Files)
                {
                    Process(arg);
                }
            }
        }

        private void Process()
        {
            try
            {
            }
            catch (Exception e)
            {
            }
        }

        private unsafe void Process(string file_name)
        {
            try
            {
                byte[] fileBytes = File.ReadAllBytes(file_name);
                fixed(byte *bytes = fileBytes)
				{
                    Encoding encoding = null;
                    switch (_config.FromCode) {
                        case "utf-8":
                            encoding = new UTF8Encoding(false, true);
                            break;
                        case "utf-32":
                            encoding = new UTF32Encoding(false, true);
                            break;
                        default:
                            encoding = new UTF8Encoding(false, true);
                            break;
					}
                    bool b;
                    try
                    {
                        encoding.GetCharCount(bytes, fileBytes.Length);
                        b = true;
                        var str = encoding.GetString(bytes, fileBytes.Length);
                        System.Console.Write(str);
                    }
                    catch (DecoderFallbackException)
                    {
                        b = false;
                        System.Console.Error.WriteLine("cannot convert");
                    }
                    Environment.ExitCode = b ? 0 : 1;
                }
            }
            catch (Exception e)
            {
                System.Console.Error.WriteLine("Error in file " + e.ToString());
            }
        }
    }
}
