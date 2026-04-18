# How to run
1. `cd` to project's main directory
2. `dotnet build`
4. Find the created `FolderReplicator.dll`, it should be in `src/FolderReplicator/bin/Debug/net8.0/FolderReplicator.dll`
5. Run it: `dotnet FolderReplicator.dll replicate [src-folder] [dest-folder] [period-ms]`

# How to run in a safe environment
1. `cd` to project's main directory
2. `docker build -t folder-replicator -f Dockerfile .`
3. `docker run -it folder-replicator`
4. `docker run --rm --name folder-replicator folder-replicator:latest`
5. In a different terminal: `docker exec -it folder-replicator bash`

You can now interact with the program in a safe environment. The reference folder is `/home/app/src` and the target folder is `/home/app/dest`. Everything from the reference folder will be replicated to the target folder. Everything that is in the target folder and not in the reference folder will be removed.

# Implementation
1. Create tree representations of reference and target folders. Result: 2 trees with type `TreeNode`.
2. Search the trees for differences.
   - This requires going over the 2 trees separately.
   - The chosen tree search method is BFS, but in this case it does not matter, since we search the trees fully.
   - The search is pruned for optimization. For example, if a directory is present only in the reference folder, the search will not go over the files inside. Such search wouldn't change anything as the whole directory needs to be copied anyway.
3. Remove or copy files accordingly. 