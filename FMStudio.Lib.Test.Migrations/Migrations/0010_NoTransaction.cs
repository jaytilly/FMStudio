using FluentMigrator;
using FMStudio.Lib.Test.Migrations.Resources;
using System;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(10, TransactionBehavior.None, "No transaction")]
    public class NoTransaction : Migration
    {
        public override void Up()
        {
            Execute.Sql(MigrationSql._0010_NoTransaction);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}