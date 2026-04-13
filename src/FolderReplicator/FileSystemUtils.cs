using Zio;

namespace FolderReplicator;

public static class FileSystemUtils {

    public static UPath GetRelativePath(UPath referencePath, UPath targetPath) {
        return Path.GetRelativePath(referencePath.ToString(), targetPath.ToString());
    }

}
