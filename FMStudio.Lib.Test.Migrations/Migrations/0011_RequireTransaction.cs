using FluentMigrator;
using FMStudio.Lib.Test.Migrations.Resources;
using System;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(11, TransactionBehavior.Default, "Requires transaction")]
    public class RequireTransaction : Migration
    {
        public override void Up()
        {
            Execute.Sql(MigrationSql._0011_RequireTransaction);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}