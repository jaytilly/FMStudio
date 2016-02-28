using FluentMigrator;
using System;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(3, "Add stored procedure")]
    public class AddStoredProcedure : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
-- 0003_AddStoredProcedure.sql
");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}