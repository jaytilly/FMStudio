using FluentMigrator;
using FMStudio.Lib.Test.Migrations.Resources;
using System;
using System.Threading;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(9, "Slow migration 2")]
    public class SlowMigration2 : Migration
    {
        public override void Up()
        {
            Thread.Sleep(1000);

            Execute.Sql(MigrationSql._0009_SlowMigration2);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}