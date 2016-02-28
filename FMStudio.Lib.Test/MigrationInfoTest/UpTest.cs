using System.Linq;
using Xunit;

namespace FMStudio.Lib.Test.MigrationInfoTest
{
    public class UpTest
    {
        [Theory(Skip = "TODO")]
        [ClassData(typeof(ContextProvider))]
        public void Success()
        {
        }

        [Theory(Skip = "TODO")]
        [ClassData(typeof(ContextProvider))]
        public void ClearFromVersionInfoTable()
        {
        }

        [Theory(Skip = "TODO")]
        [ClassData(typeof(ContextProvider))]
        public void AlreadyRan_WithoutClearVersionInfoTable()
        {
        }

        [Theory]
        [ClassData(typeof(ContextProvider))]
        public void NoTransaction(Context context)
        {
            var project = new ProjectInfo();

            project.InitializeMigrationsAsync(context.FMTestMigrationsPath).Wait();
            project.InitializeDatabase(DatabaseType.SqlServer2014, context.DBConnectionString).Wait();

            var migration = project.Migrations.FirstOrDefault(m => m.Version == 10);
            Assert.NotNull(migration);
            Assert.False(migration.HasRun);

            migration.UpAsync(false).Wait();

            Assert.True(migration.HasRun);
        }

        [Theory]
        [ClassData(typeof(ContextProvider))]
        public void RequireTransaction(Context context)
        {
            var project = new ProjectInfo();

            project.InitializeMigrationsAsync(context.FMTestMigrationsPath).Wait();
            project.InitializeDatabase(DatabaseType.SqlServer2014, context.DBConnectionString).Wait();

            var migration = project.Migrations.FirstOrDefault(m => m.Version == 11);
            Assert.NotNull(migration);

            Assert.False(migration.HasRun);

            migration.UpAsync(false).Wait();

            Assert.True(migration.HasRun);
        }
    }
}