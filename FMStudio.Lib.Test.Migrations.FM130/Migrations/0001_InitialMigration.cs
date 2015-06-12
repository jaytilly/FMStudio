using FluentMigrator;
using FMStudio.Lib.Test.Migrations.FM130.Resources;

namespace FMStudio.Lib.Test.Migrations.FM130.Migrations
{
    [Migration(1, "Initial migration")]
    public class InitialMigration : Migration
    {
        public override void Up()
        {
            Execute.Sql(MigrationSql._0001_InitialMigration_Up);
        }

        public override void Down()
        {
            Execute.Sql(MigrationSql._0001_InitialMigration_Down);
        }
    }
}