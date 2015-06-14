using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(7, "Migration with a really really loooonnggg name")]
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
