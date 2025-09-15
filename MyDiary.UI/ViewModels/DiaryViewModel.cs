using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyDiary.UI.Models;
using MyDiary.UI.Services;

namespace MyDiary.UI.ViewModels;

public partial class DiaryViewModel : ViewModelBase
{

    private readonly SettingsService _settingsService;

    [ObservableProperty] private string? _workspacePath;

    [ObservableProperty] private bool _isWorkspaceSet;

    [ObservableProperty] private ObservableCollection<TreeNode> _treeNodes = new();

    [ObservableProperty] private ObservableCollection<EditorViewModel> _openDiaries = new();

    [ObservableProperty] private EditorViewModel? _selectedDiary;

    [ObservableProperty] private bool _isTodayDiaryExists;

    public Func<Task<string?>>? ShowFolderOpenDialog { get; set; }


    public DiaryViewModel(SettingsService settingsService)
    {
        _settingsService = settingsService;
        LoadWorkspaceSettings();
    }

    private void CheckIfTodayDiaryExists()
    {
        if (string.IsNullOrEmpty(WorkspacePath))
        {
            return;
        }
        var today = DateTime.Today;
        var filePath = Path.Combine(WorkspacePath, today.Year.ToString(), today.Month.ToString("D2"), $"{today:yyyy-MM-dd}.md");

        if (!File.Exists(filePath))
        {
            Debug.WriteLine("Create today diary file");
            CreateNewDiary();
        }
    }

    private void LoadWorkspaceSettings()
    {
        var settings = _settingsService.Load();

        if (settings != null && !string.IsNullOrEmpty(settings.WorkspacePath) &&
            Directory.Exists(settings.WorkspacePath))
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

        var directories = Directory.GetDirectories(WorkspacePath).OrderByDescending(d => d);

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
        if (!Directory.Exists(parentNode.Path)) return;

        var directories = Directory.GetDirectories(parentNode.Path);

        foreach (var dir in directories.OrderByDescending(d => d))
        {
            var dirInfo = new DirectoryInfo(dir);
            var childNode = new TreeNode { Name = dirInfo.Name, Path = dirInfo.FullName };
            LoadSubNodes(childNode);
            parentNode.Children.Add(childNode);
        }

        var files = Directory.GetFiles(parentNode.Path, "*.md");

        foreach (var file in files.OrderByDescending(f => f))
        {
            var fileInfo = new FileInfo(file);
            parentNode.Children.Add(new TreeNode
            { Name = Path.GetFileNameWithoutExtension(fileInfo.Name), Path = fileInfo.FullName });
        }
    }

    public void OpenFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
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
            var content = File.ReadAllText(filePath);
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


    private void CreateNewDiary()
    {
        if (!IsWorkspaceSet || string.IsNullOrEmpty(WorkspacePath)) return;

        var today = DateTime.Today;
        var yearName = today.Year.ToString();
        var monthName = today.Month.ToString("D2");
        var fileName = $"{today:yyyy-MM-dd}";

        var yearDir = Path.Combine(WorkspacePath, yearName);
        var monthDir = Path.Combine(yearDir, monthName);

        if (!Directory.Exists(monthDir))
        {
            Directory.CreateDirectory(monthDir);
        }

        var filePath = Path.Combine(monthDir, $"{fileName}.md");

        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, $"# {fileName}");
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

        OpenFile(filePath);
        CheckIfTodayDiaryExists();
    }

    [RelayCommand]
    private async Task SelectWorkspace()
    {
        if (ShowFolderOpenDialog != null)
        {
            var path = await ShowFolderOpenDialog();

            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                WorkspacePath = path;
                _settingsService.Save(new Settings { WorkspacePath = WorkspacePath });
                IsWorkspaceSet = true;
                LoadDiaries();
            }
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