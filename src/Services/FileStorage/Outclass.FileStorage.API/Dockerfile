FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Services/FileStorage/Outclass.FileStorage.API/Outclass.FileStorage.API.csproj", "src/Services/FileStorage/Outclass.FileStorage.API/"]
COPY ["src/Services/FileStorage/Outclass.FileStorage.Infrastructure/Outclass.FileStorage.Infrastructure.csproj", "src/Services/FileStorage/Outclass.FileStorage.Infrastructure/"]
COPY ["src/Services/FileStorage/Outclass.FileStorage.Application/Outclass.FileStorage.Application.csproj", "src/Services/FileStorage/Outclass.FileStorage.Application/"]
COPY ["src/Services/FileStorage/Outclass.FileStorage.Domain/Outclass.FileStorage.Domain.csproj", "src/Services/FileStorage/Outclass.FileStorage.Domain/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Infrastructure/Outclass.BuildingBlocks.Infrastructure.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Infrastructure/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Application/Outclass.BuildingBlocks.Application.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Application/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Domain/Outclass.BuildingBlocks.Domain.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Contracts/Outclass.BuildingBlocks.Contracts.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Contracts/"]
RUN dotnet restore "src/Services/FileStorage/Outclass.FileStorage.API/Outclass.FileStorage.API.csproj"
COPY . .
WORKDIR "/src/src/Services/FileStorage/Outclass.FileStorage.API"
RUN dotnet build "Outclass.FileStorage.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Outclass.FileStorage.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /tmp/outclass-files
ENTRYPOINT ["dotnet", "Outclass.FileStorage.API.dll"]
