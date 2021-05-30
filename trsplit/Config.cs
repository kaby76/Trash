namespace Trash
{
    using CommandLine;

    public class Config
    {
        [Value(0, Min = 1)]
        public string File { get; set; }
    }
}
