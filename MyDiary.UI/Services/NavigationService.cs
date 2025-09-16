using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using MyDiary.UI.Services.Interfaces;

namespace MyDiary.UI.Services;

public class NavigationService : INavigationService
{
    private readonly Window _parentWindow;

    public NavigationService(Window parentWindow)
    {
        _parentWindow = parentWindow;
    }

    public async Task<string?> ShowFolderOpenDialogAsync()
    {
        var topLevel = TopLevel.GetTopLevel(_parentWindow);
        if (topLevel == null) return null;

        var storageProvider = topLevel.StorageProvider;
        if (storageProvider == null) return null;

        var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var startLocation = await storageProvider.TryGetFolderFromPathAsync(homeDir);

        var result = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "选择工作区",
            AllowMultiple = false,
            SuggestedStartLocation = startLocation
        });

        return result.Count > 0 ? result[0].Path.LocalPath : null;
    }
}