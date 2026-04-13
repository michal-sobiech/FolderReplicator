using Zio;

namespace FolderReplicator;

public class FileSystemService(IFileSystem fs) {

    private readonly IFileSystem _fs = fs;

    public bool DirHasPath(UPath baseDir, UPath relPath) {
        UPath fullPath = baseDir / relPath;
        return _fs.DirectoryExists(fullPath);
    }

    public IEnumerable<UPath> EnumerateSubdirRelPaths(UPath dir) {
        return _fs.EnumerateDirectories(dir, "*", SearchOption.AllDirectories)
            .Select(absPath => FileSystemUtils.GetRelativePath(dir, absPath));
    }

}