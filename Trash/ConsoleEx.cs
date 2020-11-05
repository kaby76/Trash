namespace Trash
{
    using System;
    using System.Runtime.InteropServices;

    public static class ConsoleEx
    {
        public static bool IsOutputRedirected
        {
            get { return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stdout)); }
        }
        public static bool IsInputRedirected
        {
            get { return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stdin)); }
        }
        public static bool IsErrorRedirected
        {
            get { return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stderr)); }
        }

        // P/Invoke:
        private enum FileType { Unknown, Disk, Char, Pipe };
        private enum StdHandle { Stdin = -10, Stdout = -11, Stderr = -12 };
        [DllImport("kernel32.dll")]
        private static extern FileType GetFileType(IntPtr hdl);
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(StdHandle std);
    }
}
