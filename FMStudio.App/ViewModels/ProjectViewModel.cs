using FMStudio.App.Interfaces;
using FMStudio.App.Utility;
using FMStudio.Configuration;
using FMStudio.Lib;
using FMStudio.Utility.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FMStudio.App.ViewModels
{
    public class ProjectViewModel : HierarchicalBaseViewModel, ICanBeDragged
    {
        private ILog _log;

        public RootViewModel RootVM { get; private set; }

        public MigrationsViewModel MigrationsVM { get; private set; }

        public ProfilesViewModel ProfilesVM { get; private set; }

        public ProjectInfo ProjectInfo { get; private set; }

        #region Properties

        public Guid Id { get; private set; }

        public string AutomationId { get { return "trviProject_" + Id.ToString(); } }

        public Binding<bool> IsNew { get; private set; }

        public Binding<bool> IsInitialized { get; private set; }

        public Binding<bool> IsReadOnly { get; private set; }

        public Binding<bool> IsInProgress { get; private set; }

        public Binding<int> PendingMigrationsCount { get; private set; }

        public Binding<bool> HasPendingMigrations { get; private set; }

        public Binding<int> MigrationsCount { get; private set; }

        public Binding<int> ProfilesCount { get; private set; }

        public Binding<string> PathToMigrationsFile { get; private set; }

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

        public ICommand BrowsePathToMigrationsFileCommand { get; private set; }

        public ICommand InitializeProjectCommand { get; private set; }

        public ICommand CloneProjectCommand { get; private set; }

        public ICommand DeleteProjectCommand { get; private set; }

        #endregion Commands

        public ProjectViewModel(ILog log, RootViewModel root, ProjectConfiguration configProject)
        {
            _log = log;

            RootVM = root;

            DatabaseTypes = new ObservableCollection<DatabaseTypeViewModel>(DatabaseTypeViewModel.GetDatabaseTypes());

            Id = configProject.Id;

            IsNew = new Binding<bool>();
            IsInitialized = new Binding<bool>();
            IsReadOnly = new Binding<bool>(configProject.IsReadOnly);
            IsInProgress = new Binding<bool>();
            IsNodeExpanded.Value = configProject.IsExpanded;
            PendingMigrationsCount = new Binding<int>();
            HasPendingMigrations = new Binding<bool>();
            MigrationsCount = new Binding<int>();
            ProfilesCount = new Binding<int>();

            Name = new Binding<string>(configProject.Name);
            ConnectionString = new Binding<string>(configProject.ConnectionString);
            PathToMigrationsFile = new Binding<string>(configProject.DllPath);

            DatabaseType = new Binding<DatabaseTypeViewModel>();
            if (configProject.DatabaseType.HasValue)
                DatabaseType.Value = DatabaseTypes.FirstOrDefault(d => d.Value == configProject.DatabaseType);

            Tags = new Binding<string>();
            if (configProject.Tags != null)
                Tags.Value = string.Join(" ", configProject.Tags);

            Profile = new Binding<string>(configProject.Profile);

            FullUpdateCommand = new RelayCommand(async param => await FullUpdateAsync(), param => !IsReadOnly.Value && IsInitialized.Value);
            MigrationsOnlyCommand = new RelayCommand(async param => await RunMigrationsAsync(), param => !IsReadOnly.Value && IsInitialized.Value);
            ProfilesOnlyCommand = new RelayCommand(async param => await RunProfilesAsync(), param => !IsReadOnly.Value && IsInitialized.Value);

            BrowsePathToMigrationsFileCommand = new RelayCommand(param => BrowsePathToMigrationsDll());
            InitializeProjectCommand = new RelayCommand(async param => await InitializeAsync());
            CloneProjectCommand = new RelayCommand(param => Clone());
            DeleteProjectCommand = new RelayCommand(async param => await DeleteAsync(), param => !IsReadOnly.Value);

            ProjectInfo = new ProjectInfo(null, null, null, RootVM.OutputVM);

            // Migrations
            MigrationsVM = new MigrationsViewModel(_log, this);
            MigrationsVM.IsNodeExpanded.Value = configProject.IsMigrationsExpanded;
            Add(MigrationsVM);

            // Profiles
            ProfilesVM = new ProfilesViewModel(_log, this);
            ProfilesVM.IsNodeExpanded.Value = configProject.IsProfilesExpanded;
            Add(ProfilesVM);
        }

        public override async Task InitializeAsync()
        {
            if (!PathToMigrationsFile.HasValue)
            {
                _log.Warning("No path to a migrations assembly has been specified.");
                return;
            }

            if (!ConnectionString.HasValue)
            {
                _log.Warning("No connection string has been specified.");
                return;
            }

            if (!DatabaseType.HasValue)
            {
                _log.Warning("No database type has been specified.");
                return;
            }

            IsInProgress.Value = true;

            ProjectInfo.Name = Name.Value;
            ProjectInfo.Profile = Profile.Value;

            if (Tags.HasValue)
                ProjectInfo.Tags = Tags.Value.Split(new char[] { ' ' }).ToList();

            try
            {
                await ProjectInfo.InitializeMigrationsAsync(PathToMigrationsFile.Value);

                await ProjectInfo.InitializeDatabase(DatabaseType.Value.Value.ToLib(), ConnectionString.Value);

                await MigrationsVM.InitializeAsync();

                await ProfilesVM.InitializeAsync();

                await Update();

                IsInitialized.Value = true;
            }
            catch (Exception e)
            {
                _log.Error("Could not initialize project '{0}': {1}", Name.Value, e.GetFullMessage());
            }

            IsInProgress.Value = false;
        }

        public override int CompareTo(object obj)
        {
            var categoryVM = obj as CategoryViewModel;
            if (categoryVM != null)
                return 1;

            return base.CompareTo(obj);
        }

        public async Task Update()
        {
            PendingMigrationsCount.Value = ProjectInfo.ToBeRunMigrationsCount;
            HasPendingMigrations.Value = ProjectInfo.ToBeRunMigrationsCount > 0;

            MigrationsCount.Value = ProjectInfo.Migrations.Count;
            ProfilesCount.Value = ProjectInfo.Profiles.Count;

            await RootVM.UpdateHasPendingMigrations();
        }

        public async Task FullUpdateAsync()
        {
            if (MigrationsVM.ProjectVM.IsReadOnly.Value)
                return;

            IsInProgress.Value = true;

            try
            {
                await ProjectInfo.FullUpdateAsync();
            }
            catch (Exception e)
            {
                _log.Error("Could not run a full update on project '{0}': {1}", Name.Value, e.GetFullMessage());
            }

            IsInProgress.Value = false;
        }

        public async Task RunMigrationsAsync()
        {
            if (MigrationsVM.ProjectVM.IsReadOnly.Value)
                return;

            IsInProgress.Value = true;

            try
            {
                await ProjectInfo.RunApplicableMigrationsAsync();
            }
            catch (Exception e)
            {
                _log.Error("Could not run migrations on project '{0}': {1}", Name.Value, e.GetFullMessage());
            }

            IsInProgress.Value = false;
        }

        public async Task RunProfilesAsync()
        {
            if (MigrationsVM.ProjectVM.IsReadOnly.Value)
                return;

            IsInProgress.Value = true;

            try
            {
                await ProjectInfo.RunApplicableProfilesAsync();
            }
            catch (Exception e)
            {
                _log.Error("Could not run profiles on project '{0}': {1}", Name.Value, e.GetFullMessage());
            }

            IsInProgress.Value = false;
        }

        public void BrowsePathToMigrationsDll()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.DefaultExt = ".dll";
            dialog.Filter = "Dynamic Link Library Files (*.dll)|*.dll|Other Files (*.*)|*.*";

            var result = dialog.ShowDialog();

            if (result == true)
            {
                PathToMigrationsFile.Value = dialog.FileName;
            }
        }

        public void Clone()
        {
            var configuration = ToConfiguration();
            configuration.Id = Guid.NewGuid();

            var clonedVM = new ProjectViewModel(_log, RootVM, configuration);
            Parent.Add(clonedVM);

            RootVM.SelectActiveEntity(clonedVM);
        }

        public async Task DeleteAsync()
        {
            var confirm = await RootVM.DialogService.ConfirmAsync("Confirm project deletion", "Are you sure you want to delete project '" + Name.Value + "'?");
            if (!confirm)
                return;

            Parent.Remove(this);
            RootVM.ActiveEntity.Value = null;

            _log.Info("Deleted project '{0}'", Name);
        }

        public ProjectConfiguration ToConfiguration()
        {
            var result = new ProjectConfiguration()
            {
                Id = Id,
                ConnectionString = ConnectionString.Value,
                DllPath = PathToMigrationsFile.Value,
                IsExpanded = IsNodeExpanded.Value,
                IsMigrationsExpanded = MigrationsVM.IsNodeExpanded.Value,
                IsProfilesExpanded = ProfilesVM.IsNodeExpanded.Value,
                IsReadOnly = IsReadOnly.Value,
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