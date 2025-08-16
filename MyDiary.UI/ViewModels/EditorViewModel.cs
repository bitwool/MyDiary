using System.IO;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using MyDiary.UI.Models;

namespace MyDiary.UI.ViewModels;

public partial class EditorViewModel : ViewModelBase
{
    [ObservableProperty] private DiaryEntry _diaryEntry;

    [ObservableProperty] private TextDocument _document;


    public EditorViewModel()
    {
        if (Avalonia.Controls.Design.IsDesignMode)
        {
            var designDiaryEntry = new DiaryEntry
            {
                Header = "Design-Time Header",
                Content = "# Design-Time Content\n\nThis is some markdown content for the designer.",
                FilePath = "design/time/dummy.md"
            };
            _diaryEntry = designDiaryEntry;
            _document = new TextDocument(designDiaryEntry.Content);
        }
        else
        {
            var runtimeDiaryEntry = new DiaryEntry()
            {
                Content = "Please select a file to open.",
                Header = "Welcome"
            };
            _diaryEntry = runtimeDiaryEntry;
            _document = new TextDocument(runtimeDiaryEntry.Content);
        }
    }

    public EditorViewModel(DiaryEntry diaryEntry)
    {
        _diaryEntry = diaryEntry;
        _document = new TextDocument(diaryEntry?.Content ?? string.Empty);

        _document.TextChanged += (sender, args) => { _diaryEntry.Content = _document.Text; };
    }

    partial void OnDiaryEntryChanged(DiaryEntry value)
    {
        Document = new TextDocument(value?.Content ?? string.Empty);
        Document.TextChanged += (sender, args) =>
        {
            if (value != null)
                value.Content = Document.Text;
        };
    }

    public void Save()
    {
        if (DiaryEntry.IsDirty && !string.IsNullOrEmpty(DiaryEntry.FilePath))
        {
            File.WriteAllText(DiaryEntry.FilePath, DiaryEntry.Content);
            DiaryEntry.IsDirty = false;
        }
    }
}