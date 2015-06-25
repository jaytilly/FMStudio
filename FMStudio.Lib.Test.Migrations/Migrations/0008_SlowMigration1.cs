using FluentMigrator;
using FMStudio.Lib.Test.Migrations.Resources;
using System;
using System.Threading;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(8, "Slow migration 1")]
    public class SlowMigration1 : Migration
    {
        public override void Up()
        {
            Thread.Sleep(500);

            Execute.Sql(MigrationSql._0008_SlowMigration1);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}