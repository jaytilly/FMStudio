using FluentMigrator;
using FMStudio.Lib.Test.Migrations.Resources;
using System;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(1, Name)]
    public class ExecuteSql : Migration
    {
        public const string Name = "Execute Sql";

        public override void Up()
        {
            Execute.Sql(MigrationSql._0001_ExecuteSql);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}