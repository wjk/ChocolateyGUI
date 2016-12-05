﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AutoFacConfiguration.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.IoC
{
    using System;
    using Autofac;
    using AutoMapper;
    using ChocolateyGui.Providers;
    using ChocolateyGui.Services;
    using ChocolateyGui.ViewModels.Controls;
    using ChocolateyGui.ViewModels.Items;
    using ChocolateyGui.ViewModels.Windows;
    using ChocolateyGui.Views.Controls;
    using ChocolateyGui.Views.Windows;
    using NuGet;

    public static class AutoFacConfiguration
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is really a requirement due to required registrations.")]
        public static IContainer RegisterAutoFac()
        {
            var builder = new ContainerBuilder();

            // Register Providers
            builder.RegisterType<VersionNumberProvider>().As<IVersionNumberProvider>().SingleInstance();

            var configurationProvider = new ChocolateyConfigurationProvider();
            builder.RegisterInstance(configurationProvider).As<IChocolateyConfigurationProvider>().SingleInstance();
            builder.RegisterType<ChocolateyPackageService>().As<IChocolateyPackageService>().SingleInstance();

            // Register View Models
            builder.RegisterType<MainWindowViewModel>().As<IMainWindowViewModel>();
            builder.RegisterType<SourceViewModel>();
            builder.Register(
                (c, parameters) =>
                new SourceTabViewModel(
                    c.Resolve(
                        typeof(Lazy<>).MakeGenericType(parameters.TypedAs<Type>()),
                        new TypedParameter(typeof(Uri), parameters.TypedAs<Uri>())),
                    parameters.TypedAs<string>()));

            builder.RegisterType<SourcesControlViewModel>().As<ISourcesControlViewModel>();
            builder.RegisterType<LocalSourceControlViewModel>().As<ILocalSourceControlViewModel>();
            builder.RegisterType<RemoteSourceControlViewModel>().As<IRemoteSourceControlViewModel>();
            builder.RegisterType<PackageControlViewModel>().As<IPackageControlViewModel>();
            builder.Register(c => new PackageViewModel(c.Resolve<IRemotePackageService>(), c.Resolve<IChocolateyPackageService>(), c.Resolve<INavigationService>(), c.Resolve<IMapper>())).As<IPackageViewModel>();

            // Register Services
            builder.RegisterType<Log4NetLoggingService>().As<ILogService>();
            builder.RegisterType<ChocolateySourcesService>().As<ISourceService>().SingleInstance();
            builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();
            builder.RegisterType<RemotePackageService>().As<IRemotePackageService>().SingleInstance();
            builder.RegisterType<ProgressService>().As<IProgressService>().SingleInstance();
            builder.RegisterType<PersistenceService>().As<IPersistenceService>().SingleInstance();

            // Register Views
            builder.RegisterType<MainWindow>();
            builder.RegisterType<SourcesControl>();
            builder.RegisterType<LocalSourceControl>();
            builder.Register((c, parameters) =>
                new RemoteSourceControl(c.Resolve<IRemoteSourceControlViewModel>(parameters), c.Resolve<Lazy<INavigationService>>()));
            builder.Register((c, pvm) => new PackageControl(c.Resolve<IPackageControlViewModel>(), pvm.TypedAs<PackageViewModel>()));

            // Register Mapper
            var mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<IPackage, IPackageViewModel>();
                config.CreateMap<IPackageViewModel, IPackageViewModel>()
                    .ForMember(vm => vm.IsInstalled, options => options.Ignore());
            });

            builder.RegisterInstance(mapperConfiguration.CreateMapper()).As<IMapper>();

            return builder.Build();
        }
    }
}