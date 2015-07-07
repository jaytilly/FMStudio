using FMStudio.App.Utility;
using FMStudio.Lib;
using FMStudio.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FMStudio.App.ViewModels
{
    public class MigrationViewModel : HierarchicalBaseViewModel
    {
        private ILog _log;

        public MigrationsViewModel MigrationsVM { get; set; }

        public MigrationInfo MigrationInfo { get; set; }

        public Binding<string> Description { get; private set; }

        public Binding<long> Version { get; set; }

        public ObservableCollection<TagViewModel> Tags { get; private set; }

        public Binding<bool> HasRun { get; set; }

        public Binding<bool> IsToBeRun { get; set; }

        public Binding<bool> IsSkipped { get; set; }

        public Binding<DateTime?> AppliedOn { get; set; }

        public Binding<string> Sql { get; set; }

        public ICommand AddToDatabaseCommand { get; private set; }

        public ICommand MigrateDownCommand { get; private set; }

        public ICommand MigrateUpCommand { get; private set; }

        public ICommand RemoveFromDatabaseCommand { get; private set; }

        public ICommand ReRunCommand { get; private set; }

        public ICommand SaveToFileCommand { get; private set; }

        public MigrationViewModel(ILog log, MigrationsViewModel migrationsVM, MigrationInfo migrationInfo)
        {
            _log = log;

            MigrationsVM = migrationsVM;
            MigrationInfo = migrationInfo;
            MigrationInfo.MigrationUpdated += (s, e) => Update();

            Description = new Binding<string>();
            Version = new Binding<long>();
            Tags = new ObservableCollection<TagViewModel>();
            HasRun = new Binding<bool>();
            IsToBeRun = new Binding<bool>();
            IsSkipped = new Binding<bool>();
            AppliedOn = new Binding<DateTime?>();
            Sql = new Binding<string>(() => MigrationInfo.GetSqlAsync().Result); // TODO: Make nicer

            AddToDatabaseCommand = new RelayCommand(async param => await AddToDatabaseAsync());
            MigrateDownCommand = new RelayCommand(async param => await MigrateDownAsync());
            MigrateUpCommand = new RelayCommand(async param => await MigrateUpAsync());
            ReRunCommand = new RelayCommand(async param => await ReRunMigrateUpAsync());
            RemoveFromDatabaseCommand = new RelayCommand(async param => await RemoveFromDatabaseAsync());
            SaveToFileCommand = new RelayCommand(param => SaveToFile());
        }

        public override async Task InitializeAsync()
        {
            try
            {
                await MigrationInfo.InitializeAsync();
            }
            catch (Exception e)
            {
                _log.Error("Could not initialized migration {0} '{1}': ", Version.Value, Description.Value, e.GetFullMessage());
            }
        }

        private void Update()
        {
            Version.Value = MigrationInfo.Version;
            Description.Value = MigrationInfo.Description;

            if (MigrationInfo.AppliedOn.HasValue)
                AppliedOn.Value = MigrationInfo.AppliedOn.Value;

            HasRun.Value = MigrationInfo.HasRun;

            Tags.ClearOnDispatcher();

            if (MigrationInfo.Tags != null)
                MigrationInfo.Tags.ForEach(t => Tags.AddOnDispatcher(new TagViewModel() { Name = t }));

            var tagIsIncluded = MigrationInfo.IsToBeRun;
            IsToBeRun.Value = !HasRun.Value && tagIsIncluded;
            IsSkipped.Value = !HasRun.Value && !tagIsIncluded;

            MigrationsVM.ProjectVM.Update();
        }

        private async Task AddToDatabaseAsync()
        {
            try
            {
                await MigrationInfo.AddToVersionInfoTableAsync();
            }
            catch (Exception e)
            {
                _log.Error("Could not add migration to database without running it: {0}", e.GetFullMessage());
            }
        }

        private async Task MigrateDownAsync()
        {
            try
            {
                await MigrationInfo.DownAsync();
            }
            catch (Exception e)
            {
                _log.Error("Could not run migration Down-operation: {0}", e.GetFullMessage());
            }
        }

        private async Task MigrateUpAsync()
        {
            try
            {
                await MigrationInfo.UpAsync(false);
            }
            catch (Exception e)
            {
                _log.Error("Could not run migration Up-operation: {0}", e.GetFullMessage());
            }
        }

        private async Task RemoveFromDatabaseAsync()
        {
            try
            {
                await MigrationInfo.DeleteFromVersionInfoTableAsync();
            }
            catch (Exception e)
            {
                _log.Error("Could not remove migration from database: {0}", e.GetFullMessage());
            }
        }

        private async Task ReRunMigrateUpAsync()
        {
            try
            {
                await MigrationInfo.UpAsync(true);
            }
            catch (Exception e)
            {
                _log.Error("Could not re-run migration: {0}", e.GetFullMessage());
            }
        }

        private void SaveToFile()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();

            dialog.DefaultExt = ".sql";
            dialog.Filter = "SQL files (*.sql)|*.sql|All Files (*.*)|*.*";

            var result = dialog.ShowDialog();

            if (result == true)
            {
                try
                {
                    File.WriteAllText(dialog.FileName, Sql.Value);
                }
                catch (Exception e)
                {
                    _log.Error("Error while writing to file: {0}", e.GetFullMessage());
                }
            }
        }
    }
}