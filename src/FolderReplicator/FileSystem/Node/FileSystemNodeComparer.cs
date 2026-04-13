using System.Security.Cryptography;

using Zio;

namespace FolderReplicator.FileSystem.Node;

public class FileSystemNodeComparer(IFileSystem fs) {

    private readonly IFileSystem _fs = fs;

    public bool AreNodesEqual(UPath node1, UPath node2) {
        return AreFsNodesSameType(node1, node2)
            && AreNodesTheSameSize(node1, node2)
            && AreNodeContentsEqual(node1, node2);
    }

    private bool AreFsNodesSameType(UPath node1, UPath node2) {
        return (_fs.DirectoryExists(node1) && _fs.DirectoryExists(node2))
            || (_fs.FileExists(node1) && _fs.FileExists(node2));
    }

    private bool AreNodesTheSameSize(UPath node1, UPath node2) {
        FileEntry entry1 = _fs.GetFileEntry(node1);
        FileEntry entry2 = _fs.GetFileEntry(node2);
        return entry1.Length == entry2.Length;
    }

    private bool AreNodeContentsEqual(UPath node1, UPath node2) {
        Stream stream1 = _fs.OpenFile(node1, FileMode.Open, FileAccess.Read);
        Stream stream2 = _fs.OpenFile(node2, FileMode.Open, FileAccess.Read);

        var hash1 = MD5.HashData(stream1);
        var hash2 = MD5.HashData(stream2);

        return hash1.SequenceEqual(hash2);
    }

}