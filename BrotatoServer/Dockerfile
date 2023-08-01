#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BrotatoServer/BrotatoServer.csproj", "BrotatoServer/"]
COPY ["CollectibleCardEngine/CollectibleCardEngine.csproj", "CollectibleCardEngine/"]
RUN dotnet restore "BrotatoServer/BrotatoServer.csproj"
COPY . .
WORKDIR "/src/BrotatoServer"
RUN dotnet build "BrotatoServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BrotatoServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrotatoServer.dll"]