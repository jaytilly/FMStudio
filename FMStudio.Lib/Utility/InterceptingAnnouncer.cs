using FluentMigrator.Runner;
using System;
using System.IO;

namespace FMStudio.Lib.Utility
{
    public sealed class InterceptingAnnouncer : IAnnouncer, IDisposable
    {
        public MemoryStream SqlStream { get; private set; }

        private StreamWriter _writer;

        public InterceptingAnnouncer()
        {
            SqlStream = new MemoryStream();

            _writer = new StreamWriter(SqlStream);
        }

        public void ElapsedTime(TimeSpan timeSpan)
        {
        }

        public void Emphasize(string message)
        {
        }

        public void Error(Exception exception)
        {
        }

        public void Error(string message)
        {
        }

        public void Heading(string message)
        {
        }

        public void Say(string message)
        {
        }

        public void Sql(string sql)
        {
            _writer.WriteLine(sql);
        }

        public void Write(string message, bool escaped)
        {
        }

        public void Flush()
        {
            _writer.Flush();
        }

        public void Dispose()
        {
            SqlStream.Dispose();
        }
    }
}