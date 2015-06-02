using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using System.Collections.Generic;
using System.Reflection;

namespace FMStudio.Lib.Utility
{
    public class StubMigrationContext : IMigrationContext
    {
        public StubMigrationContext()
        {
            Expressions = new List<IMigrationExpression>();
        }

        public object ApplicationContext { get; set; }

        public string Connection { get; set; }

        public IMigrationConventions Conventions { get; set; }

        public ICollection<IMigrationExpression> Expressions { get; set; }

        public Assembly MigrationAssembly { get; set; }

        public IQuerySchema QuerySchema { get; set; }

        public IAssemblyCollection MigrationAssemblies { get; set; }
    }
}