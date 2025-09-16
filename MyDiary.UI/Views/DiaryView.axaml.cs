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


    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        // The navigation functionality is now handled by the NavigationService
        // No need to set up ShowFolderOpenDialog property as it has been removed
    }

    private async void TreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is DiaryViewModel vm &&
            e.AddedItems.Count > 0 &&
            e.AddedItems[0] is TreeNode node &&
            node.Children.Count == 0)
        {
            await vm.OpenFileAsync(node.Path);
        }
    }

    private async void TreeView_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is DiaryViewModel vm &&
            e.Source is TextBlock { DataContext: TreeNode node } &&
            node.Children.Count == 0)
        {
            await vm.OpenFileAsync(node.Path);
        }
    }
}