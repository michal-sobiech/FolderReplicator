using System.CommandLine;

using FolderReplicator;
using FolderReplicator.FileSystem.Directory;

using Microsoft.Extensions.DependencyInjection;

using Zio;
using Zio.FileSystems;

var services = new ServiceCollection();

services.AddSingleton<IFileSystem, FileSystem>();
services.AddSingleton<FileSystemService>();
services.AddSingleton<DirDeepComparer>();
services.AddSingleton<FolderReplicationService>();
services.AddSingleton<ReplicateCommandHandler>();

var sp = services.BuildServiceProvider();

var srcArg = new Argument<string>("src");
var destArg = new Argument<string>("dest");
var periodMsArg = new Argument<int>("period-ms");

var replicateCommand = new Command("replicate-dir");
replicateCommand.Arguments.Add(srcArg);
replicateCommand.Arguments.Add(destArg);
replicateCommand.Arguments.Add(periodMsArg);

replicateCommand.SetAction(result => {
    UPath src = new UPath(
        result.GetValue(srcArg)
        ?? throw new ArgumentNullException("src"))
        .ToAbsolute();

    UPath dest = new UPath(
        result.GetValue(destArg)
        ?? throw new ArgumentNullException("dest"))
        .ToAbsolute();

    int periodMs = result.GetValue(periodMsArg);
    if (periodMs < 0) throw new ArgumentNullException("period");
    TimeSpan period = TimeSpan.FromMilliseconds(periodMs);

    var replicateCommandHandler = sp.GetRequiredService<ReplicateCommandHandler>();
    replicateCommandHandler.Handle(src, dest, period);
});

var rootCommand = new RootCommand("folder-replicator");
rootCommand.Subcommands.Add(replicateCommand);

return rootCommand.Parse(args).Invoke();
