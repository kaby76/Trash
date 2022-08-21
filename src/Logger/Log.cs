namespace LoggerNs
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Threading;


    public class LogTextWriter : TextWriter
    {
        public override Encoding Encoding => throw new NotImplementedException();
        private static readonly string home = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
        private static readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private static bool done = false;

        // This method must only be called from the client so multiple instances of the
        // server doesn't blow away the file after the client already had.
        public void CleanUpLogFile()
        {
            if (done) return;
            done = true;
            var log = home
               + System.IO.Path.DirectorySeparatorChar
               + ".antlrlog";
            cacheLock.EnterWriteLock();
            try
            {
                File.Delete(log);
                using (StreamWriter w = File.AppendText(log))
                {
                    w.WriteLine("Logging for Antlrvsix started "
                        + DateTime.Now.ToString());
                }
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public override void Write(string message)
        {
            var log = home
               + System.IO.Path.DirectorySeparatorChar
               + ".antlrlog";
            cacheLock.EnterWriteLock();
            try
            {
                using (StreamWriter w = File.AppendText(log))
                {
                    w.Write(message);
                }
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public override void WriteLine(string message)
        {
            var log = home
               + System.IO.Path.DirectorySeparatorChar
               + ".antlrlog";
            cacheLock.EnterWriteLock();
            try
            {
                using (StreamWriter w = File.AppendText(log))
                {
                    w.WriteLine(message);
                }
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public void Notify(string message)
        {
            WriteLine(message);
            bool options = Options.Option.GetBoolean("OptInLogging");
            if (options)
            {
                try
                {
                    HttpClient httpClient = new HttpClient();
                    HttpResponseMessage result = httpClient.GetAsync("http://domemtech.com/home/db?" + message).Result;
                }
                catch (Exception)
                {
                }
            }
        }

    }

    public class Logger
    {
        public static LogTextWriter Log { get; set; } = new LogTextWriter();
    }
}
