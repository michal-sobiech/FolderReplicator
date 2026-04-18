using FluentAssertions;

using FolderReplicator.FileSystem.Directory;
using FolderReplicator.FileSystem.Node;

using Zio;
using Zio.FileSystems;

namespace FolderReplicator.Tests;

public class FolderReplicationServiceTests {

    private static readonly UPath SRC = new("/src");
    private static readonly UPath DEST = new("/dest");

    [Fact]
    public void ReplicateFolder_ShouldCopyDirFromRefToTarget_WhenMissingInTarget() {
        IFileSystem fs = SetUpTestFs();
        var folderReplicationService = CreateFolderReplicationService(fs);

        fs.CreateDirectory(SRC / "a");

        folderReplicationService.ReplicateFolder(SRC, DEST);

        fs.DirectoryExists(DEST / "a").Should().BeTrue();
    }

    [Fact]
    public void ReplicateFolder_ShouldCopyNestedDirFromRefToTarget_WhenMissingInTarget() {
        IFileSystem fs = SetUpTestFs();
        var folderReplicationService = CreateFolderReplicationService(fs);

        fs.CreateDirectory(SRC / "a" / "b");

        folderReplicationService.ReplicateFolder(SRC, DEST);

        fs.DirectoryExists(DEST / "a" / "b").Should().BeTrue();
    }

    [Fact]
    public void ReplicateFolder_ShouldCopyFileFromRefToTarget_WhenMissingInTarget() {
        IFileSystem fs = SetUpTestFs();
        var folderReplicationService = CreateFolderReplicationService(fs);

        using (fs.CreateFile(SRC / "a")) { }

        folderReplicationService.ReplicateFolder(SRC, DEST);

        fs.FileExists("/dest/a").Should().BeTrue();
    }

    [Fact]
    public void ReplicateFolder_ShouldCopyNestedFileFromRefToTarget_WhenMissingInTarget() {
        IFileSystem fs = SetUpTestFs();
        var folderReplicationService = CreateFolderReplicationService(fs);

        fs.CreateDirectory(SRC / "a");
        using (fs.CreateFile(SRC / "a" / "b")) { }

        folderReplicationService.ReplicateFolder(SRC, DEST);

        fs.FileExists(SRC / "a" / "b").Should().BeTrue();
    }

    [Fact]
    public void ReplicateFolder_ShouldRemoveFileInTarget_WhenMissingInReference() {
        IFileSystem fs = SetUpTestFs();
        var folderReplicationService = CreateFolderReplicationService(fs);

        using (fs.CreateFile(DEST / "a")) { }

        folderReplicationService.ReplicateFolder(SRC, DEST);

        fs.FileExists(DEST / "a").Should().BeFalse();
    }


    [Fact]
    public void ReplicateFolder_ShouldRemoveDirInTarget_WhenMissingInReference() {
        IFileSystem fs = SetUpTestFs();
        var folderReplicationService = CreateFolderReplicationService(fs);

        fs.CreateDirectory(DEST / "a");

        folderReplicationService.ReplicateFolder(SRC, DEST);

        fs.DirectoryExists(DEST / "a").Should().BeFalse();
    }

    [Fact]
    public void ReplicateFolder_ShouldRemoveNestedFileInTarget_WhenMissingInReference() {
        IFileSystem fs = SetUpTestFs();
        var folderReplicationService = CreateFolderReplicationService(fs);

        fs.CreateDirectory(DEST / "a");
        using (fs.CreateFile(DEST / "a" / "b")) { }

        folderReplicationService.ReplicateFolder(SRC, DEST);

        fs.FileExists(DEST / "a").Should().BeFalse();
    }


    [Fact]
    public void ReplicateFolder_ShouldRemoveNestedDirInTarget_WhenMissingInReference() {
        IFileSystem fs = SetUpTestFs();
        var folderReplicationService = CreateFolderReplicationService(fs);

        fs.CreateDirectory(DEST / "a" / "b");

        folderReplicationService.ReplicateFolder(SRC, DEST);

        fs.DirectoryExists(DEST / "a").Should().BeFalse();
    }

    private IFileSystem SetUpTestFs() {
        var fs = new MemoryFileSystem();

        fs.CreateDirectory(SRC);
        fs.CreateDirectory(DEST);

        return fs;
    }

    private FolderReplicationService CreateFolderReplicationService(IFileSystem fs) {
        var fsService = new FileSystemService(fs);
        var fsNodeComparer = new FileSystemNodeComparer(fs);
        var dirDeepComparer = new DirDeepComparer(fs, fsService, fsNodeComparer);
        return new FolderReplicationService(fs, fsService, dirDeepComparer);
    }

}