using System;

namespace FMStudio.Lib.Exceptions
{
    public class MigrationException : Exception
    {
        public MigrationInfo MigrationInfo { get; private set; }

        public MigrationException(string message, Exception innerException, MigrationInfo migrationInfo)
            : base(message, innerException)
        {
            MigrationInfo = migrationInfo;
        }
    }
}