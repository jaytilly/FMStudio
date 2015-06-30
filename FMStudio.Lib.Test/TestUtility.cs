using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FMStudio.Lib.Test
{
    public static class TestUtility
    {
        public static void AssertExceptionThrown<T>(Action action) where T : Exception
        {
            AssertExceptionThrown<T>(action, null);
        }

        public static void AssertExceptionThrown<T>(Action action, Action<T> onException) where T : Exception
        {
            var expected = typeof(T);

            try
            {
                action();
            }
            catch (Exception e)
            {
                var actual = e.GetType();

                if (actual == typeof(AggregateException))
                {
                    e = e.InnerException;
                    actual = e.GetType();
                }

                if (expected != actual)
                {
                    Assert.Fail("Expected exception of type '" + expected + "', but got exception of type '" + actual + "'");
                }

                if (onException != null)
                {
                    onException(e as T);
                }
            }
        }
    }
}