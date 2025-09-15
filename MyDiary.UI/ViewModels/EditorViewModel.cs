using System;
using System.IO;
using System.Threading;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using MyDiary.UI.Models;

namespace MyDiary.UI.ViewModels;

public partial class EditorViewModel : ViewModelBase
{
    [ObservableProperty] private DiaryEntry _diaryEntry;

    [ObservableProperty] private TextDocument _document;

    private Timer? _autoSaveTimer;
    private const int AutoSaveDelayMs = 2000; // 2秒延迟自动保存

    public EditorViewModel(DiaryEntry diaryEntry)
    {
        _diaryEntry = diaryEntry;
        _document = new TextDocument(diaryEntry?.Content ?? string.Empty);

        _document.TextChanged += (sender, args) => {
            _diaryEntry.Content = _document.Text;
            TriggerAutoSave();
        };
    }

    partial void OnDiaryEntryChanged(DiaryEntry value)
    {
        Document = new TextDocument(value?.Content ?? string.Empty);
        Document.TextChanged += (sender, args) =>
        {
            if (value != null)
            {
                value.Content = Document.Text;
                TriggerAutoSave();
            }
        };
    }

    public void Save()
    {
        if (DiaryEntry.IsDirty && !string.IsNullOrEmpty(DiaryEntry.FilePath))
        {
            try
            {
                File.WriteAllText(DiaryEntry.FilePath, DiaryEntry.Content);
                DiaryEntry.IsDirty = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存文件失败: {ex.Message}");
            }
        }
    }

    private void TriggerAutoSave()
    {
        _autoSaveTimer?.Dispose();
        _autoSaveTimer = new Timer(_ => {
            if (DiaryEntry.IsDirty && !string.IsNullOrEmpty(DiaryEntry.FilePath))
            {
                Save();
            }
        }, null, AutoSaveDelayMs, Timeout.Infinite);
    }

    public void Dispose()
    {
        _autoSaveTimer?.Dispose();
    }
}