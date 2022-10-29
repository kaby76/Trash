namespace Trash
{
    public enum OSType
    {
        Unix,
        Windows,
        Mac,
    }

    public static class Extensions
    {
        public static string ToString(this OSType os_type)
        {
            var s = os_type switch
            {
                OSType.Unix => "unix",
                OSType.Windows => "windows",
                OSType.Mac => "mac",
            };
            return s;
        }
    }

}
