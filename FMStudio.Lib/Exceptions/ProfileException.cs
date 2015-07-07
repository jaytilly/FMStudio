using System;

namespace FMStudio.Lib.Exceptions
{
    public class ProfileException : Exception
    {
        public ProfileInfo ProfileInfo { get; private set; }

        public ProfileException(string message, Exception innerException, ProfileInfo migrationInfo)
            : base(message, innerException)
        {
            ProfileInfo = migrationInfo;
        }
    }
}