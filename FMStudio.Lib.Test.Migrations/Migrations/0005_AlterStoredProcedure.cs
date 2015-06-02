using FluentMigrator;
using FMStudio.Lib.Test.Migrations.Resources;
using System;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(5, "Alter stored procedure")]
    public class AlterStoredProcedure : Migration
    {
        public override void Up()
        {
            Execute.Sql(MigrationSql._0005_AlterStoredProcedure);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}