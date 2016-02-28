using FluentMigrator;
using System;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(5, "Alter stored procedure")]
    public class AlterStoredProcedure : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
-- 0005_AlterStoredProcedure.sql
");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}