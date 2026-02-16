FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/Tenant/Outclass.Tenant.API/Outclass.Tenant.API.csproj", "src/Services/Tenant/Outclass.Tenant.API/"]
COPY ["src/Services/Tenant/Outclass.Tenant.Infrastructure/Outclass.Tenant.Infrastructure.csproj", "src/Services/Tenant/Outclass.Tenant.Infrastructure/"]
COPY ["src/Services/Tenant/Outclass.Tenant.Application/Outclass.Tenant.Application.csproj", "src/Services/Tenant/Outclass.Tenant.Application/"]
COPY ["src/Services/Tenant/Outclass.Tenant.Domain/Outclass.Tenant.Domain.csproj", "src/Services/Tenant/Outclass.Tenant.Domain/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Infrastructure/Outclass.BuildingBlocks.Infrastructure.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Infrastructure/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Application/Outclass.BuildingBlocks.Application.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Application/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Domain/Outclass.BuildingBlocks.Domain.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Contracts/Outclass.BuildingBlocks.Contracts.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Contracts/"]
RUN dotnet restore "src/Services/Tenant/Outclass.Tenant.API/Outclass.Tenant.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Tenant/Outclass.Tenant.API"
RUN dotnet build "Outclass.Tenant.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Outclass.Tenant.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Outclass.Tenant.API.dll"]
