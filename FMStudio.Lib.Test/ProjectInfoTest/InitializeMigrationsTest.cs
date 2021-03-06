﻿using Xunit;

namespace FMStudio.Lib.Test.ProjectInfoTest
{
    public class InitializeMigrationsTest
    {
        [Theory(Skip = "TODO")]
        [ClassData(typeof(ContextProvider))]
        public void NoMigrationsRan()
        {
            //    using (var db = new Database())
            //    {
            //        db.CreateVersionInfoTable();

            //        var project = new ProjectInfo(Constants.FMTestMigrationsPath, db.ConnectionString, DatabaseType.Sqlite);

            //        project.InitializeAsync().Wait();

            //        Assert.IsNotNull(project.Migrations);
            //        Assert.AreEqual(6, project.Migrations.Count);

            //        AssertMigration(project.Migrations, 1, ExecuteSql.Name, false, new string[0], "-- 0001_ExecuteSql.sql");
            //        AssertMigration(project.Migrations, 2, "Add table", false, new string[0], "-- 0002_AddTable.sql");
            //        AssertMigration(project.Migrations, 3, "Add stored procedure", false, new string[0], "-- 0003_AddStoredProcedure.sql");
            //        AssertMigration(project.Migrations, 4, "Alter table", false, new string[0], "-- 0004_AlterTable");
            //        AssertMigration(project.Migrations, 5, "Alter stored procedure", false, new string[0], "-- 0005_AlterStoredProcedure.sql");
            //        AssertMigration(project.Migrations, 6, "Insert data", false, new string[] { "DEV", "TST" }, "-- 0006_InsertData.sql");

            //        Assert.IsNotNull(project.Profiles);
            //        Assert.AreEqual(2, project.Profiles.Count);

            //        AssertProfile(project.Profiles, "DEV", "-- Profile_DEV.sql");
            //        AssertProfile(project.Profiles, "TST", "-- Profile_TST.sql");
            //    }
        }

        [Theory(Skip = "TODO")]
        [ClassData(typeof(ContextProvider))]
        public void OneMigrationRan()
        {
            //    using (var db = new Database())
            //    {
            //        db
            //            .CreateVersionInfoTable()
            //            .ApplyMigration(1, ExecuteSql.Name)
            //        ;

            //        var project = new ProjectInfo(Constants.FMTestMigrationsPath, db.ConnectionString, DatabaseType.Sqlite);

            //        project.InitializeAsync().Wait();

            //        Assert.IsNotNull(project.Migrations);
            //        Assert.AreEqual(6, project.Migrations.Count);

            //        AssertMigration(project.Migrations, 1, ExecuteSql.Name, true, new string[0], "-- 0001_ExecuteSql.sql");
            //        AssertMigration(project.Migrations, 2, "Add table", false, new string[0], "-- 0002_AddTable.sql");
            //        AssertMigration(project.Migrations, 3, "Add stored procedure", false, new string[0], "-- 0003_AddStoredProcedure.sql");
            //        AssertMigration(project.Migrations, 4, "Alter table", false, new string[0], "-- 0004_AlterTable");
            //        AssertMigration(project.Migrations, 5, "Alter stored procedure", false, new string[0], "-- 0005_AlterStoredProcedure.sql");
            //        AssertMigration(project.Migrations, 6, "Insert data", false, new string[] { "DEV", "TST" }, "-- 0006_InsertData.sql");

            //        Assert.IsNotNull(project.Profiles);
            //        Assert.AreEqual(2, project.Profiles.Count);

            //        AssertProfile(project.Profiles, "DEV", "-- Profile_DEV.sql");
            //        AssertProfile(project.Profiles, "TST", "-- Profile_TST.sql");
            //    }
        }

        [Theory(Skip = "TODO")]
        [ClassData(typeof(ContextProvider))]
        public void PathToMigrationsDllNotFound()
        {
            //    var project = new ProjectInfo(@"no/such/file", _db.ConnectionString, DatabaseType.Sqlite);

            //    TestUtility.AssertExceptionThrown<InitializeProjectException>(() => project.InitializeAsync().Wait());
        }

        [Theory(Skip = "TODO")]
        [ClassData(typeof(ContextProvider))]
        public void Project_Initialize_CouldNotConnectToDatabase()
        {
            //    var project = new ProjectInfo(Constants.FMTestMigrationsPath, @"no/such/connection", DatabaseType.Sqlite);

            //    TestUtility.AssertExceptionThrown<InitializeProjectException>(() => project.InitializeAsync().Wait());
        }

        [Theory(Skip = "TODO")]
        [ClassData(typeof(ContextProvider))]
        public void DllIsNotAMigrationsDll()
        {
            //    var project = new ProjectInfo(Constants.FMUtilityDllPath, _db.ConnectionString, DatabaseType.Sqlite);

            //    TestUtility.AssertExceptionThrown<InitializeProjectException>(
            //        () => project.InitializeAsync().Wait(),
            //        (e) => Assert.AreEqual(ExceptionType.CouldNotFindFluentMigratorDllReference, e.ExceptionType)
            //    );
        }

        [Theory(Skip = "TODO")]
        [ClassData(typeof(ContextProvider))]
        public void PathToMigrationsDllIsNotADll()
        {
        }

        #region Utility

        //private void AssertMigration(List<MigrationInfo> migrations, long version, string description, bool hasRun, string[] tags, string sql)
        //{
        //    var migrationInfo = migrations.FirstOrDefault(m => m.Version == version);
        //    Assert.IsNotNull(migrationInfo, "Expected migration but not found: " + version);
        //    Assert.AreEqual(description, migrationInfo.Description);
        //    Assert.AreEqual(hasRun, migrationInfo.HasRun);
        //    Assert.IsNotNull(migrationInfo.Tags);
        //    Assert.AreEqual(tags.Length, migrationInfo.Tags.Count);

        //    foreach (var tag in tags)
        //    {
        //        Assert.IsTrue(migrationInfo.Tags.Any(t => t == tag), "Expected tag but not found: " + tag);
        //    }

        //    Assert.IsTrue(migrationInfo.Sql.Contains(sql), "Migration sql not found: " + sql);
        //}

        //private void AssertProfile(List<ProfileInfo> profiles, string name, string sql)
        //{
        //    var profileInfo = profiles.FirstOrDefault(p => p.Name == name);
        //    Assert.IsNotNull(profileInfo, "Expected profile but not found: " + name);
        //    Assert.IsTrue(profileInfo.Sql.Contains(sql), "Profile sql not found: " + sql);
        //}

        #endregion Utility
    }
}