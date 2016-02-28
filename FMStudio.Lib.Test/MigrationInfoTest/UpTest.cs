using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace FMStudio.Lib.Test.MigrationInfoTest
{
    [TestClass]
    public class UpTest
    {
        [TestMethod]
        public void MigrationInfo_Up_Success()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void MigrationInfo_Up_ClearFromVersionInfoTable()
        {
            Assert.Inconclusive();
        }

        //[TestMethod]
        //public void Up()
        //{
        //    using (var db = new Database())
        //    {
        //        var project = new ProjectInfo(Constants.FMTestMigrationsPath, db.ConnectionString, DatabaseType.Sqlite);
        //        project.InitializeAsync().Wait();

        //        var migration1 = project.Migrations.First();
        //        Assert.IsFalse(migration1.HasRun);

        //        migration1.UpAsync(false).Wait();
        //        Assert.IsTrue(migration1.HasRun);
        //    }
        //}

        //[TestMethod]
        //public void Up_AlreadyRan_WithoutClearVersionInfoTable()
        //{
        //    using (var db = new Database())
        //    {
        //        db
        //            .CreateVersionInfoTable()
        //            .ApplyMigration(1)
        //        ;

        //        var project = new ProjectInfo(Constants.FMTestMigrationsPath, db.ConnectionString, DatabaseType.Sqlite);
        //        project.InitializeAsync().Wait();

        //        var migration1 = project.Migrations.First();
        //        Assert.IsTrue(migration1.HasRun);

        //        migration1.UpAsync(false).Wait();
        //        Assert.IsTrue(migration1.HasRun);
        //    }
        //}

        [TestMethod]
        public void MigrationInfo_Up_NoTransaction()
        {
            var connectionString = @"server=.\SQL2014;database=FMStudioTestDb;integrated security=true;";

            var project = new ProjectInfo();

            project.InitializeMigrationsAsync(Constants.FMTestMigrationsPath).Wait();
            project.InitializeDatabase(DatabaseType.SqlServer2014, connectionString).Wait();

            var migration = project.Migrations.FirstOrDefault(m => m.Version == 10);
            Assert.IsNotNull(migration, "Cannot find migration number 10");

            Assert.IsFalse(migration.HasRun);

            migration.UpAsync(false).Wait();

            Assert.IsTrue(migration.HasRun);
        }

        [TestMethod]
        public void MigrationInfo_Up_RequireTransaction()
        {
            var connectionString = @"server=.\SQL2014;database=FMStudioTestDb;integrated security=true;";

            var project = new ProjectInfo();

            project.InitializeMigrationsAsync(Constants.FMTestMigrationsPath).Wait();
            project.InitializeDatabase(DatabaseType.SqlServer2014, connectionString).Wait();

            var migration = project.Migrations.FirstOrDefault(m => m.Version == 11);
            Assert.IsNotNull(migration, "Cannot find migration number 11");

            Assert.IsFalse(migration.HasRun);

            migration.UpAsync(false).Wait();

            Assert.IsTrue(migration.HasRun);
        }
    }
}