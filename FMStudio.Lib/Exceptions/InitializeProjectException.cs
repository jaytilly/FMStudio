using System;

namespace FMStudio.Lib.Exceptions
{
    public class InitializeProjectException : Exception
    {
        public ExceptionType ExceptionType { get; set; }

        public InitializeProjectException(string message)
            : base(message)
        {
        }

        public InitializeProjectException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public InitializeProjectException(ExceptionType exceptionType)
            : base(exceptionType.ToString())
        {
            ExceptionType = exceptionType;
        }

        public InitializeProjectException(ExceptionType exceptionType, Exception innerException)
            : base(exceptionType.ToString(), innerException)
        {
            ExceptionType = exceptionType;
        }

        public InitializeProjectException(ExceptionType exceptionType, string message)
            : base(message)
        {
            ExceptionType = exceptionType;
        }

        public InitializeProjectException(ExceptionType exceptionType, string message, params string[] args)
            : base(string.Format(message, args))
        {
            ExceptionType = exceptionType;
        }
    }
}