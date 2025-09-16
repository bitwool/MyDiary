using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MyDiary.UI.Services;
using MyDiary.UI.ViewModels;
using MyDiary.UI.Views;
using System;

namespace MyDiary.UI;

public partial class App : Application
{
    public IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        // First initialize without Window
        Services = DIService.ConfigureServices();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);

            // Create MainWindow first
            var mainWindow = new MainWindow();

            // Reconfigure services with the Window
            Services = DIService.ConfigureServices(mainWindow);

            // Set DataContext with reconfigured services
            mainWindow.DataContext = Services!.GetRequiredService<MainWindowViewModel>();

            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}