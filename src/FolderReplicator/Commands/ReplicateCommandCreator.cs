using System.CommandLine;

using FolderReplicator.FileSystem.Directory;

using Serilog;

using Zio;

namespace FolderReplicator.Commands;

public static class ReplicateCommandCreator {

    public static Command CreateReplicateCommand(
        FileSystemService fileSystemService,
        DirDeepComparer dirDeepComparer
    ) {
        var srcArg = new Argument<string>("src");
        var destArg = new Argument<string>("dest");
        var periodMsArg = new Argument<int>("period-ms");
        var logFileArg = new Argument<string>("log-file");

        var replicateCommand = new Command("replicate");
        replicateCommand.Arguments.Add(srcArg);
        replicateCommand.Arguments.Add(destArg);
        replicateCommand.Arguments.Add(periodMsArg);
        replicateCommand.Arguments.Add(logFileArg);

        replicateCommand.SetAction(result => {

            UPath src = new UPath(
                result.GetValue(srcArg)
                ?? throw new ArgumentNullException(nameof(src)))
                .ToAbsolute();

            UPath dest = new UPath(
                result.GetValue(destArg)
                ?? throw new ArgumentNullException(nameof(dest)))
                .ToAbsolute();

            int periodMs = result.GetValue(periodMsArg);
            if (periodMs < 0) throw new ArgumentNullException("period");
            TimeSpan period = TimeSpan.FromMilliseconds(periodMs);

            UPath logFile = new UPath(
                result.GetValue(logFileArg)
                ?? throw new ArgumentNullException(nameof(logFile)))
                .ToAbsolute();

            string logOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

            var logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: logOutputTemplate)
                .WriteTo.File(
                    logFile.ToString(),
                    rollingInterval: RollingInterval.Infinite,
                    outputTemplate: logOutputTemplate
                ).CreateLogger();

            var folderReplicationService = new FolderReplicationService(
                logger,
                fileSystemService,
                dirDeepComparer
            );

            var commandHandler = new ReplicateCommandHandler(folderReplicationService);
            commandHandler.Handle(src, dest, period);
        });

        return replicateCommand;
    }

}