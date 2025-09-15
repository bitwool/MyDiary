using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyDiary.UI.Models;
using MyDiary.UI.Services;
using MyDiary.UI.ViewModels;

namespace MyDiary.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{

    public SettingsService _settingsService { get; }

    [ObservableProperty] private ViewModelBase? _contentView;


    public DiaryViewModel DiaryViewModel { get; }

    public ToolBoxViewModel ToolBoxViewModel { get; }

    public MainWindowViewModel(
        DiaryViewModel diaryViewModel,
        ToolBoxViewModel toolBoxViewModel,
        SettingsService settingsService)
    {
        DiaryViewModel = diaryViewModel;
        ToolBoxViewModel = toolBoxViewModel;
        _settingsService = settingsService;
        ShowDiary();
    }


    [RelayCommand]
    private void ShowDiary()
    {
        ContentView = DiaryViewModel;
    }

    [RelayCommand]
    private void ShowToolBox()
    {
        ContentView = ToolBoxViewModel;
    }
}