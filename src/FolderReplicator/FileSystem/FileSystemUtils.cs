using Zio;

namespace FolderReplicator;

public static class FileSystemUtils {

    public static UPath GetRelativePath(UPath referencePath, UPath targetPath) {
        return Path.GetRelativePath(referencePath.ToString(), targetPath.ToString());
    }

    public static UPath CreateUPath(List<string> segments) {
        return segments.Aggregate(UPath.Empty, (acc, curr) => acc / curr);
    }

}
