using FluentMigrator;
using System;

namespace FMStudio.Lib.Test.Migrations.Profiles
{
    [Profile("TST")]
    public class TST : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
-- Profile_TST.sql
");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}