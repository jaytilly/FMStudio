using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}