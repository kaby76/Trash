namespace Server
{
    using System;
    using System.IO;
    using System.Text;

    internal partial class Program
    {
        class Dup : Stream
        {
            string _name;

            private Dup() { }

            public Dup(string name) { _name = name; }

            public override bool CanRead { get { return false; } }

            public override bool CanSeek { get { return false; } }

            public override bool CanWrite { get { return true; } }

            public override long Length => throw new NotImplementedException();

            public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotImplementedException();
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                DateTime now = DateTime.Now;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Raw message from " + _name + " " + now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
                var truncated_array = new byte[count];
                for (int i = offset; i < offset + count; ++i)
                    truncated_array[i - offset] = buffer[i];
                string str = System.Text.Encoding.Default.GetString(truncated_array);
                sb.AppendLine("data (length " + str.Length + ")= '" + str + "'");
                LoggerNs.Logger.Log.WriteLine(sb.ToString());
            }
        }
    }
}
