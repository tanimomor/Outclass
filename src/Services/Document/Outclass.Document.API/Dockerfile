FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Services/Document/Outclass.Document.API/Outclass.Document.API.csproj", "src/Services/Document/Outclass.Document.API/"]
COPY ["src/Services/Document/Outclass.Document.Infrastructure/Outclass.Document.Infrastructure.csproj", "src/Services/Document/Outclass.Document.Infrastructure/"]
COPY ["src/Services/Document/Outclass.Document.Application/Outclass.Document.Application.csproj", "src/Services/Document/Outclass.Document.Application/"]
COPY ["src/Services/Document/Outclass.Document.Domain/Outclass.Document.Domain.csproj", "src/Services/Document/Outclass.Document.Domain/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Infrastructure/Outclass.BuildingBlocks.Infrastructure.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Infrastructure/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Application/Outclass.BuildingBlocks.Application.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Application/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Domain/Outclass.BuildingBlocks.Domain.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Contracts/Outclass.BuildingBlocks.Contracts.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Contracts/"]
RUN dotnet restore "src/Services/Document/Outclass.Document.API/Outclass.Document.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Document/Outclass.Document.API"
RUN dotnet build "Outclass.Document.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Outclass.Document.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Outclass.Document.API.dll"]
