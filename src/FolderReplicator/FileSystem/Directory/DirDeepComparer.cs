using FolderReplicator.DataStructures.Tree;
using FolderReplicator.FileSystem.Node;

using Zio;

namespace FolderReplicator.FileSystem.Directory;

public class DirDeepComparer(
    IFileSystem fs,
    FileSystemService fsService,
    FileSystemNodeComparer fsNodeComparer
) {

    private readonly IFileSystem _fs = fs;
    private readonly FileSystemService _fsService = fsService;
    private readonly FileSystemNodeComparer _fsNodeComparer = fsNodeComparer;

    public IEnumerable<DirDeepCompareResultRow> DeepCompareDirs(
        UPath referenceDir,
        UPath targetDir
    ) {
        var referenceBase = referenceDir.GetDirectory();
        var targetBase = targetDir.GetDirectory();

        var referenceTree = _fsService.CreateTreeFromPath(referenceDir);
        var targetTree = _fsService.CreateTreeFromPath(targetDir);

        return [
            ..CalcDifferentAndOnlyInReference(referenceBase, targetBase, referenceTree),
            ..CalcOnlyInTarget(referenceBase, targetBase, targetTree)
        ];
    }

    private IEnumerable<DirDeepCompareResultRow> CalcDifferentAndOnlyInReference(
        UPath referenceBase,
        UPath targetBase,
        TreeNode<string> referenceTree
    ) {
        List<DirDeepCompareResultRow> result = [];

        Func<TreeNode<string>, bool> OnTreeNodeVisit = treeNode => {
            if (treeNode.Parent == null) {
                // Skip the root
                return false;
            }

            List<string> treeNodePath = treeNode.GetPath();
            UPath fsNodeRelPath = FileSystemUtils.CreateUPath(treeNodePath);

            UPath referenceFsNode = referenceBase / fsNodeRelPath;
            UPath targetFsNode = targetBase / fsNodeRelPath;

            bool existsInTarget = _fsService.NodeExists(targetFsNode);
            if (!existsInTarget) {
                var resultRow = new NodeOnlyInReference(fsNodeRelPath);
                result.Add(resultRow);
                return true;
            }

            if (!_fsNodeComparer.AreNodesEqual(referenceFsNode, targetFsNode)) {
                var resultRow = new DifferentNodes(fsNodeRelPath);
                result.Add(resultRow);
                return true;
            }

            return false;
        };

        TreeUtils.BfsPruned(referenceTree, OnTreeNodeVisit);

        return result;
    }

    private IEnumerable<DirDeepCompareResultRow> CalcOnlyInTarget(
        UPath referenceBase,
        UPath targetBase,
        TreeNode<string> targetTree
    ) {
        List<DirDeepCompareResultRow> result = [];

        Func<TreeNode<string>, bool> OnTreeNodeVisit = treeNode => {
            if (treeNode.Parent == null) {
                // Skip the root
                return false;
            }

            List<string> treeNodePath = treeNode.GetPath();
            UPath fsNodeRelPath = FileSystemUtils.CreateUPath(treeNodePath);

            UPath referenceFsNode = referenceBase / fsNodeRelPath;
            UPath targetFsNode = targetBase / fsNodeRelPath;

            bool existsInReference = _fsService.NodeExists(referenceFsNode);
            if (!existsInReference) {
                var resultRow = new NodeOnlyInTarget(fsNodeRelPath);
                result.Add(resultRow);
                return true;
            }

            return false;
        };

        TreeUtils.BfsPruned(targetTree, OnTreeNodeVisit);

        return result;
    }


}