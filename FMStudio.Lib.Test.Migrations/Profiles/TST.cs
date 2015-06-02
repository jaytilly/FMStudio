using FluentMigrator;
using FMStudio.Lib.Test.Migrations.Resources;
using System;

namespace FMStudio.Lib.Test.Migrations.Profiles
{
    [Profile("TST")]
    public class TST : Migration
    {
        public override void Up()
        {
            Execute.Sql(MigrationSql.Profile_TST);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}