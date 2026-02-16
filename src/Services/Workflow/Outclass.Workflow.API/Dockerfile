FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Services/Workflow/Outclass.Workflow.API/Outclass.Workflow.API.csproj", "src/Services/Workflow/Outclass.Workflow.API/"]
COPY ["src/Services/Workflow/Outclass.Workflow.Infrastructure/Outclass.Workflow.Infrastructure.csproj", "src/Services/Workflow/Outclass.Workflow.Infrastructure/"]
COPY ["src/Services/Workflow/Outclass.Workflow.Application/Outclass.Workflow.Application.csproj", "src/Services/Workflow/Outclass.Workflow.Application/"]
COPY ["src/Services/Workflow/Outclass.Workflow.Domain/Outclass.Workflow.Domain.csproj", "src/Services/Workflow/Outclass.Workflow.Domain/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Infrastructure/Outclass.BuildingBlocks.Infrastructure.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Infrastructure/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Application/Outclass.BuildingBlocks.Application.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Application/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Domain/Outclass.BuildingBlocks.Domain.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/Outclass.BuildingBlocks.Contracts/Outclass.BuildingBlocks.Contracts.csproj", "src/BuildingBlocks/Outclass.BuildingBlocks.Contracts/"]
RUN dotnet restore "src/Services/Workflow/Outclass.Workflow.API/Outclass.Workflow.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Workflow/Outclass.Workflow.API"
RUN dotnet build "Outclass.Workflow.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Outclass.Workflow.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Outclass.Workflow.API.dll"]
