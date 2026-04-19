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
        var referenceTree = _fsService.CreateTreeFromPath(referenceDir);
        var targetTree = _fsService.CreateTreeFromPath(targetDir);

        return [
            ..CalcDifferentAndOnlyInReference(referenceDir, targetDir, referenceTree),
            ..CalcOnlyInTarget(referenceDir, targetDir, targetTree)
        ];
    }

    private IEnumerable<DirDeepCompareResultRow> CalcDifferentAndOnlyInReference(
        UPath referenceDir,
        UPath targetDir,
        TreeNode<string> referenceTree
    ) {
        List<DirDeepCompareResultRow> result = [];

        Func<TreeNode<string>, bool> OnTreeNodeVisit = treeNode => {
            if (treeNode.Parent == null) {
                // Skip the root
                return false;
            }

            List<string> treeNodePathNoRoot = treeNode.GetPath().Skip(1).ToList();
            UPath fsNodeRelPath = FileSystemUtils.CreateUPath(treeNodePathNoRoot);

            UPath referenceFsNode = referenceDir / fsNodeRelPath;
            UPath targetFsNode = targetDir / fsNodeRelPath;

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
        UPath referenceDir,
        UPath targetDir,
        TreeNode<string> targetTree
    ) {
        List<DirDeepCompareResultRow> result = [];

        Func<TreeNode<string>, bool> OnTreeNodeVisit = treeNode => {
            if (treeNode.Parent == null) {
                // Skip the root
                return false;
            }

            List<string> treeNodePathNoRoot = treeNode.GetPath().Skip(1).ToList();
            UPath fsNodeRelPath = FileSystemUtils.CreateUPath(treeNodePathNoRoot);

            UPath referenceFsNode = referenceDir / fsNodeRelPath;
            UPath targetFsNode = targetDir / fsNodeRelPath;

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