using FluentMigrator;
using FMStudio.Lib.Test.Migrations.Resources;
using System;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(4, "Alter table")]
    public class AlterTable : Migration
    {
        public override void Up()
        {
            Execute.Sql(MigrationSql._0004_AlterTable);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}