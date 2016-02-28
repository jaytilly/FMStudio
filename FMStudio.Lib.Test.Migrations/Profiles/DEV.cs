using FluentMigrator;
using System;

namespace FMStudio.Lib.Test.Migrations.Profiles
{
    [Profile("DEV")]
    public class DEV : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
-- Profile_DEV.sql
");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}