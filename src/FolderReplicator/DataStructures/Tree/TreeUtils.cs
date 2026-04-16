using System.Collections;

using FolderReplicator.DataStructures.Tree;

namespace FolderReplicator.DataStructures;

public static class TreeUtils {

    public static void BfsPruned<T>(
        TreeNode<T> root,
        Action<TreeNode<T>> onNodeVisit,
        Func<TreeNode<T>, bool> shouldPruneNodeChildren
    ) {
        var queue = new Queue<TreeNode<T>>();

        queue.Enqueue(root);

        while (queue.Count > 0) {
            TreeNode<T> node = queue.Dequeue();

            onNodeVisit(node);

            if (!shouldPruneNodeChildren(node)) {
                foreach (var child in node.Children) queue.Enqueue(child);
            }
        }
    }

}