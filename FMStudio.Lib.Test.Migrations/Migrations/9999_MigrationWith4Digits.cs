using FluentMigrator;
using System;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(9999, "Migration with 4 digits")]
    public class MigrationWith4Digits : Migration
    {
        public override void Up()
        {
            Execute.Sql(
@"-- 0001_ExecuteSql.sql

--CREATE TABLE Table1
--(
--	Id		INTEGER		PRIMARY KEY ASC,
--	Name	VARCHAR(100)
--)");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}