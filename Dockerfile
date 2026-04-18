FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app
COPY . .
RUN dotnet build --output ./out

USER app
RUN mkdir ~/src ~/dest
RUN touch ~/src/test-file.txt
RUN mkdir ~/src/test-dir

ENTRYPOINT ["dotnet", "out/FolderReplicator.dll", "replicate", "/home/app/src", "/home/app/dest", "1000"]