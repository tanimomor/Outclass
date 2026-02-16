FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Gateway/Outclass.Gateway/Outclass.Gateway.csproj", "src/Gateway/Outclass.Gateway/"]
RUN dotnet restore "src/Gateway/Outclass.Gateway/Outclass.Gateway.csproj"
COPY . .
WORKDIR "/src/src/Gateway/Outclass.Gateway"
RUN dotnet build "Outclass.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Outclass.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Outclass.Gateway.dll"]
