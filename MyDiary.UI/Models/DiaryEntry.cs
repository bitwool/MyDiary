using System.ComponentModel;

namespace MyDiary.UI.Models;

public class DiaryEntry : INotifyPropertyChanged
{
    public string Header { get; set; } = string.Empty;
    private string _content = string.Empty;

    public string Content
    {
        get => _content;
        set
        {
            if (_content == value) return;
            _content = value;
            IsDirty = true;
            OnPropertyChanged(nameof(Content));
        }
    }

    public string FilePath { get; set; } = string.Empty;
    public bool IsDirty { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}