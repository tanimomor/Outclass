FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/Automation/Outclass.Automation.API/Outclass.Automation.API.csproj", "src/Services/Automation/Outclass.Automation.API/"]
COPY ["src/Services/Automation/Outclass.Automation.Infrastructure/Outclass.Automation.Infrastructure.csproj", "src/Services/Automation/Outclass.Automation.Infrastructure/"]
COPY ["src/Services/Automation/Outclass.Automation.Application/Outclass.Automation.Application.csproj", "src/Services/Automation/Outclass.Automation.Application/"]
COPY ["src/Services/Automation/Outclass.Automation.Domain/Outclass.Automation.Domain.csproj", "src/Services/Automation/Outclass.Automation.Domain/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Infrastructure/Outclass.BuildingBlocks.Infrastructure.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Infrastructure/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Application/Outclass.BuildingBlocks.Application.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Application/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Domain/Outclass.BuildingBlocks.Domain.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Contracts/Outclass.BuildingBlocks.Contracts.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Contracts/"]
RUN dotnet restore "src/Services/Automation/Outclass.Automation.API/Outclass.Automation.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Automation/Outclass.Automation.API"
RUN dotnet build "Outclass.Automation.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Outclass.Automation.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Outclass.Automation.API.dll"]
