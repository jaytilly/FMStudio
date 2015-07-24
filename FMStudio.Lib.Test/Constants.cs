using System.IO;

namespace FMStudio.Lib.Test
{
    public static class Constants
    {
        public static readonly string FMTestMigrationsPath = Path.GetFullPath(@"FMStudio.Lib.Test.Migrations.dll");

        // Used for testing non-migration dll's
        public static readonly string FMUtilityDllPath = Path.GetFullPath(@"FMStudio.Utility.dll");

        public static readonly string FMDllPath = Path.GetFullPath(@"FluentMigrator.dll");

        public static readonly string FMRunnerDllPath = Path.GetFullPath(@"FluentMigrator.Runner.dll");

        public static readonly string FMTestMigrationsFullName = typeof(Migrations.Migrations.AddTable).Assembly.FullName;
    }
}