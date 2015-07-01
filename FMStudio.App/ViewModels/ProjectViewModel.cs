using FMStudio.App.Interfaces;
using FMStudio.App.Utility;
using FMStudio.Configuration;
using FMStudio.Lib;
using FMStudio.Lib.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FMStudio.App.ViewModels
{
    public class ProjectViewModel : BaseViewModel, IHaveAName, ICanBeDragged
    {
        public RootViewModel RootVM { get; private set; }

        public CategoryViewModel ParentCategory { get; set; }

        public MigrationsViewModel MigrationsVM { get; private set; }

        public ProfilesViewModel ProfilesVM { get; private set; }

        public ProjectInfo ProjectInfo { get; private set; }

        public ProjectConfiguration ProjectConfiguration { get; private set; }

        #region Properties

        public ObservableCollection<BaseViewModel> Children { get; set; }

        public Binding<bool> IsNew { get; private set; }

        public Binding<bool> IsInitialized { get; private set; }

        public Binding<string> Name { get; private set; }

        public Binding<int> UnRunMigrationsCount { get; private set; }

        public Binding<bool> HasPendingMigrations { get; private set; }

        public Binding<string> PathToMigrationsDll { get; private set; }

        public Binding<List<DatabaseTypeViewModel>> DatabaseTypes { get; private set; }

        public Binding<DatabaseTypeViewModel> DatabaseType { get; private set; }

        public Binding<string> ConnectionString { get; private set; }

        public Binding<string> Tags { get; private set; }

        public Binding<string> Profile { get; private set; }

        #endregion Properties

        #region Commands

        public ICommand FullUpdateCommand { get; set; }

        public ICommand MigrationsOnlyCommand { get; set; }

        public ICommand ProfilesOnlyCommand { get; set; }

        public ICommand BrowsePathToMigrationsDllCommand { get; private set; }

        public ICommand SaveProjectCommand { get; private set; }

        public ICommand VerifyProjectCommand { get; private set; }

        public ICommand DeleteProjectCommand { get; private set; }

        #endregion Commands

        public ProjectViewModel(RootViewModel root, ProjectConfiguration configProject)
        {
            RootVM = root;
            ProjectConfiguration = configProject;

            Children = new ObservableCollection<BaseViewModel>();

            IsNew = new Binding<bool>();
            IsInitialized = new Binding<bool>();
            Name = new Binding<string>(configProject.Name);
            UnRunMigrationsCount = new Binding<int>();
            HasPendingMigrations = new Binding<bool>();
            PathToMigrationsDll = new Binding<string>();
            DatabaseTypes = new Binding<List<DatabaseTypeViewModel>>();
            DatabaseType = new Binding<DatabaseTypeViewModel>();
            ConnectionString = new Binding<string>();
            Tags = new Binding<string>();
            Profile = new Binding<string>();

            DeleteProjectCommand = new RelayCommand(async param => await DeleteAsync());

            FullUpdateCommand = new RelayCommand(async param => await FullUpdateAsync(), param => !IsNew.Value);
            MigrationsOnlyCommand = new RelayCommand(async param => await RunMigrationsAsync(), param => !IsNew.Value);
            ProfilesOnlyCommand = new RelayCommand(async param => await RunProfilesAsync(), param => !IsNew.Value);

            BrowsePathToMigrationsDllCommand = new RelayCommand(param => BrowsePathToMigrationsDll());

            SaveProjectCommand = new RelayCommand(async param => await SaveAsync());
            VerifyProjectCommand = new RelayCommand(async param => await VerifyAsync());
            DeleteProjectCommand = new RelayCommand(async param => await DeleteAsync());

            DatabaseTypes.Value = DatabaseTypeViewModel.GetDatabaseTypes();

            Children.Add(new BaseViewModel());
        }

        public async Task InitializeAsync()
        {
            Name.Value = ProjectConfiguration.Name;
            PathToMigrationsDll.Value = ProjectConfiguration.DllPath;
            DatabaseType.Value = DatabaseTypes.Value.FirstOrDefault(dbt => dbt.Value == ProjectConfiguration.DatabaseType);
            ConnectionString.Value = ProjectConfiguration.ConnectionString;
            Tags.Value = string.Join(" ", ProjectConfiguration.Tags);
            Profile.Value = ProjectConfiguration.Profile;

            ProjectInfo = new Lib.ProjectInfo(ProjectConfiguration.DllPath, ProjectConfiguration.ConnectionString, ProjectConfiguration.DatabaseType.ToLib())
            {
                Tags = ProjectConfiguration.Tags.ToList(),
                Profile = ProjectConfiguration.Profile
            };

            var outputWriter = new FMStudio.App.Utility.NotifyingOutputWriter();
            outputWriter.OnOutput(output => RootVM.AppendOutput(output));
            ProjectInfo.Output = outputWriter;

            Children.Clear();

            // Migrations
            MigrationsVM = new MigrationsViewModel(this, ProjectInfo);
            Children.Add(MigrationsVM);

            // Profiles
            ProfilesVM = new ProfilesViewModel(this, ProjectInfo);
            Children.Add(ProfilesVM);

            try
            {
                await ProjectInfo.InitializeAsync();

                await MigrationsVM.InitializeAsync();

                await ProfilesVM.InitializeAsync();

                Update();

                IsInitialized.Value = true;

                RootVM.OutputVM.Write("Loaded project '{0}', from assembly {1}, which uses FluentMigrator {2}", ProjectConfiguration.Name, ProjectInfo.Assembly.GetName().Name, ProjectInfo.FMAssembly.Version.ToString());
            }
            catch (InitializeProjectException e)
            {
                RootVM.AppendOutput("Could not initialize project '{0}': {1}", ProjectConfiguration.Name, e.GetFullMessage());
            }
        }

        public void MoveTo(CategoryViewModel category)
        {
            if (ParentCategory != null)
                ParentCategory.Remove(this);

            category.Add(this);
            
            RootVM.Configuration.Save();
        }

        public void Update()
        {
            UnRunMigrationsCount.Value = ProjectInfo.ToBeRunMigrationsCount;
            HasPendingMigrations.Value = ProjectInfo.ToBeRunMigrationsCount > 0;
        }

        private async Task FullUpdateAsync()
        {
            RootVM.AppendOutput("Running full update on project '{0}'...", ProjectConfiguration.Name);

            try
            {
                await ProjectInfo.FullUpdateAsync();
            }
            catch (Exception e)
            {
                RootVM.AppendOutput("Could not run a full update on project '{0}': {1}", ProjectConfiguration.Name, e.GetFullMessage());
            }
        }

        private async Task RunMigrationsAsync()
        {
            RootVM.AppendOutput("Running migrations only on project '{0}'...", ProjectConfiguration.Name);

            try
            {
                await ProjectInfo.RunApplicableMigrationsAsync();
            }
            catch (Exception e)
            {
                RootVM.AppendOutput("Could not run migrations on project '{0}': {1}", ProjectConfiguration.Name, e.GetFullMessage());
            }
        }

        private async Task RunProfilesAsync()
        {
            RootVM.AppendOutput("Running profiles only on project '{0}'...", ProjectConfiguration.Name);

            try
            {
                await ProjectInfo.RunApplicableProfilesAsync();
            }
            catch (Exception e)
            {
                RootVM.AppendOutput("Could not run profiles on project '{0}': {1}", ProjectConfiguration.Name, e.GetFullMessage());
            }
        }

        private void BrowsePathToMigrationsDll()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.DefaultExt = ".dll";
            dialog.Filter = "Dynamic Link Library Files (*.dll)|*.dll|Other Files (*.*)|*.*";

            var result = dialog.ShowDialog();

            if (result == true)
            {
                PathToMigrationsDll.Value = dialog.FileName;
            }
        }

        private async Task SaveAsync()
        {
            if (await Validate())
            {
                await Task.Run(() =>
                {
                    if (ProjectConfiguration == null)
                        ProjectConfiguration = new ProjectConfiguration();

                    ProjectConfiguration.ConnectionString = ConnectionString.Value;
                    ProjectConfiguration.DatabaseType = DatabaseType.Value.Value;
                    ProjectConfiguration.DllPath = PathToMigrationsDll.Value;
                    ProjectConfiguration.Name = Name.Value;
                    ProjectConfiguration.Profile = Profile.Value;

                    if (!string.IsNullOrWhiteSpace(Tags.Value))
                        ProjectConfiguration.Tags = Tags.Value.Split(new char[] { ' ' }).ToList();
                    else
                        ProjectConfiguration.Tags = new List<string>();
                });

                //if (!ProjectConfiguration..Contains(ProjectConfiguration))
                //    RootVM.Configuration.Projects.Add(ProjectConfiguration);

                RootVM.Configuration.Save();

                IsNew.Value = false;

                await InitializeAsync();
            }
        }

        private async Task VerifyAsync()
        {
            await Validate();
        }

        private async Task DeleteAsync()
        {
            await Task.Run(() =>
            {
                RootVM.AppendOutput("Deleting project...");

                ParentCategory.Children.Remove(this);
            });
            
            RootVM.ActiveEntity.Value = null;
        }

        private async Task<bool> Validate()
        {
            // TODO: Redirect to a nice list of validation messages, which get displayed in the view
            if (string.IsNullOrWhiteSpace(Name.Value))
            {
                RootVM.AppendOutput("The 'Name' field is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(ConnectionString.Value))
            {
                RootVM.AppendOutput("The 'Connectionstring' field is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(PathToMigrationsDll.Value))
            {
                RootVM.AppendOutput("The 'Path to migrations dll' field is required.");
                return false;
            }

            if (DatabaseType.Value == null)
            {
                RootVM.AppendOutput("The 'Database type' field is required.");
                return false;
            }

            return true;
        }
    }
}