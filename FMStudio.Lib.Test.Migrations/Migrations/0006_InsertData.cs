using FluentMigrator;
using FMStudio.Lib.Test.Migrations.Resources;
using System;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(6, "Insert data")]
    [Tags("DEV", "TST")]
    public class InsertData : Migration
    {
        public override void Up()
        {
            Execute.Sql(MigrationSql._0006_InsertData);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}