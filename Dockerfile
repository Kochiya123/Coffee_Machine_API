
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
COPY bin/Release/net8.0/publish App/
WORKDIR /App
ENTRYPOINT ["dotnet", "WebApplication2.dll"]