FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Services/Identity/Outclass.Identity.API/Outclass.Identity.API.csproj", "src/Services/Identity/Outclass.Identity.API/"]
COPY ["src/Services/Identity/Outclass.Identity.Infrastructure/Outclass.Identity.Infrastructure.csproj", "src/Services/Identity/Outclass.Identity.Infrastructure/"]
COPY ["src/Services/Identity/Outclass.Identity.Application/Outclass.Identity.Application.csproj", "src/Services/Identity/Outclass.Identity.Application/"]
COPY ["src/Services/Identity/Outclass.Identity.Domain/Outclass.Identity.Domain.csproj", "src/Services/Identity/Outclass.Identity.Domain/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Infrastructure/Outclass.BuildingBlocks.Infrastructure.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Infrastructure/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Application/Outclass.BuildingBlocks.Application.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Application/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Domain/Outclass.BuildingBlocks.Domain.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Contracts/Outclass.BuildingBlocks.Contracts.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Contracts/"]
RUN dotnet restore "src/Services/Identity/Outclass.Identity.API/Outclass.Identity.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Identity/Outclass.Identity.API"
RUN dotnet build "Outclass.Identity.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Outclass.Identity.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Outclass.Identity.API.dll"]
