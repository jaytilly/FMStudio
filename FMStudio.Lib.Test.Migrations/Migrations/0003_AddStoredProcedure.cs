using FluentMigrator;
using FMStudio.Lib.Test.Migrations.Resources;
using System;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(3, "Add stored procedure")]
    public class AddStoredProcedure : Migration
    {
        public override void Up()
        {
            Execute.Sql(MigrationSql._0003_AddStoredProcedure);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}