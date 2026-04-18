using FluentAssertions;

using FolderReplicator.FileSystem.Directory;
using FolderReplicator.FileSystem.Node;

using Zio;
using Zio.FileSystems;

namespace FolderReplicator.Tests;

public class FolderReplicationServiceTests {

    [Fact]
    public void ReplicateFolder_ShouldCopyDirFromRefToTarget_WhenMissingInTarget() {
        IFileSystem fs = SetUpTestFs();
        var folderReplicationService = CreateFolderReplicationService(fs);

        fs.CreateDirectory("/src/a");

        folderReplicationService.ReplicateFolder("/src", "/dest");

        fs.DirectoryExists("/dest/a").Should().BeTrue();
    }

    [Fact]
    public void ReplicateFolder_ShouldCopyNestedDirFromRefToTarget_WhenMissingInTarget() {
        IFileSystem fs = SetUpTestFs();
        var folderReplicationService = CreateFolderReplicationService(fs);

        fs.CreateDirectory("/src/a");
        fs.CreateDirectory("/src/a/b");

        folderReplicationService.ReplicateFolder("/src", "/dest");

        fs.DirectoryExists("/dest/a/b").Should().BeTrue();
    }

    [Fact]
    public void ReplicateFolder_ShouldCopyFileFromRefToTarget_WhenMissingInTarget() {
        IFileSystem fs = SetUpTestFs();
        var folderReplicationService = CreateFolderReplicationService(fs);

        using (fs.CreateFile("/src/a")) { }

        folderReplicationService.ReplicateFolder("/src", "/dest");

        fs.FileExists("/dest/a").Should().BeTrue();
    }

    [Fact]
    public void ReplicateFolder_ShouldCopyNestedFileFromRefToTarget_WhenMissingInTarget() {
        IFileSystem fs = SetUpTestFs();
        var folderReplicationService = CreateFolderReplicationService(fs);

        fs.CreateDirectory("/src/a");
        using (fs.CreateFile("/src/a/b")) { }

        folderReplicationService.ReplicateFolder("/src", "/dest");

        fs.FileExists("/dest/a/b").Should().BeTrue();
    }

    private IFileSystem SetUpTestFs() {
        var fs = new MemoryFileSystem();

        UPath src = new("/src");
        UPath dest = new("/dest");

        fs.CreateDirectory(src);
        fs.CreateDirectory(dest);

        return fs;
    }

    private FolderReplicationService CreateFolderReplicationService(IFileSystem fs) {
        var fsService = new FileSystemService(fs);
        var fsNodeComparer = new FileSystemNodeComparer(fs);
        var dirDeepComparer = new DirDeepComparer(fs, fsService, fsNodeComparer);
        return new FolderReplicationService(fs, fsService, dirDeepComparer);
    }

}