using System.CommandLine;

using FolderReplicator;
using FolderReplicator.Commands;
using FolderReplicator.FileSystem.Directory;
using FolderReplicator.FileSystem.Node;

using Microsoft.Extensions.DependencyInjection;

using Zio;
using Zio.FileSystems;

var services = new ServiceCollection();

services.AddSingleton<IFileSystem>(new PhysicalFileSystem());
services.AddSingleton<FileSystemService>();
services.AddSingleton<FileSystemNodeComparer>();
services.AddSingleton<DirDeepComparer>();

var sp = services.BuildServiceProvider();

var replicateCommand = ReplicateCommandCreator.CreateReplicateCommand(
    sp.GetRequiredService<FileSystemService>(),
    sp.GetRequiredService<DirDeepComparer>()
);
var rootCommand = new RootCommand("folder-replicator");
rootCommand.Subcommands.Add(replicateCommand);

return rootCommand.Parse(args).Invoke();
