using System.Runtime.InteropServices;

using FolderReplicator.FileSystem.Directory;

using Serilog;

using Zio;

namespace FolderReplicator;

public class FolderReplicationService(
    ILogger logger,
    FileSystemService fileSystemService,
    DirDeepComparer dirDeepComparer
) {

    private readonly ILogger _logger = logger;
    private readonly FileSystemService _fileSystemService = fileSystemService;
    private readonly DirDeepComparer _dirDeepComparer = dirDeepComparer;

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
        CopyNodes(referenceDir, different, targetDir);

        DeleteNodes(targetDir, onlyInTarget);

        CopyNodes(referenceDir, onlyInRef, targetDir);
    }

    private void DeleteNodes(UPath dir, IEnumerable<UPath> nodes) {
        foreach (var node in nodes) {
            UPath path = dir / node;
            _fileSystemService.DeleteNode(path);
            _logger.Information($"Delete {path}");
        }
    }

    private void CopyNodes(UPath src, IEnumerable<UPath> nodes, UPath dest) {
        foreach (var node in nodes) {
            UPath srcAbs = src / node;
            UPath destAbs = dest / node;

            _fileSystemService.CopyNode(srcAbs, destAbs);
            _logger.Information($"Copy {srcAbs} to {destAbs}");
        }
    }

}