﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="App.xaml.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui
{
    using System;
    using System.Windows;
    using Autofac;
    using ChocolateyGui.IoC;
    using ChocolateyGui.Services;
    using ChocolateyGui.Utilities.Extensions;
    using ChocolateyGui.Views.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        static App()
        {
            Container = AutoFacConfiguration.RegisterAutoFac();

            Log = typeof(App).GetLogger();

            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            Log.Info("Starting...");
        }

        internal static IContainer Container { get; private set; }

        private static ILogService Log { get; set; }

        protected override void OnExit(ExitEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            Log.InfoFormat("Exiting with code {0}.", e.ApplicationExitCode);
            Log.ForceFlush();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = Container.Resolve<MainWindow>();
            MainWindow = mainWindow;
            MainWindow.Show();
        }

        // Monkey patch for confliciting versions of Reactive in Chocolatey and ChocolateyGUI.
        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name ==
                "System.Reactive.PlatformServices, Version=0.9.10.0, Culture=neutral, PublicKeyToken=79d02ea9cad655eb")
            {
                return typeof(chocolatey.Lets).Assembly;
            }
            else
            {
                return null;
            }
        }

        private static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            Log.Debug("First Chance Exception", e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                Log.Fatal("Unhandled Exception", e.ExceptionObject as Exception);
                MessageBox.Show(
                    e.ExceptionObject.ToString(),
                    "Unhandled Exception",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK,
                    MessageBoxOptions.ServiceNotification);
            }
            else
            {
                Log.Error("Unhandled Exception", e.ExceptionObject as Exception);
            }
        }
    }
}