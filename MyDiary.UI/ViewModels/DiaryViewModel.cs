using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyDiary.UI.Models;
using MyDiary.UI.Services.Interfaces;

namespace MyDiary.UI.ViewModels;

public partial class DiaryViewModel : ViewModelBase
{

    private readonly ISettingsService _settingsService;

    [ObservableProperty] private string? _workspacePath;

    [ObservableProperty] private bool _isWorkspaceSet;

    [ObservableProperty] private ObservableCollection<TreeNode> _treeNodes = new();

    [ObservableProperty] private ObservableCollection<EditorViewModel> _openDiaries = new();

    [ObservableProperty] private EditorViewModel? _selectedDiary;

    [ObservableProperty] private bool _isTodayDiaryExists;

    private readonly IFileService _fileService;
    private readonly INavigationService _navigationService;

    public DiaryViewModel(
        ISettingsService settingsService,
        IFileService fileService,
        INavigationService navigationService)
    {
        _settingsService = settingsService;
        _fileService = fileService;
        _navigationService = navigationService;
        LoadWorkspaceSettings();
    }

    private void CheckIfTodayDiaryExists()
    {
        if (string.IsNullOrEmpty(WorkspacePath))
        {
            return;
        }
        var today = DateTime.Today;
        _fileService.CombinePath(out var filePath, WorkspacePath, today.Year.ToString(), today.Month.ToString("D2"), $"{today:yyyy-MM-dd}.md");

        if (!_fileService.FileExists(filePath))
        {
            Debug.WriteLine("Create today diary file");
            _ = CreateNewDiaryAsync();
        }
    }

    private void LoadWorkspaceSettings()
    {
        var settings = _settingsService.Load();

        if (settings != null && !string.IsNullOrEmpty(settings.WorkspacePath) &&
            _fileService.DirectoryExists(settings.WorkspacePath))
        {
            WorkspacePath = settings.WorkspacePath;
            IsWorkspaceSet = true;
            LoadDiaries();
        }
    }

    partial void OnSelectedDiaryChanged(EditorViewModel? oldValue, EditorViewModel? newValue)
    {
        oldValue?.Save();
    }


    private void LoadDiaries()
    {
        if (!IsWorkspaceSet || string.IsNullOrEmpty(WorkspacePath)) return;

        TreeNodes.Clear();

        var directories = _fileService.GetDirectories(WorkspacePath).OrderByDescending(d => d);

        foreach (var dir in directories)
        {
            var dirInfo = new DirectoryInfo(dir);
            var yearNode = new TreeNode { Name = dirInfo.Name, Path = dirInfo.FullName };
            LoadSubNodes(yearNode);
            TreeNodes.Add(yearNode);
        }

        CheckIfTodayDiaryExists();
    }

    private void LoadSubNodes(TreeNode parentNode)
    {
        if (!_fileService.DirectoryExists(parentNode.Path)) return;

        var directories = _fileService.GetDirectories(parentNode.Path);

        foreach (var dir in directories.OrderByDescending(d => d))
        {
            var dirInfo = new DirectoryInfo(dir);
            var childNode = new TreeNode { Name = dirInfo.Name, Path = dirInfo.FullName };
            LoadSubNodes(childNode);
            parentNode.Children.Add(childNode);
        }

        var files = _fileService.GetFiles(parentNode.Path, "*.md");

        foreach (var file in files.OrderByDescending(f => f))
        {
            var fileInfo = new FileInfo(file);
            parentNode.Children.Add(new TreeNode
            { Name = Path.GetFileNameWithoutExtension(fileInfo.Name), Path = fileInfo.FullName });
        }
    }

    public async Task OpenFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !_fileService.FileExists(filePath))
        {
            return;
        }

        var existing = OpenDiaries.FirstOrDefault(d => d.DiaryEntry.FilePath == filePath);

        if (existing != null)
        {
            SelectedDiary = existing;
            return;
        }

        SelectedDiary?.Save();

        try
        {
            var content = await _fileService.ReadAllTextAsync(filePath);
            var diary = new DiaryEntry
            {
                Header = Path.GetFileNameWithoutExtension(filePath),
                Content = content,
                FilePath = filePath
            };

            if (OpenDiaries.Any(x => x.DiaryEntry.Header == "Welcome"))
            {
                OpenDiaries.RemoveAt(0);
            }

            var editorViewModel = new EditorViewModel(diary);
            OpenDiaries.Add(editorViewModel);
            SelectedDiary = editorViewModel;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"读取文件时发生异常: {ex}");
        }
    }


    private async Task CreateNewDiaryAsync()
    {
        if (!IsWorkspaceSet || string.IsNullOrEmpty(WorkspacePath)) return;

        var today = DateTime.Today;
        var yearName = today.Year.ToString();
        var monthName = today.Month.ToString("D2");
        var fileName = $"{today:yyyy-MM-dd}";

        _fileService.CombinePath(out var yearDir, WorkspacePath, yearName);
        _fileService.CombinePath(out var monthDir, yearDir, monthName);
        _fileService.CombinePath(out var filePath, monthDir, $"{fileName}.md");

        if (!_fileService.DirectoryExists(monthDir))
        {
            _fileService.CreateDirectory(monthDir);
        }

        if (!_fileService.FileExists(filePath))
        {
            await _fileService.WriteAllTextAsync(filePath, $"# {fileName}");
        }

        var yearNode = TreeNodes.FirstOrDefault(n => n.Name == yearName);
        if (yearNode == null)
        {
            yearNode = new TreeNode { Name = yearName, Path = yearDir };
            TreeNodes.Insert(0, yearNode);
        }

        var monthNode = yearNode.Children.FirstOrDefault(n => n.Name == monthName);
        if (monthNode == null)
        {
            monthNode = new TreeNode { Name = monthName, Path = monthDir };
            yearNode.Children.Insert(0, monthNode);
        }

        var fileNode = monthNode.Children.FirstOrDefault(n => n.Name == fileName);
        if (fileNode == null)
        {
            fileNode = new TreeNode { Name = fileName, Path = filePath };
            monthNode.Children.Insert(0, fileNode);
        }

        await OpenFileAsync(filePath);
        CheckIfTodayDiaryExists();
    }

    [RelayCommand]
    private async Task SelectWorkspace()
    {
        var path = await _navigationService.ShowFolderOpenDialogAsync();

        if (!string.IsNullOrEmpty(path) && _fileService.DirectoryExists(path))
        {
            WorkspacePath = path;
            _settingsService.Save(new Settings { WorkspacePath = WorkspacePath });
            IsWorkspaceSet = true;
            LoadDiaries();
        }
    }

    [RelayCommand]
    private void CloseDiary(EditorViewModel? diaryToClose)
    {
        if (diaryToClose == null) return;

        diaryToClose.Save();
        OpenDiaries.Remove(diaryToClose);
    }
}