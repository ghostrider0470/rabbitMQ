FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RabbitMQServer.csproj", "./"]
RUN dotnet restore "RabbitMQServer.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "RabbitMQServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RabbitMQServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RabbitMQServer.dll"]
