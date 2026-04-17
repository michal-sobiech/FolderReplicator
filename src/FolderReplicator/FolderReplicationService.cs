using FolderReplicator.FileSystem.Directory;

using Zio;

namespace FolderReplicator;

public class FolderReplicationService(
    DirDeepComparer dirDeepComparer,
    FileSystemService fileSystemService,
    IFileSystem fs
) {

    private readonly DirDeepComparer _dirDeepComparer = dirDeepComparer;
    private readonly FileSystemService _fileSystemService = fileSystemService;
    private readonly IFileSystem _fs = fs;

    public void ReplicateFolder(UPath referenceDir, UPath targetDir) {
        var comparisonResult = _dirDeepComparer.DeepCompareDirs(referenceDir, targetDir);

        var different = comparisonResult.OfType<DifferentNodes>()
            .Select(x => x.FsNodePath)
            .ToList();
        var onlyInRef = comparisonResult.OfType<NodeOnlyInReference>()
            .Select(x => x.FsNodePath)
            .ToList();
        var onlyInTarget = comparisonResult.OfType<NodeOnlyInTarget>()
            .Select(x => x.FsNodePath)
            .ToList();

        DeleteNodes(targetDir, different);
        DeleteNodes(targetDir, onlyInTarget);
        CopyNodes(referenceDir, onlyInRef, targetDir);
    }

    private void DeleteNodes(UPath dir, IEnumerable<UPath> nodes) {
        foreach (var node in nodes) {
            UPath path = dir / node;
            _fileSystemService.DeleteNode(path);
        }
    }

    private void CopyNodes(UPath src, IEnumerable<UPath> nodes, UPath dest) {
        foreach (var node in nodes) {
            UPath srcAbs = src / node;
            UPath destAbs = dest / node;

            _fileSystemService.CopyNode(srcAbs, destAbs);
        }
    }

}