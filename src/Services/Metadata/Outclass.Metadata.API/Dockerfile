FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Services/Metadata/Outclass.Metadata.API/Outclass.Metadata.API.csproj", "src/Services/Metadata/Outclass.Metadata.API/"]
COPY ["src/Services/Metadata/Outclass.Metadata.Infrastructure/Outclass.Metadata.Infrastructure.csproj", "src/Services/Metadata/Outclass.Metadata.Infrastructure/"]
COPY ["src/Services/Metadata/Outclass.Metadata.Application/Outclass.Metadata.Application.csproj", "src/Services/Metadata/Outclass.Metadata.Application/"]
COPY ["src/Services/Metadata/Outclass.Metadata.Domain/Outclass.Metadata.Domain.csproj", "src/Services/Metadata/Outclass.Metadata.Domain/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Infrastructure/Outclass.BuildingBlocks.Infrastructure.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Infrastructure/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Application/Outclass.BuildingBlocks.Application.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Application/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Domain/Outclass.BuildingBlocks.Domain.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Contracts/Outclass.BuildingBlocks.Contracts.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Contracts/"]
RUN dotnet restore "src/Services/Metadata/Outclass.Metadata.API/Outclass.Metadata.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Metadata/Outclass.Metadata.API"
RUN dotnet build "Outclass.Metadata.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Outclass.Metadata.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Outclass.Metadata.API.dll"]
