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
using MyDiary.UI.Services.Interfaces;
using MyDiary.UI.ViewModels;

namespace MyDiary.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{

    public ISettingsService SettingsService { get; }

    [ObservableProperty] private ViewModelBase? _contentView;


    public DiaryViewModel DiaryViewModel { get; }

    public MainWindowViewModel(
        DiaryViewModel diaryViewModel,
        ISettingsService settingsService)
    {
        DiaryViewModel = diaryViewModel;
        SettingsService = settingsService;
        ShowDiary();
    }


    [RelayCommand]
    private void ShowDiary()
    {
        ContentView = DiaryViewModel;
    }

}