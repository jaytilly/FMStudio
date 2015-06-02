using System;

namespace FMStudio.Lib.Exceptions
{
    public class FMStudioLibException : Exception
    {
        public FMStudioLibException(string message)
            : base(message)
        { }
    }
}