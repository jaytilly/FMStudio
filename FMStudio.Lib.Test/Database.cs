using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FMStudio.Lib.Test
{
    public class Database : IDisposable
    {
        public const string DbDirectory = "Databases";
        public const string SqliteExtension = ".sqlite";

        public static void Cleanup()
        {
            Debug.WriteLine("Cleaning up stale database files...");

            if (Directory.Exists(DbDirectory))
                Directory.GetFiles(DbDirectory).Where(f => f.EndsWith(SqliteExtension)).ToList().ForEach(f => File.Delete(f));
        }

        private string _path;

        public Database()
        {
            Directory.CreateDirectory(DbDirectory);
            _path = Path.GetFullPath(DbDirectory + "/" + Guid.NewGuid() + SqliteExtension);

            SQLiteConnection.CreateFile(_path);
        }

        public string ConnectionString
        {
            get { return string.Format(@"Data Source={0};Version=3;", _path); }
        }

        public void Dispose()
        {
            try
            {
                File.Delete(_path);
            }
            catch (Exception) { }
        }

        public Database CreateVersionInfoTable()
        {
            ExecuteSql(@"
                PRAGMA foreign_keys = off;
                BEGIN TRANSACTION;

                CREATE TABLE VersionInfo (
                    Version     INTEGER  NOT NULL,
                    AppliedOn   DATETIME,
                    Description TEXT
                );

                CREATE UNIQUE INDEX UC_Version ON VersionInfo (
                    Version ASC
                );

                COMMIT TRANSACTION;
                PRAGMA foreign_keys = on;");

            return this;
        }

        public Database ApplyMigration(long version, string name = "", DateTime? date = null, string sql = null)
        {
            ExecuteSql(string.Format(@"
                INSERT INTO VersionInfo (
                    Version,
                    AppliedOn,
                    Description
                )
                VALUES (
                    1,
                    '2015-05-24T22:26:39',
                    'Execute Sql'
                );", version, date.HasValue ? date : DateTime.Now, name));

            if (!string.IsNullOrEmpty(sql))
            {
                ExecuteSql(sql);
            }

            return this;
        }

        private Database ExecuteSql(string sql)
        {
            using (var db = new SQLiteConnection(ConnectionString))
            {
                db.Open();

                var command = db.CreateCommand();
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }

            return this;
        }
    }
}