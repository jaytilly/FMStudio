using System;

namespace FMStudio.Lib.Exceptions
{
    public class MigrateUpFailedException : Exception
    {
        public MigrationInfo MigrationInfo { get; private set; }

        public MigrateUpFailedException(string message, Exception innerException, MigrationInfo migrationInfo)
            : base(message, innerException)
        {
            MigrationInfo = migrationInfo;
        }
    }
}