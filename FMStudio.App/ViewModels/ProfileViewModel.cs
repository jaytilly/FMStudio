﻿using FMStudio.App.Utility;
using FMStudio.Lib;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FMStudio.App.ViewModels
{
    public class ProfileViewModel : HierarchicalBaseViewModel
    {
        public ProfilesViewModel ProfilesVM { get; set; }

        public ProfileInfo ProfileInfo { get; set; }
        
        public Binding<string> Sql { get; private set; }

        public ICommand RunProfileCommand { get; set; }

        public ProfileViewModel(ProfilesViewModel profilesVM, ProfileInfo profileInfo)
        {
            ProfilesVM = profilesVM;
            ProfileInfo = profileInfo;
            
            Sql = new Binding<string>();

            RunProfileCommand = new RelayCommand(async param => await RunProfileAsync());
        }

        public async Task InitializeAsync()
        {
            try
            {
                await ProfileInfo.InitializeAsync();

                Name.Value = ProfileInfo.Name;
                Sql.Value = ProfileInfo.Sql;
            }
            catch (Exception e)
            {
                ProfilesVM.ProjectVM.RootVM.AppendOutput("Could not load profile '{0}': {1}", ProfileInfo.Name, e.GetFullMessage());
            }
        }

        private async Task RunProfileAsync()
        {
            try
            {
                await ProfileInfo.Run();

                await InitializeAsync();
            }
            catch (Exception e)
            {
                ProfilesVM.ProjectVM.RootVM.AppendOutput("Could not run profile '{0}': {1}", ProfileInfo.Name, e.GetFullMessage());
            }
        }
    }
}