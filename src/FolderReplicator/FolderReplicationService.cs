using FolderReplicator.FileSystem.Directory;

using Zio;

namespace FolderReplicator;

public class FolderReplicationService(
    IFileSystem fs,
    FileSystemService fileSystemService,
    DirDeepComparer dirDeepComparer
) {

    private readonly IFileSystem _fs = fs;
    private readonly FileSystemService _fileSystemService = fileSystemService;
    private readonly DirDeepComparer _dirDeepComparer = dirDeepComparer;

    public void ReplicateFolder(UPath referenceDir, UPath targetDir) {
        var comparisonResult = _dirDeepComparer.DeepCompareDirs(referenceDir, targetDir);

        Console.WriteLine($"LOG_30 {comparisonResult.Count()}");
        foreach (var row in comparisonResult) {
            Console.WriteLine($"LOG_31 {row.GetType()} {row.FsNodePath}");
        }

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
        CopyNodes(referenceDir, different, targetDir);

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
        Console.WriteLine("LOG_40");
        foreach (var node in nodes) {
            Console.WriteLine($"LOG_41, {node}");
            UPath srcAbs = src / node;
            UPath destAbs = dest / node;

            _fileSystemService.CopyNode(srcAbs, destAbs);
        }
    }

}