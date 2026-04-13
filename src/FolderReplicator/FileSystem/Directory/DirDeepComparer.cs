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
        List<DirDeepCompareResultRow> output = [];

        var (sharedPaths, onlyInReferencePaths, onlyInTargetPaths) = GroupPaths(referenceDir, targetDir);

        output.AddRange(onlyInReferencePaths.Select(path =>
            new DirDeepCompareResultRow(
                path,
                DirDeepCompareResultRowStatus.ONLY_IN_REFERENCE
            )
        ));

        output.AddRange(onlyInTargetPaths.Select(path =>
            new DirDeepCompareResultRow(
                path,
                DirDeepCompareResultRowStatus.ONLY_IN_TARGET
            )
        ));

        output.AddRange(sharedPaths.Select(path => {
            UPath node1 = referenceDir / path;
            UPath node2 = targetDir / path;

            bool areNodesEqual = _fsNodeComparer.AreNodesEqual(node1, node2);
            var status = areNodesEqual
                ? DirDeepCompareResultRowStatus.IDENTICAL
                : DirDeepCompareResultRowStatus.DIFFERENT;

            return new DirDeepCompareResultRow(path, status);
        }));

        return output;
    }

    private (
        IEnumerable<UPath> sharedPaths,
        IEnumerable<UPath> onlyInReferencePaths,
        IEnumerable<UPath> onlyInTargetPaths
    ) GroupPaths(
        UPath referenceDir,
        UPath targetDir
    ) {
        var (sharedPath, onlyInReferencePaths) = FindSharedAndOnlyInReferencePaths(referenceDir, targetDir);
        var onlyInTargetPaths = FindOnlyInTargetPaths(referenceDir, targetDir);

        return (sharedPath, onlyInReferencePaths, onlyInTargetPaths);
    }

    private (
        IEnumerable<UPath> sharedPaths,
        IEnumerable<UPath> onlyInReferencePaths
    ) FindSharedAndOnlyInReferencePaths(
        UPath referenceDir,
        UPath targetDir
    ) {
        HashSet<UPath> sharedPaths = [];
        HashSet<UPath> onlyInReferencePaths = [];

        foreach (UPath relSubdir in _fsService.EnumerateFsNodeRelPaths(referenceDir)) {
            if (_fsService.DirHasPath(targetDir, relSubdir)) {
                sharedPaths.Add(relSubdir);
            } else {
                onlyInReferencePaths.Add(relSubdir);
            }
        }

        return (sharedPaths, onlyInReferencePaths);
    }

    private IEnumerable<UPath> FindOnlyInTargetPaths(
        UPath referenceDir,
        UPath targetDir
    ) {
        return _fsService.EnumerateFsNodeRelPaths(targetDir)
            .Where(subdir => !_fsService.DirHasPath(referenceDir, subdir));
    }
}