using Zio;

namespace FolderReplicator.DirDeepComparer;

public enum DirDeepCompareResultRowStatus {
    IDENTICAL,
    ONLY_IN_REFERENCE,
    DIFFERENT,
    ONLY_IN_TARGET,
}

public record DirDeepCompareResultRow(
    UPath Directory,
    DirDeepCompareResultRowStatus Status
);