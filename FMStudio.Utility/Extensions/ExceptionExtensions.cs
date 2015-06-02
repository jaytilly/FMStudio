using System;
using System.Text;

namespace FMStudio.Utility
{
    public static class ExceptionExtensions
    {
        public static Exception UnwrapAggregateException(this Exception e)
        {
            if (e.GetType() == typeof(AggregateException))
            {
                return e.InnerException;
            }

            return e;
        }

        public static string GetFullMessage(this Exception e)
        {
            var sb = new StringBuilder();

            do
            {
                sb.AppendLine(e.Message);

                e = e.InnerException;
            }
            while (e != null);

            return sb.ToString();
        }
    }
}