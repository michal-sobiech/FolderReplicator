namespace FolderReplicator.DataStructures.Tree;

public static class TreeUtils {

    public static void BfsPruned<T>(
        TreeNode<T> root,
        Func<TreeNode<T>, bool>? onNodeVisit
    ) {
        var queue = new Queue<TreeNode<T>>();

        queue.Enqueue(root);

        while (queue.Count > 0) {
            TreeNode<T> node = queue.Dequeue();

            bool shouldPrune = onNodeVisit == null
                ? false
                : onNodeVisit.Invoke(node);

            if (!shouldPrune) {
                foreach (var child in node.Children) queue.Enqueue(child);
            }
        }
    }

    public static bool HasPath<T>(
        TreeNode<T> tree,
        List<T> path
    ) where T : IEquatable<T> {
        var node = tree;

        foreach (var segment in path) {
            TreeNode<T>? child = node.Children.FirstOrDefault(x => x.Value.Equals(segment));

            if (child == null) {
                return false;
            }

            node = child;
        }

        return true;
    }

}