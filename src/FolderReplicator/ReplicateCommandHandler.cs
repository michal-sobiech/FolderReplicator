using Zio;

namespace FolderReplicator;

public class ReplicateCommandHandler(FolderReplicationService folderReplicationService) {

    private readonly FolderReplicationService _folderReplicationService = folderReplicationService;

    public void Handle(UPath src, UPath dest, TimeSpan period) {
        Console.WriteLine("111");
        while (true) {
            Console.WriteLine("TRY TO REPLICTE");
            _folderReplicationService.ReplicateFolder(src, dest);
            Thread.Sleep(period);
        }
    }

}