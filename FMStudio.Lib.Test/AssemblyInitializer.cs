using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FMStudio.Lib.Test
{
    [TestClass]
    public static class AssemblyInitializer
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            Database.Cleanup();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Database.Cleanup();
        }
    }
}