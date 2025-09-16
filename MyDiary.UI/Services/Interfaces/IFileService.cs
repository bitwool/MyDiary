using System.Threading.Tasks;

namespace MyDiary.UI.Services.Interfaces;

public interface IFileService
{
    bool FileExists(string path);
    bool DirectoryExists(string path);
    Task<string> ReadAllTextAsync(string path);
    Task WriteAllTextAsync(string path, string content);
    void CreateDirectory(string path);
    string[] GetDirectories(string path);
    string[] GetFiles(string path, string searchPattern);
    void CombinePath(out string combinedPath, params string[] paths);
}