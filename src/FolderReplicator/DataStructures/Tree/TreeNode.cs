namespace FolderReplicator.DataStructures.Tree;

public class TreeNode<T>(T value) {
    public T Value { get; set; } = value;

    public TreeNode<T>? Parent { get; set; }
    public List<TreeNode<T>> Children { get; } = [];

    public List<T> GetPath() {
        var path = new List<T>();

        var node = this;
        while (node.Parent != null) {
            path.Add(node.Value);
            node = node.Parent;
        }

        path.Reverse();
        return path;
    }

    public void AddChild(TreeNode<T> child) {
        child.Parent = this;
        Children.Add(child);
    }
}