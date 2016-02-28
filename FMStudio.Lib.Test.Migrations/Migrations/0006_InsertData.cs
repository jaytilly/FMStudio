using FluentMigrator;
using System;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(6, "Insert data")]
    [Tags("DEV", "TST")]
    public class InsertData : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
-- 0006_InsertData.sql
");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}