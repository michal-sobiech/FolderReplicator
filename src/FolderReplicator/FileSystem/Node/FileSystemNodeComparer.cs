using System.Security.Cryptography;

using Zio;

namespace FolderReplicator.FileSystem.Node;

public class FileSystemNodeComparer(IFileSystem fs) {

    private readonly IFileSystem _fs = fs;

    public bool AreNodesEqual(UPath node1, UPath node2) {
        var node1Kind = GetNodeKind(node1);
        var node2Kind = GetNodeKind(node2);

        if (node1Kind != node2Kind) {
            return false;
        }

        if (node1Kind == FsNodeKind.DIRECTORY) {
            // We check just the node, not the children. Children are checked separately.
            // In case of directory nodes the only thing that matters is the node kind.
            return true;
        }

        // In case of file nodes we also check the file content
        return AreFilesTheSameSize(node1, node2) && AreFileContentsEqual(node1, node2);
    }

    private FsNodeKind GetNodeKind(UPath node) {
        return _fs.DirectoryExists(node) ? FsNodeKind.DIRECTORY : FsNodeKind.FILE;
    }

    private bool AreFilesTheSameSize(UPath file1, UPath file2) {
        FileEntry entry1 = _fs.GetFileEntry(file1);
        FileEntry entry2 = _fs.GetFileEntry(file2);
        return entry1.Length == entry2.Length;
    }

    private bool AreFileContentsEqual(UPath file1, UPath file2) {
        using Stream stream1 = _fs.OpenFile(file1, FileMode.Open, FileAccess.Read);
        using Stream stream2 = _fs.OpenFile(file2, FileMode.Open, FileAccess.Read);

        var hash1 = MD5.HashData(stream1);
        var hash2 = MD5.HashData(stream2);

        return hash1.SequenceEqual(hash2);
    }

}