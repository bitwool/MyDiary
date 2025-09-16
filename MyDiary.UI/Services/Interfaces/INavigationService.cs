using System.Threading.Tasks;

namespace MyDiary.UI.Services.Interfaces;

public interface INavigationService
{
    Task<string?> ShowFolderOpenDialogAsync();
}