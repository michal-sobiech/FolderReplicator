using FluentAssertions;

using FolderReplicator.FileSystem.Directory;
using FolderReplicator.FileSystem.Node;

using Zio;
using Zio.FileSystems;

namespace FolderReplicator.Tests;

public class FolderReplicationServiceTests {

    [Fact]
    public void ReplicateFolder_ShouldCopyDirFromRefToTarget_WhenMissingInTarget() {
        IFileSystem fs = new MemoryFileSystem();

        UPath src = new("/src");
        UPath dest = new("/dest");

        fs.CreateDirectory(src);
        fs.CreateDirectory("/src/a");

        fs.CreateDirectory(dest);

        var folderReplicationService = CreateFolderReplicationService(fs);
        folderReplicationService.ReplicateFolder(src, dest);

        fs.DirectoryExists("/dest/a").Should().BeTrue();
    }

    [Fact]
    public void ReplicateFolder_ShouldCopyFileFromRefToTarget_WhenMissingInTarget() {
        IFileSystem fs = new MemoryFileSystem();

        UPath src = new("/src");
        UPath dest = new("/dest");

        fs.CreateDirectory(src);
        using (fs.CreateFile("/src/a")) { }

        fs.CreateDirectory(dest);

        var folderReplicationService = CreateFolderReplicationService(fs);
        folderReplicationService.ReplicateFolder(src, dest);

        fs.FileExists("/dest/a").Should().BeTrue();
    }


    private FolderReplicationService CreateFolderReplicationService(IFileSystem fs) {
        var fsService = new FileSystemService(fs);
        var fsNodeComparer = new FileSystemNodeComparer(fs);
        var dirDeepComparer = new DirDeepComparer(fs, fsService, fsNodeComparer);
        return new FolderReplicationService(fs, fsService, dirDeepComparer);
    }

}