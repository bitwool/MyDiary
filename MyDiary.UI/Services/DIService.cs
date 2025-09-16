using Microsoft.Extensions.DependencyInjection;
using MyDiary.UI.ViewModels;
using System;
using Avalonia.Controls;
using MyDiary.UI.Services.Interfaces;

namespace MyDiary.UI.Services;

public class DIService
{
    private static ServiceProvider? _serviceProvider;

    public static IServiceProvider ConfigureServices(Window? mainWindow = null)
    {
        var services = new ServiceCollection();

        // Register services
        services.AddSingleton<ISettingsService, SettingsesService>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<INavigationService>(sp => new NavigationService(mainWindow!));

        // Register view models
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<DiaryViewModel>();
        services.AddTransient<EditorViewModel>();

        _serviceProvider = services.BuildServiceProvider();
        return _serviceProvider;
    }

    public static IServiceProvider GetServiceProvider()
    {
        if (_serviceProvider == null)
        {
            throw new InvalidOperationException("Service provider not configured. Call ConfigureServices first.");
        }

        return _serviceProvider;
    }

    public static T GetService<T>() where T : class
    {
        return GetServiceProvider().GetRequiredService<T>();
    }
}