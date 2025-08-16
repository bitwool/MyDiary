using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MyDiary.UI.Models;
using MyDiary.UI.ViewModels;

namespace MyDiary.UI.Views;

public partial class DiaryView : UserControl
{
    public DiaryView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }


    private  void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DiaryViewModel vm) return;

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null) return;

        var storageProvider = topLevel.StorageProvider;
        vm.ShowFolderOpenDialog = async () =>
        {
            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var startLocation = await storageProvider.TryGetFolderFromPathAsync(homeDir);
            var result = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "选择工作区",
                AllowMultiple = false,
                SuggestedStartLocation = startLocation
            });
            return result.Count > 0 ? result[0].Path.LocalPath : null;
        };
    }

    private void TreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is DiaryViewModel vm &&
            e.AddedItems.Count > 0 &&
            e.AddedItems[0] is TreeNode node &&
            node.Children.Count == 0)
        {
            vm.OpenFile(node.Path);
        }
    }

    private void TreeView_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is DiaryViewModel vm &&
            e.Source is TextBlock { DataContext: TreeNode node } &&
            node.Children.Count == 0)
        {
            vm.OpenFile(node.Path);
        }
    }
}