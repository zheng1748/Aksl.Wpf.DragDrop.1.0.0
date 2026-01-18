using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Prism.Events;

using Aksl.Toolkit.Dialogs;
using Aksl.Toolkit.Services;

using Aksl.Infrastructure;
using Aksl.Infrastructure.Events;

using Aksl.Modules.Shell;
using Aksl.Modules.Shell.ViewModels;
using Aksl.Modules.Shell.Views;

using Aksl.Modules.HamburgerMenuNavigationSideBar;

using Aksl.Modules.Account;

using Aksl.Modules.Yellows;
using Aksl.Modules.Blues;
using Aksl.Modules.Blacks;

namespace Aksl.Wpf.Unity
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.Register(typeof(ShellView).ToString(), () => Container.Resolve<ShellViewModel>());
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            #region Initialize
            var services = new ServiceCollection();
            services.AddOptions();

            string basePath = Directory.GetCurrentDirectory();
            string configPath = Path.Combine(basePath, "Configuration");
            string appSettingsPath = Path.Combine(configPath, "appsettings.json");
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().SetBasePath(basePath)
                                                                                   .AddJsonFile(path: appSettingsPath, optional: true, reloadOnChange: false);

            var configuration = configurationBuilder.Build();
            #endregion

            #region Logging
            services.AddLogging(builder =>
            {
                var loggingSection = configuration.GetSection("Logging");
                var includeScopes = loggingSection.GetValue<bool>("IncludeScopes");

                builder.AddConfiguration(loggingSection);

                //加入一个ConsoleLoggerProvider
                //builder.AddConsole(consoleLoggerOptions =>
                //{
                //    consoleLoggerOptions.IncludeScopes = includeScopes;
                //});

                //加入一个DebugLoggerProvider
                builder.AddDebug();
            });
            #endregion

            var serviceProvider = services.BuildServiceProvider();

            containerRegistry.RegisterInstance<IServiceProvider>(serviceProvider);

            containerRegistry.RegisterSingleton(typeof(IDialogService), typeof(DialogService));
            containerRegistry.RegisterSingleton(typeof(IDialogViewService), typeof(DialogViewService));

            containerRegistry.RegisterDialog<ConfirmView, ConfirmViewModel>();

            RegisterMenuFactoryAsync(containerRegistry).GetAwaiter().GetResult();

            RegisterBuildWorkspaceViewEventAsync();
        }

        protected async Task RegisterMenuFactoryAsync(IContainerRegistry containerRegistry)
        {
            try
            {
                MenuService menuService = new(new List<string> {"pack://application:,,,/Aksl.Wpf.DragDrop;Component/Data/AllMenus.xml",
                                                                "pack://application:,,,/Aksl.Wpf.DragDrop;Component/Data/Blacks.xml",
                                                                "pack://application:,,,/Aksl.Wpf.DragDrop;Component/Data/Blues.xml",
                                                                "pack://application:,,,/Aksl.Wpf.DragDrop;Component/Data/Yellows.xml",
                                                                });

                await menuService.CreateMenusAsync();

                containerRegistry.RegisterInstance<IMenuService>(menuService);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
        }

        protected Task RegisterBuildWorkspaceViewEventAsync()
        {
            try
            {
                var eventAggregator = Container.Resolve<IEventAggregator>();

                //SideBar
                _ = eventAggregator.GetEvent<OnBuildHamburgerMenuSideBarWorkspaceViewEvent>();
                _ = eventAggregator.GetEvent<OnBuildHamburgerMenuNavigationSideBarWorkspaceViewEvent>();
                _ = eventAggregator.GetEvent<OnBuildHamburgerMenuTreeSideBarWorkspaceViewEvent>();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }

            return Task.CompletedTask;
        }
  

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            _ = moduleCatalog.AddModule(nameof(AccountModule), typeof(AccountModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);

            _ = moduleCatalog.AddModule(nameof(BlacksModule), typeof(BlacksModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
            _ = moduleCatalog.AddModule(nameof(BluesModule), typeof(BluesModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
            _ = moduleCatalog.AddModule(nameof(YellowsModule), typeof(YellowsModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);

            _ = moduleCatalog.AddModule(typeof(HamburgerMenuNavigationSideBarModule).Name, typeof(HamburgerMenuNavigationSideBarModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
            _ = moduleCatalog.AddModule(typeof(ShellModule).Name, typeof(ShellModule).AssemblyQualifiedName, InitializationMode.WhenAvailable, typeof(HamburgerMenuNavigationSideBarModule).Name);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<ShellView>();
        }

        protected override  void InitializeShell(Window shell)
        {
            base.InitializeShell(shell);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }
    }
}
