using FluentMigrator;
using FMStudio.Lib.Test.Migrations.Resources;
using System;

namespace FMStudio.Lib.Test.Migrations.Profiles
{
    [Profile("DEV")]
    public class DEV : Migration
    {
        public override void Up()
        {
            Execute.Sql(MigrationSql.Profile_DEV);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}