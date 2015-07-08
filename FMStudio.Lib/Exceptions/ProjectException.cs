using System;

namespace FMStudio.Lib.Exceptions
{
    public class ProjectException : Exception
    {
        public ProjectInfo ProjectInfo { get; private set; }

        public ProjectException(string message, Exception innerException, ProjectInfo projectInfo)
            : base(message, innerException)
        {
            ProjectInfo = projectInfo;
        }
    }
}