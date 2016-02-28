using FluentMigrator;
using System;

namespace FMStudio.Lib.Test.Migrations.Migrations
{
    [Migration(1, Name)]
    public class ExecuteSql : Migration
    {
        public const string Name = "Execute Sql";

        public override void Up()
        {
            Execute.Sql(
@"-- 0001_ExecuteSql.sql

--CREATE TABLE Table1
--(
--	Id		INTEGER		PRIMARY KEY ASC,
--	Name	VARCHAR(100)
--)

--CREATE TABLE Table1
--(
--	Id		INTEGER		PRIMARY KEY ASC,
--	Name	VARCHAR(100)
--)

--CREATE TABLE Table1
--(
--	Id		INTEGER		PRIMARY KEY ASC,
--	Name	VARCHAR(100)
--)

--CREATE TABLE Table1
--(
--	Id		INTEGER		PRIMARY KEY ASC,
--	Name	VARCHAR(100)
--)

--CREATE TABLE Table1
--(
--	Id		INTEGER		PRIMARY KEY ASC,
--	Name	VARCHAR(100)
--)

--CREATE TABLE Table1
--(
--	Id		INTEGER		PRIMARY KEY ASC,
--	Name	VARCHAR(100)
--)

--CREATE TABLE Table1
--(
--	Id		INTEGER		PRIMARY KEY ASC,
--	Name	VARCHAR(100)
--)"
);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}