FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app
COPY . .
RUN dotnet build --output ./out

USER app
RUN mkdir ~/src && touch ~/src/test-file.txt && mkdir ~/src/test-dir && mkdir ~/dest

ENTRYPOINT ["dotnet", "out/FolderReplicator.dll", "replicate", "/home/app/src", "/home/app/dest", "1000", "/home/app/log.txt"]