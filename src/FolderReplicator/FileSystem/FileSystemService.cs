using System.Data;

using FolderReplicator.DataStructures.Tree;

using Zio;

namespace FolderReplicator;

public class FileSystemService(IFileSystem fs) {

    private readonly IFileSystem _fs = fs;

    public bool DirHasPath(UPath baseDir, UPath relPath) {
        UPath fullPath = baseDir / relPath;
        return _fs.DirectoryExists(fullPath);
    }

    public IEnumerable<UPath> EnumerateFsNodeRelPaths(UPath dir) {
        return _fs.EnumerateDirectories(dir, "*", SearchOption.AllDirectories)
            .Select(absPath => FileSystemUtils.GetRelativePath(dir, absPath));
    }

    public void DeleteNode(UPath node) {
        if (_fs.FileExists(node)) {
            _fs.DeleteFile(node);
        } else if (_fs.DirectoryExists(node)) {
            _fs.DeleteDirectory(node, true);
        }
    }

    public void CopyNode(UPath src, UPath dest) {
        if (_fs.FileExists(src)) {
            _fs.CopyFile(src, dest, overwrite: true);
        } else if (_fs.DirectoryExists(src)) {
            CopyDirectory(src, dest);
        }
    }

    public void CopyDirectory(UPath src, UPath dest) {
        _fs.CreateDirectory(dest);

        foreach (UPath file in _fs.EnumerateFiles(src)) {
            UPath targetFile = dest / file.GetName();
            _fs.CopyFile(file, targetFile, overwrite: true);
        }

        foreach (UPath dir in _fs.EnumerateDirectories(src)) {
            UPath targetDir = dest / dir.GetName();
            CopyDirectory(dir, targetDir);
        }
    }

    public TreeNode<string> CreateTreeFromPath(UPath path) {
        if (_fs.DirectoryExists(path)) {
            var node = new TreeNode<string>(path.GetName());

            foreach (var dir in _fs.EnumerateDirectories(path)) {
                var child = CreateTreeFromPath(path / dir);
                node.AddChild(child);
            }

            foreach (var file in _fs.EnumerateFiles(path)) {
                var child = new TreeNode<string>(file.GetName());
                node.AddChild(child);
            }

            return node;
        } else if (_fs.FileExists(path)) {
            return new TreeNode<string>(path.GetName());
        } else {
            throw new ArgumentException($"Path does not exist: {path}");
        }
    }

    public bool NodeExists(UPath path) {
        return _fs.FileExists(path) || _fs.DirectoryExists(path);
    }

}