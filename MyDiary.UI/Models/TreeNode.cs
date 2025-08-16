using System.Collections.ObjectModel;

namespace MyDiary.UI.Models;

public class TreeNode
{
    public required string Name { get; set; }
    public required string Path { get; set; }
    public ObservableCollection<TreeNode> Children { get; set; } = new();
}