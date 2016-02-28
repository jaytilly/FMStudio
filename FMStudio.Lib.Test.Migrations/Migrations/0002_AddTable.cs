using FluentMigrator;
using System;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(2, "Add table")]
    public class AddTable : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
-- 0002_AddTable.sql
");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}