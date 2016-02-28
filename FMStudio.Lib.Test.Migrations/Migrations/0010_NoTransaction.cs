using FluentMigrator;
using System;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(10, TransactionBehavior.None, "No transaction")]
    public class NoTransaction : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
IF XACT_STATE() = 1
BEGIN
	RAISERROR('This migration cannot be run in a transaction', 18, 1)
END
");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}