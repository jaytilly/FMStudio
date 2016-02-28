using FluentMigrator;
using System;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(4, "Alter table")]
    public class AlterTable : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
-- 0004_AlterTable.sql
");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}