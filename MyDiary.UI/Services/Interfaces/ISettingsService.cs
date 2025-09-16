using MyDiary.UI.Models;

namespace MyDiary.UI.Services.Interfaces;

public interface ISettingsService
{
    Settings? Load();
    void Save(Settings settings);
}