﻿using AGV_Control_Center.Events;
using AGV_Control_Center.Navigation;
using AGV_Control_Center.Views;
using CommonLibraries.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AGV_Control_Center.ViewModels
{
    class ApplicationShellViewModel : BindableBase, IRegionMemberLifetime, INavigationAware
    {
        private readonly RegionManager _regionManager;
        private readonly EventAggregator _eventAggregator;


        private string title;

        private SQLCommunicator sqldbCommunicator = new SQLCommunicator();
        private DispatcherTimer totalElapsedTimerMVVM = new DispatcherTimer();

        private string currentTimePorperty;

        public string CurrentTimeProperty
        {
            get { return currentTimePorperty; }
            set { SetProperty( ref currentTimePorperty, value); }
        }

        public bool KeepAlive
        {
            get
            {
                return true;
            }
        }
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private ApplicationUser user;

        public ApplicationUser UserProperty
        {
            get { return user; }
            set { SetProperty(ref user, value); }
        }


        public DelegateCommand ExitCommand { get; set; }

        public ApplicationShellViewModel(RegionManager regionManager, EventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            _eventAggregator.GetEvent<UserCredentialsDTO>().Subscribe(LoadUserCredentials);

            ExitCommand = new DelegateCommand(exExitCmd);

            Title = "AGV Control Center";

            _regionManager.RegisterViewWithRegion(RegionNames.PrimaryContentRegion, typeof(Login));

            totalElapsedTimerMVVM.Interval = TimeSpan.FromMilliseconds(300);
            totalElapsedTimerMVVM.Tick += new EventHandler(totalElapsedTimerMVVM_Tick);
            totalElapsedTimerMVVM.Start();
        }

        private void totalElapsedTimerMVVM_Tick(object sender, EventArgs e)
        {
            CurrentTimeProperty = "Current Date and Time: " + DateTime.Now.ToString();
        }

        private void exExitCmd()
        {
            if (UserProperty != null)
            {
                UserProperty.LogOut = DateTime.Now;
                sqldbCommunicator.LogUserOUT(UserProperty);
            }
            
            totalElapsedTimerMVVM.Stop();
            Application.Current.Shutdown();
        }
        private void LoadUserCredentials(UserCredentialsDTO obj)
        {
            UserProperty = obj.User;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            UserProperty = (ApplicationUser)navigationContext.Parameters[typeof(ApplicationUser).Name] ?? UserProperty;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            navigationContext.Parameters.Add(typeof(ApplicationUser).Name, UserProperty);
        }
    }
}
