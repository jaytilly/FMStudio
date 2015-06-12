﻿using FMStudio.App.Utility;
using FMStudio.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FMStudio.App.ViewModels
{
    public class RootViewModel : NotifyPropertyChanged
    {
        public ObservableCollection<ProjectViewModel> Projects { get; set; }

        public Binding<BaseViewModel> ActiveEntity { get; set; }

        public ICommand AddProjectCommand { get; private set; }

        public ICommand EditPreferencesCommand { get; private set; }

        public ICommand FullUpdateCommand { get; private set; }

        public ICommand SelectActiveEntityCommand { get; private set; }

        public FMConfiguration Configuration { get; set; }

        public List<FMStudio.Lib.ProjectInfo> LibProjects { get; set; }

        public Binding<string> Output { get; set; }

        public RootViewModel(FMConfiguration configuration)
        {
            ActiveEntity = new Binding<BaseViewModel>();
            Output = new Binding<string>();

            AddProjectCommand = new RelayCommand(param => AddProject());
            EditPreferencesCommand = new RelayCommand(param => EditPreferences());
            FullUpdateCommand = new RelayCommand(async param => await FullUpdateAsync());
            SelectActiveEntityCommand = new RelayCommand(param => SelectActiveEntity(param));

            Configuration = configuration;
            Projects = new ObservableCollection<ProjectViewModel>();
        }

        public async Task InitializeAsync()
        {
            Projects.Clear();

            foreach (var project in Configuration.Projects)
            {
                AppendOutput("Loading project '{0}'", project.Name);

                var projectVM = new ProjectViewModel(this, project);
                Projects.Add(projectVM);

                await projectVM.InitializeAsync();
            }
        }

        public void AppendOutput(string output)
        {
            Output.Value = output + Environment.NewLine + Output.Value;
        }

        public void AppendOutput(string format, params object[] args)
        {
            Output.Value = string.Format(format, args) + Environment.NewLine + Output.Value;
        }

        private void AddProject()
        {
            var projectVM = new ProjectViewModel(this, new ProjectConfiguration());
            projectVM.IsNew.Value = true;
            Projects.Add(projectVM);

            ActiveEntity.Value = projectVM;
        }

        private void EditPreferences()
        {
            ActiveEntity.Value = new PreferencesViewModel(this);
        }

        private async Task FullUpdateAsync()
        {
            foreach (var project in Projects)
            {
                try
                {
                    await project.ProjectInfo.FullUpdateAsync();
                }
                catch (Exception e)
                {
                    AppendOutput("Could not run a full update on project '{0}': {1}", project.Name.Value, e.Message);
                }
            }
        }

        private void SelectActiveEntity(object param)
        {
            var migrationEntity = param as BaseViewModel;
            if (migrationEntity != null)
            {
                ActiveEntity.Value = migrationEntity;
            }
        }
    }
}