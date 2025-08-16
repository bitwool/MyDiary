using System;
using System.IO;
using System.Text.Json;
using MyDiary.UI.Models;

namespace MyDiary.UI.Services;

public class SettingsService
{
    private readonly string _configPath;

    public SettingsService()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appDir = Path.Combine(appDataPath, "MyDiary");
        if (!Directory.Exists(appDir))
        {
            Directory.CreateDirectory(appDir);
        }
        _configPath = Path.Combine(appDir, ".mydiary.json");
    }

    public Settings? Load()
    {
        if (!File.Exists(_configPath))
        {
            return null;
        }

        var json = File.ReadAllText(_configPath);
        return JsonSerializer.Deserialize<Settings>(json);
    }

    public void Save(Settings settings)
    {
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_configPath, json);
    }
}
