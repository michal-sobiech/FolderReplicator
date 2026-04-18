using FluentAssertions;

using FolderReplicator.FileSystem.Directory;
using FolderReplicator.FileSystem.Node;

using Zio;
using Zio.FileSystems;

namespace FolderReplicator.Tests;

record TestContext(
    IFileSystem Fs,
    FileSystemService FsService,
    FolderReplicationService FolderReplicationService
);

public class FolderReplicationServiceTests {

    private static readonly UPath SRC = new("/src");
    private static readonly UPath DEST = new("/dest");

    [Fact]
    public void ReplicateFolder_ShouldCopyDirFromRefToTarget_WhenMissingInTarget() {
        var testContext = CreateTestContext();
        IFileSystem fs = testContext.Fs;
        var folderReplicationService = testContext.FolderReplicationService;

        fs.CreateDirectory(SRC / "a");

        folderReplicationService.ReplicateFolder(SRC, DEST);

        fs.DirectoryExists(DEST / "a").Should().BeTrue();
    }

    [Fact]
    public void ReplicateFolder_ShouldCopyNestedDirFromRefToTarget_WhenMissingInTarget() {
        var testContext = CreateTestContext();
        IFileSystem fs = testContext.Fs;
        var folderReplicationService = testContext.FolderReplicationService;

        fs.CreateDirectory(SRC / "a" / "b");

        folderReplicationService.ReplicateFolder(SRC, DEST);

        fs.DirectoryExists(DEST / "a" / "b").Should().BeTrue();
    }

    [Fact]
    public void ReplicateFolder_ShouldCopyFileFromRefToTarget_WhenMissingInTarget() {
        var testContext = CreateTestContext();
        IFileSystem fs = testContext.Fs;
        var fsService = testContext.FsService;
        var folderReplicationService = testContext.FolderReplicationService;

        fsService.CreateFile(SRC / "a");

        folderReplicationService.ReplicateFolder(SRC, DEST);

        fs.FileExists("/dest/a").Should().BeTrue();
    }

    [Fact]
    public void ReplicateFolder_ShouldCopyNestedFileFromRefToTarget_WhenMissingInTarget() {
        var testContext = CreateTestContext();
        IFileSystem fs = testContext.Fs;
        var fsService = testContext.FsService;
        var folderReplicationService = testContext.FolderReplicationService;

        fs.CreateDirectory(SRC / "a");
        fsService.CreateFile(SRC / "a" / "b");

        folderReplicationService.ReplicateFolder(SRC, DEST);

        fs.FileExists(SRC / "a" / "b").Should().BeTrue();
    }

    [Fact]
    public void ReplicateFolder_ShouldRemoveFileInTarget_WhenMissingInReference() {
        var testContext = CreateTestContext();
        IFileSystem fs = testContext.Fs;
        var fsService = testContext.FsService;
        var folderReplicationService = testContext.FolderReplicationService;

        fsService.CreateFile(DEST / "a");

        folderReplicationService.ReplicateFolder(SRC, DEST);

        fs.FileExists(DEST / "a").Should().BeFalse();
    }


    [Fact]
    public void ReplicateFolder_ShouldRemoveDirInTarget_WhenMissingInReference() {
        var testContext = CreateTestContext();
        IFileSystem fs = testContext.Fs;
        var folderReplicationService = testContext.FolderReplicationService;

        fs.CreateDirectory(DEST / "a");

        folderReplicationService.ReplicateFolder(SRC, DEST);

        fs.DirectoryExists(DEST / "a").Should().BeFalse();
    }

    [Fact]
    public void ReplicateFolder_ShouldRemoveNestedFileInTarget_WhenMissingInReference() {
        var testContext = CreateTestContext();
        IFileSystem fs = testContext.Fs;
        var fsService = testContext.FsService;
        var folderReplicationService = testContext.FolderReplicationService;

        fs.CreateDirectory(DEST / "a");
        fsService.CreateFile(DEST / "a" / "b");

        folderReplicationService.ReplicateFolder(SRC, DEST);

        fs.FileExists(DEST / "a").Should().BeFalse();
    }


    [Fact]
    public void ReplicateFolder_ShouldRemoveNestedDirInTarget_WhenMissingInReference() {
        var testContext = CreateTestContext();
        IFileSystem fs = testContext.Fs;
        var folderReplicationService = testContext.FolderReplicationService;

        fs.CreateDirectory(DEST / "a" / "b");

        folderReplicationService.ReplicateFolder(SRC, DEST);

        fs.DirectoryExists(DEST / "a").Should().BeFalse();
    }

    [Fact]
    public void ReplicateFolder_ShouldReplaceFileWithDir_WhenDifferent() {
        var testContext = CreateTestContext();
        IFileSystem fs = testContext.Fs;
        var folderReplicationService = testContext.FolderReplicationService;

        fs.CreateDirectory(SRC / "a");
        fs.CreateDirectory(SRC / "a");

        folderReplicationService.ReplicateFolder(SRC, DEST);

        fs.DirectoryExists(DEST / "a").Should().BeFalse();
    }

    private TestContext CreateTestContext() {
        var fs = CreateTestFileSystem();
        var fsService = new FileSystemService(fs);
        var folderReplicationService = CreateFolderReplicationService(fs, fsService);
        return new TestContext(fs, fsService, folderReplicationService);
    }

    private IFileSystem CreateTestFileSystem() {
        var fs = new MemoryFileSystem();

        fs.CreateDirectory(SRC);
        fs.CreateDirectory(DEST);

        return fs;
    }

    private FolderReplicationService CreateFolderReplicationService(
        IFileSystem fs,
        FileSystemService fsService
    ) {
        var fsNodeComparer = new FileSystemNodeComparer(fs);
        var dirDeepComparer = new DirDeepComparer(fs, fsService, fsNodeComparer);
        return new FolderReplicationService(fs, fsService, dirDeepComparer);
    }

}