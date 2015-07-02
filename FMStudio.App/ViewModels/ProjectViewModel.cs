using FMStudio.App.Interfaces;
using FMStudio.App.Utility;
using FMStudio.Configuration;
using FMStudio.Lib;
using FMStudio.Lib.Exceptions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FMStudio.App.ViewModels
{
    public class ProjectViewModel : HierarchicalBaseViewModel, ICanBeDragged
    {
        public RootViewModel RootVM { get; private set; }

        public MigrationsViewModel MigrationsVM { get; private set; }

        public ProfilesViewModel ProfilesVM { get; private set; }

        public ProjectInfo ProjectInfo { get; private set; }
        
        #region Properties

        public Guid Id { get; private set; }

        public Binding<bool> IsNew { get; private set; }

        public Binding<bool> IsInitialized { get; private set; }
        
        public Binding<int> UnRunMigrationsCount { get; private set; }

        public Binding<bool> HasPendingMigrations { get; private set; }

        public Binding<string> PathToMigrationsDll { get; private set; }

        public ObservableCollection<DatabaseTypeViewModel> DatabaseTypes { get; private set; }

        public Binding<DatabaseTypeViewModel> DatabaseType { get; private set; }

        public Binding<string> ConnectionString { get; private set; }

        public Binding<string> Tags { get; private set; }

        public Binding<string> Profile { get; private set; }

        #endregion Properties

        #region Commands

        public ICommand FullUpdateCommand { get; private set; }

        public ICommand MigrationsOnlyCommand { get; private set; }

        public ICommand ProfilesOnlyCommand { get; private set; }

        public ICommand BrowsePathToMigrationsDllCommand { get; private set; }

        public ICommand InitializeProjectCommand { get; private set; }

        public ICommand CloneProjectCommand { get; private set; }

        public ICommand DeleteProjectCommand { get; private set; }

        #endregion Commands

        public ProjectViewModel(RootViewModel root, ProjectConfiguration configProject)
        {
            RootVM = root;

            DatabaseTypes = new ObservableCollection<DatabaseTypeViewModel>(DatabaseTypeViewModel.GetDatabaseTypes());

            Id = configProject.Id;

            IsNew = new Binding<bool>();
            IsInitialized = new Binding<bool>();
            IsNodeExpanded.Value = configProject.IsExpanded;
            UnRunMigrationsCount = new Binding<int>();
            HasPendingMigrations = new Binding<bool>();

            Name = new Binding<string>(configProject.Name);
            ConnectionString = new Binding<string>(configProject.ConnectionString);
            PathToMigrationsDll = new Binding<string>(configProject.DllPath);

            DatabaseType = new Binding<DatabaseTypeViewModel>();
            if (configProject.DatabaseType.HasValue)
                DatabaseType.Value = DatabaseTypes.FirstOrDefault(d => d.Value == configProject.DatabaseType);

            Tags = new Binding<string>();
            if (configProject.Tags != null)
                Tags.Value = string.Join(" ", configProject.Tags);

            Profile = new Binding<string>(configProject.Profile);

            FullUpdateCommand = new RelayCommand(async param => await FullUpdateAsync(), param => !IsNew.Value);
            MigrationsOnlyCommand = new RelayCommand(async param => await RunMigrationsAsync(), param => !IsNew.Value);
            ProfilesOnlyCommand = new RelayCommand(async param => await RunProfilesAsync(), param => !IsNew.Value);

            BrowsePathToMigrationsDllCommand = new RelayCommand(param => BrowsePathToMigrationsDll());
            InitializeProjectCommand = new RelayCommand(async param => await InitializeAsync());
            CloneProjectCommand = new RelayCommand(param => Clone());
            DeleteProjectCommand = new RelayCommand(param => Delete());

            ProjectInfo = new ProjectInfo();

            // Migrations
            MigrationsVM = new MigrationsViewModel(this);
            MigrationsVM.IsNodeExpanded.Value = configProject.IsMigrationsExpanded;
            Add(MigrationsVM);

            // Profiles
            ProfilesVM = new ProfilesViewModel(this);
            ProfilesVM.IsNodeExpanded.Value = configProject.IsProfilesExpanded;
            Add(ProfilesVM);
        }

        public override async Task InitializeAsync()
        {
            if (!PathToMigrationsDll.HasValue)
            {
                RootVM.AppendOutput("No path to a migrations assembly has been specified.");
                return;
            }

            if (!ConnectionString.HasValue)
            {
                RootVM.AppendOutput("No connection string has been specified.");
                return;
            }

            if (!DatabaseType.HasValue)
            {
                RootVM.AppendOutput("No database type has been specified.");
                return;
            }

            ProjectInfo.PathToMigrationsDll = PathToMigrationsDll.Value;
            ProjectInfo.ConnectionString = ConnectionString.Value;
            ProjectInfo.DatabaseType = DatabaseType.Value.Value.ToLib();
            ProjectInfo.Profile = Profile.Value;

            if (Tags.HasValue)
                ProjectInfo.Tags = Tags.Value.Split(new char[] { ' ' }).ToList();

            var outputWriter = new FMStudio.App.Utility.NotifyingOutputWriter();
            outputWriter.OnOutput(output => RootVM.AppendOutput(output));
            ProjectInfo.Output = outputWriter;
            
            try
            {
                await ProjectInfo.InitializeAsync();

                await MigrationsVM.InitializeAsync();

                await ProfilesVM.InitializeAsync();
                
                Update();

                IsInitialized.Value = true;

                RootVM.OutputVM.Write("Loaded project '{0}', from assembly {1}, which uses FluentMigrator {2}", Name.Value, ProjectInfo.Assembly.GetName().Name, ProjectInfo.FMAssembly.Version.ToString());
            }
            catch (InitializeProjectException e)
            {
                RootVM.AppendOutput("Could not initialize project '{0}': {1}", Name.Value, e.GetFullMessage());
            }
        }

        public override int CompareTo(object obj)
        {
            var categoryVM = obj as CategoryViewModel;
            if (categoryVM != null)
                return 1;

            return base.CompareTo(obj);
        }

        public void Update()
        {
            UnRunMigrationsCount.Value = ProjectInfo.ToBeRunMigrationsCount;
            HasPendingMigrations.Value = ProjectInfo.ToBeRunMigrationsCount > 0;
        }

        private async Task FullUpdateAsync()
        {
            RootVM.AppendOutput("Running full update on project '{0}'...", Name.Value);

            try
            {
                await ProjectInfo.FullUpdateAsync();
            }
            catch (Exception e)
            {
                RootVM.AppendOutput("Could not run a full update on project '{0}': {1}", Name.Value, e.GetFullMessage());
            }
        }

        private async Task RunMigrationsAsync()
        {
            RootVM.AppendOutput("Running migrations only on project '{0}'...", Name.Value);

            try
            {
                await ProjectInfo.RunApplicableMigrationsAsync();
            }
            catch (Exception e)
            {
                RootVM.AppendOutput("Could not run migrations on project '{0}': {1}", Name.Value, e.GetFullMessage());
            }
        }

        private async Task RunProfilesAsync()
        {
            RootVM.AppendOutput("Running profiles only on project '{0}'...", Name.Value);

            try
            {
                await ProjectInfo.RunApplicableProfilesAsync();
            }
            catch (Exception e)
            {
                RootVM.AppendOutput("Could not run profiles on project '{0}': {1}", Name.Value, e.GetFullMessage());
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

        private void Clone()
        {
            Parent.Add(new ProjectViewModel(RootVM, ToConfiguration()));
        }

        private void Delete()
        {
            RootVM.AppendOutput("Deleting project...");

            Parent.Remove(this);
            RootVM.ActiveEntity.Value = null;
        }

        public ProjectConfiguration ToConfiguration()
        {
            var result = new ProjectConfiguration()
            {
                Id = Id,
                ConnectionString = ConnectionString.Value,
                DllPath = PathToMigrationsDll.Value,
                IsExpanded = IsNodeExpanded.Value,
                IsMigrationsExpanded = MigrationsVM.IsNodeExpanded.Value,
                IsProfilesExpanded = ProfilesVM.IsNodeExpanded.Value,
                Name = Name.Value,
                Profile = Profile.Value
            };

            if (DatabaseType.HasValue)
                result.DatabaseType = DatabaseType.Value.Value;

            if (Tags.HasValue)
                result.Tags = Tags.Value.Split(new char[] { ' ' }).ToList();

            return result;
        }
    }
}