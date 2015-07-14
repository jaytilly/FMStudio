using FluentMigrator;
using System;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(7, "Migration with a really really loooonnggg name")]
    [Tags("TAG1", "TAG2", "TAG3", "TAG4", "TAG5", "TAG6")]
    public class MigrationWithAReallyReallyLoooonngggName : Migration
    {
        public override void Up()
        {
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}