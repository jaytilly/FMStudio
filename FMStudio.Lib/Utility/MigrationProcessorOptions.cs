using FluentMigrator;
using FluentMigrator.Runner.Initialization;

namespace FMStudio.Lib.Utility
{
    public class MigrationProcessorOptions : IMigrationProcessorOptions
    {
        public bool PreviewOnly { get; set; }

        public string ProviderSwitches { get; set; }

        public int? Timeout { get; set; }

        public MigrationProcessorOptions(RunnerContext runnerContext)
        {
            PreviewOnly = runnerContext.PreviewOnly;
            ProviderSwitches = runnerContext.ProviderSwitches;
            Timeout = runnerContext.Timeout;
        }
    }
}