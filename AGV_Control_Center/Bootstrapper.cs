﻿using AGV_Control_Center.Navigation;
using AGV_Control_Center.Views;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AGV_Control_Center
{
    class Bootstrapper :UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.TryResolve<ApplicationShell>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.RegisterTypeForNavigation<Login>(ViewNames.Login);
            Container.RegisterTypeForNavigation<ApplicationExplorer>(ViewNames.ApplicationExplorer);
            Container.RegisterTypeForNavigation<CommandClient>(ViewNames.CommandClient);
            Container.RegisterTypeForNavigation<CommandServer>(ViewNames.CommandServer);
            Container.RegisterTypeForNavigation<AGV_Control_Center_Home>(ViewNames.AGV_Control_Center_Home);
            Container.RegisterTypeForNavigation<ApplicationConfiguration>(ViewNames.ApplicationConfiguration);
        }
    }
}
