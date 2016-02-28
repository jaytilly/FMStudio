using FluentMigrator;
using System;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(11, TransactionBehavior.Default, "Requires transaction")]
    public class RequireTransaction : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
IF XACT_STATE() = 0
BEGIN
	RAISERROR('This migration must be run in a transaction', 18, 1)
END
");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}