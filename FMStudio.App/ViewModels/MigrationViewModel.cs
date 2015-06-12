﻿using FMStudio.App.Utility;
using FMStudio.Lib;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FMStudio.App.ViewModels
{
    public class MigrationViewModel : BaseViewModel
    {
        public MigrationsViewModel MigrationsVM { get; set; }

        public MigrationInfo MigrationInfo { get; set; }

        public Binding<string> Description { get; private set; }

        public Binding<long> Version { get; set; }

        public ObservableCollection<TagViewModel> Tags { get; private set; }

        public Binding<bool> HasRun { get; set; }

        public Binding<string> Sql { get; set; }

        public ICommand AddToDatabaseCommand { get; set; }

        public ICommand MigrateDownCommand { get; set; }

        public ICommand MigrateUpCommand { get; set; }

        public ICommand RemoveFromDatabaseCommand { get; set; }

        public ICommand ReRunCommand { get; set; }

        public MigrationViewModel(MigrationsViewModel migrationsVM, MigrationInfo migrationInfo)
        {
            MigrationsVM = migrationsVM;
            MigrationInfo = migrationInfo;
            MigrationInfo.OnUpdate += (s, e) => Update();

            Description = new Binding<string>();
            Version = new Binding<long>();
            Tags = new ObservableCollection<TagViewModel>();
            HasRun = new Binding<bool>();
            Sql = new Binding<string>();

            AddToDatabaseCommand = new RelayCommand(async param => await AddToDatabaseAsync());
            MigrateDownCommand = new RelayCommand(async param => await MigrateDownAsync());
            MigrateUpCommand = new RelayCommand(async param => await MigrateUpAsync());
            ReRunCommand = new RelayCommand(async param => await ReRunMigrateUpAsync());
            RemoveFromDatabaseCommand = new RelayCommand(async param => await RemoveFromDatabaseAsync());
        }

        public async Task InitializeAsync()
        {
            try
            {
                await MigrationInfo.InitializeAsync();

                Update();
            }
            catch (Exception e)
            {
                MigrationsVM.ProjectVM.RootVM.AppendOutput("Could not initialized migration {0} '{1}': ", Version.Value, Description.Value, e.Message);
            }
        }

        private void Update()
        {
            Version.Value = MigrationInfo.Version;
            Description.Value = MigrationInfo.Description;
            Sql.Value = MigrationInfo.Sql;
            HasRun.Value = MigrationInfo.HasRun;

            Tags.Clear();

            if (MigrationInfo.Tags != null)
            {
                foreach (var tag in MigrationInfo.Tags)
                {
                    Tags.Add(new TagViewModel()
                    {
                        Name = tag
                    });
                }
            }
        }

        private async Task AddToDatabaseAsync()
        {
            try
            {
                await MigrationInfo.AddToVersionInfoTableAsync();

                await InitializeAsync();
            }
            catch (Exception e)
            {
                MigrationsVM.ProjectVM.RootVM.AppendOutput("Could not add migration to database without running it: {0}", e.Message);
            }
        }

        private async Task MigrateDownAsync()
        {
            try
            {
                await MigrationInfo.DownAsync();

                await InitializeAsync();
            }
            catch (Exception e)
            {
                MigrationsVM.ProjectVM.RootVM.AppendOutput("Could not run migration Down-operation: {0}", e.Message);
            }
        }

        private async Task MigrateUpAsync()
        {
            try
            {
                await MigrationInfo.UpAsync(false);

                await InitializeAsync();
            }
            catch (Exception e)
            {
                MigrationsVM.ProjectVM.RootVM.AppendOutput("Could not run migration Up-operation: {0}", e.Message);
            }
        }

        private async Task RemoveFromDatabaseAsync()
        {
            try
            {
                await MigrationInfo.DeleteFromVersionInfoTableAsync();

                await InitializeAsync();
            }
            catch (Exception e)
            {
                MigrationsVM.ProjectVM.RootVM.AppendOutput("Could not remove migration from database: {0}", e.Message);
            }
        }

        private async Task ReRunMigrateUpAsync()
        {
            try
            {
                await MigrationInfo.UpAsync(true);

                await InitializeAsync();
            }
            catch (Exception e)
            {
                MigrationsVM.ProjectVM.RootVM.AppendOutput("Could not re-run migration: {0}", e.Message);
            }
        }
    }
}