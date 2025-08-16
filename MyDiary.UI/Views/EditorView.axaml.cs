using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace MyDiary.UI.Views;

public partial class EditorView : UserControl
{

    private TextMate.Installation? _textMateInstallation;

    public EditorView()
    {
        InitializeComponent();

        // 延迟执行主题设置
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // 小延迟确保控件完全渲染
        await Task.Delay(50);
        await Dispatcher.UIThread.InvokeAsync(SetTheme);
    }

    protected void SetTheme()
    {
        var textEditor = this.FindControl<TextEditor>("TextEditor");
        if (textEditor == null)
            return;

        var registryOptions = new RegistryOptions(ThemeName.LightPlus);
        _textMateInstallation = textEditor.InstallTextMate(registryOptions);

        ApplyBrushAction(_textMateInstallation, "editor.background", brush => textEditor.Background = brush);
        ApplyBrushAction(_textMateInstallation, "editor.foreground", brush => textEditor.Foreground = brush);

        _textMateInstallation.SetGrammar(
            registryOptions.GetScopeByLanguageId(registryOptions.GetLanguageByExtension(".md").Id));
    }

    bool ApplyBrushAction(TextMate.Installation e, string colorKeyNameFromJson, Action<IBrush> applyColorAction)
    {
        if (!e.TryGetThemeColor(colorKeyNameFromJson, out var colorString))
            return false;

        if (!Color.TryParse(colorString, out Color color))
            return false;

        var colorBrush = new SolidColorBrush(color);
        applyColorAction(colorBrush);
        return true;
    }
}