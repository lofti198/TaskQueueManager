# Use the official Microsoft ASP.NET 7 runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5020
EXPOSE 5021

# Use the SDK to build the project
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TaskQueueManager.csproj", "./"]
RUN dotnet restore "TaskQueueManager.csproj"
COPY . .
RUN dotnet build "TaskQueueManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskQueueManager.csproj" -c Release -o /app/publish

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskQueueManager.dll"]
