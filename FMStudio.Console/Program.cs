using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors.SqlServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MigrationViewer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var dllPath = @"Migrations.dll";

            var assembly = Assembly.LoadFile(dllPath);
            var fmDll = assembly.GetReferencedAssemblies().FirstOrDefault(a => a.Name.Equals("FluentMigrator", StringComparison.OrdinalIgnoreCase));

            if (fmDll != null)
            {
                Debug.WriteLine("Selected dll is using Fluent Migrator version " + fmDll.Version.ToString());
            }

            var migrations = assembly
                .DefinedTypes
                .Where(t => t.IsSubclassOf(typeof(Migration)))
                .ToList();

            foreach (var mig in migrations)
            {
                var migrationAttr = mig.GetCustomAttribute<MigrationAttribute>();
                var profileAttr = mig.GetCustomAttribute<ProfileAttribute>();
                var sql = GetMigrationSql(assembly, mig.FullName);
                var tagsAttr = mig.GetCustomAttributes<TagsAttribute>();

                if (migrationAttr != null)
                {
                    Console.WriteLine("Migration: " + migrationAttr.Version + ": " + migrationAttr.Description);
                }
                else if (profileAttr != null)
                {
                    Console.WriteLine("Profile: " + profileAttr.ProfileName);
                }

                if (tagsAttr.Any())
                {
                    var tags = tagsAttr.SelectMany(t => t.TagNames).ToList();
                    Console.WriteLine("Tags: " + string.Join(",", tags));
                }

                Console.WriteLine("-- SQL -------------------");
                Console.WriteLine(sql.Substring(0, 100));
                Console.WriteLine("-- /SQL ------------------");

                var run = CheckIfMigrationHasRun(assembly);
            }
        }

        public static string GetMigrationSql(Assembly assembly, string migrationClass)
        {
            var inst = assembly.CreateInstance(migrationClass) as Migration;

            var context = new StubContext();
            context.Expressions = new List<IMigrationExpression>();

            inst.SetFieldValue("_context", context);
            inst.Up();

            return string.Join(Environment.NewLine + "===========", context.Expressions.Select(e => e.ToString()).ToList());
        }

        private class MigrationProcessorOptions : IMigrationProcessorOptions
        {
            public bool PreviewOnly { get; set; }

            public string ProviderSwitches { get; set; }

            public int Timeout { get; set; }

            public MigrationProcessorOptions(RunnerContext runnerContext)
            {
                PreviewOnly = runnerContext.PreviewOnly;
                ProviderSwitches = runnerContext.ProviderSwitches;
                Timeout = runnerContext.Timeout;
            }
        }

        public class MigrationInfo
        {
            public long Version { get; set; }

            public string Description { get; set; }

            public bool HasRun { get; set; }
        }

        public static IEnumerable<MigrationInfo> CheckIfMigrationHasRun(Assembly migrationAssembly)
        {
            var announcer = new TextWriterAnnouncer(s => System.Diagnostics.Debug.WriteLine(s));

            var migrationContext = new RunnerContext(announcer)
            {
                Tags = new string[0],
                PreviewOnly = true
            };

            var factory = new SqlServer2008ProcessorFactory();
            var processor = factory.Create("server=.;database=BSContracteren;integrated security=true;", announcer, new MigrationProcessorOptions(migrationContext));
            var runner = new MigrationRunner(migrationAssembly, migrationContext, processor);

            // Check if theres any migration that wasn't executed (but should have been)
            return runner.MigrationLoader.LoadMigrations()
                    .Select(pair => new MigrationInfo()
                    {
                        Version = pair.Value.Version,
                        Description = pair.Value.Description,
                        HasRun = runner.VersionLoader.VersionInfo.HasAppliedMigration(pair.Key)
                    })
                    .ToList();
        }
    }

    public class StubContext : IMigrationContext
    {
        public object ApplicationContext { get; set; }

        public string Connection { get; set; }

        public IMigrationConventions Conventions { get; set; }

        public ICollection<IMigrationExpression> Expressions { get; set; }

        public Assembly MigrationAssembly { get; set; }

        public IQuerySchema QuerySchema { get; set; }

        public IAssemblyCollection MigrationAssemblies { get; set; }
    }

    public static class ReflectionHelper
    {
        private static FieldInfo GetFieldInfo(Type type, string fieldName)
        {
            FieldInfo fieldInfo;
            do
            {
                fieldInfo = type.GetField(fieldName,
                       BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                type = type.BaseType;
            }
            while (fieldInfo == null && type != null);
            return fieldInfo;
        }

        public static object GetFieldValue(this object obj, string fieldName)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            Type objType = obj.GetType();
            FieldInfo fieldInfo = GetFieldInfo(objType, fieldName);
            if (fieldInfo == null)
                throw new ArgumentOutOfRangeException("fieldName",
                  string.Format("Couldn't find field {0} in type {1}", fieldName, objType.FullName));
            return fieldInfo.GetValue(obj);
        }

        public static void SetFieldValue(this object obj, string fieldName, object val)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            Type objType = obj.GetType();
            FieldInfo fieldInfo = GetFieldInfo(objType, fieldName);
            if (fieldInfo == null)
                throw new ArgumentOutOfRangeException("fieldName",
                  string.Format("Couldn't find field {0} in type {1}", fieldName, objType.FullName));
            fieldInfo.SetValue(obj, val);
        }
    }
}