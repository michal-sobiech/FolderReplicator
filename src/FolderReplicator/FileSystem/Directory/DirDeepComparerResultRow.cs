using Zio;

namespace FolderReplicator.FileSystem.Directory;

public abstract record DirDeepCompareResultRow(UPath FsNodePath);

public record IdenticalNodes(UPath FsNodePath) : DirDeepCompareResultRow(FsNodePath);
public record DifferentFiles(UPath FsNodePath) : DirDeepCompareResultRow(FsNodePath);
public record NodeOnlyInReference(UPath FsNodePath) : DirDeepCompareResultRow(FsNodePath);
public record NodeOnlyInTarget(UPath FsNodePath) : DirDeepCompareResultRow(FsNodePath);