namespace FMStudio.Lib.Utility
{
    public static class References
    {
        public static void Load()
        {
            var sqlite = typeof(FluentMigrator.Runner.Processors.SQLite.SQLiteProcessorFactory);
        }
    }
}