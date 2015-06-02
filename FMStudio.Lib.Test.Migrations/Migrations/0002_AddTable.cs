using FluentMigrator;
using FMStudio.Lib.Test.Migrations.Resources;
using System;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(2, "Add table")]
    public class AddTable : Migration
    {
        public override void Up()
        {
            Execute.Sql(MigrationSql._0002_AddTable);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}