using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyDiary.UI.Services.Interfaces;

namespace MyDiary.UI.Services;

public class FileService : IFileService
{
    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public async Task<string> ReadAllTextAsync(string path)
    {
        return await File.ReadAllTextAsync(path);
    }

    public async Task WriteAllTextAsync(string path, string content)
    {
        await File.WriteAllTextAsync(path, content);
    }

    public void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }

    public string[] GetDirectories(string path)
    {
        return Directory.GetDirectories(path);
    }

    public string[] GetFiles(string path, string searchPattern)
    {
        return Directory.GetFiles(path, searchPattern);
    }

    public void CombinePath(out string combinedPath, params string[] paths)
    {
        combinedPath = Path.Combine(paths);
    }
}