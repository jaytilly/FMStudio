using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace FMStudio.Lib.Test
{
    public class Context
    {
        public string Name { get; set; }

        public string DBConnectionString { get; set; }

        public string FMTestMigrationsPath { get; set; }

        // Used for testing non-migration dll's
        public string FMUtilityDllPath { get; set; }

        public string FMDllPath { get; set; }

        public string FMRunnerDllPath { get; set; }

        public string FMTestMigrationsFullName { get; set; }

        public Context(string name)
        {
            Name = name;
        }

        public Context Initialize()
        {
            var connStr = new SqlConnectionStringBuilder(DBConnectionString);
            connStr.InitialCatalog = "master";

            using (var conn = new SqlConnection(connStr.ConnectionString))
            {
                conn.Open();

                var comm = new SqlCommand(@"
IF EXISTS(SELECT name FROM sys.databases WHERE name = 'FMStudioTestDb')
BEGIN
	ALTER DATABASE FMStudioTestDb SET OFFLINE WITH ROLLBACK IMMEDIATE
	ALTER DATABASE FMStudioTestDb SET ONLINE

	DROP DATABASE FMStudioTestDb
END

CREATE DATABASE FMStudioTestDb
", conn);
                comm.ExecuteNonQuery();
            }

            return this;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ContextProvider : IEnumerable<object[]>
    {
        IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator()
        {
            yield return new[]
            {
                new Context("FM160 SQL2014")
                {
                    DBConnectionString = @"server=.\SQL2014;database=FMStudioTestDb;user=sa;password=Password12!;pooling=false;",

                    FMTestMigrationsPath = Path.GetFullPath(@"FMStudio.Lib.Test.Migrations.dll"),
                    FMUtilityDllPath = Path.GetFullPath(@"FMStudio.Utility.dll"),
                    FMDllPath = Path.GetFullPath(@"FluentMigrator.dll"),
                    FMRunnerDllPath = Path.GetFullPath(@"FluentMigrator.Runner.dll"),
                    FMTestMigrationsFullName = "FMStudio.Lib.Test.Migrations, Version="
                }.Initialize()
            };
            yield return new[]
            {
                new Context("FM130 SQL2014")
                {
                    DBConnectionString = @"server=.\SQL2014;database=FMStudioTestDb;user=sa;password=Password12!;pooling=false;",

                    FMTestMigrationsPath = Path.GetFullPath(@"../../FMStudio.Lib.Test.Migrations.FM130/bin/FMStudio.Lib.Test.Migrations.FM130.dll"),
                    FMUtilityDllPath = Path.GetFullPath(@"FMStudio.Utility.dll"),
                    FMDllPath = Path.GetFullPath(@"FluentMigrator.dll"),
                    FMRunnerDllPath = Path.GetFullPath(@"FluentMigrator.Runner.dll"),
                    FMTestMigrationsFullName = "FMStudio.Lib.Test.Migrations.FM130, Version="
                }.Initialize()
            };
        }

        public IEnumerator GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}