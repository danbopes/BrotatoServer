#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
RUN export LSB_RELEASE=$(dpkg --status tzdata|grep Provides|cut -f2 -d'-') && \
    printf "\ndeb http://deb.debian.org/debian $LSB_RELEASE main contrib non-free" > /etc/apt/sources.list && \
    printf "\ndeb http://deb.debian.org/debian-security/ $LSB_RELEASE-security main contrib non-free" >> /etc/apt/sources.list && \
    printf "\ndeb http://deb.debian.org/debian $LSB_RELEASE-updates main contrib non-free" >> /etc/apt/sources.list && \
    dpkg --add-architecture i386 && \
    echo steam steam/question select "I AGREE" | debconf-set-selections && \
    echo steam steam/license note '' | debconf-set-selections && \
    apt-get update && \
    apt-get install steamcmd --yes && \
    apt-get clean autoclean && \
    apt-get autoremove --yes &&  \
    rm -rf /var/lib/{apt,dpkg,cache,log}/ \
WORKDIR /app
COPY BrotatoServer/sdk_linux.txt /root/steam_build/
RUN /usr/games/steamcmd +runscript /root/steam_build/sdk_linux.txt && \
    mkdir -p /root/.steam && \
    ln -s /root/.local/share/Steam/sdk_lin/linux64/ /root/.steam/sdk64
#RUN mkdir -p /root/.steam/sdk64/
#COPY BrotatoServer/linux64/steamclient.so /root/.steam/sdk64/steamclient.so
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BrotatoServer/BrotatoServer.csproj", "BrotatoServer/"]
COPY ["BrotatoServer.Tests/BrotatoServer.Tests.csproj", "BrotatoServer.Tests/"]
COPY ["CollectibleCardEngine/CollectibleCardEngine.csproj", "CollectibleCardEngine/"]
RUN dotnet restore "BrotatoServer/BrotatoServer.csproj"
RUN dotnet restore "BrotatoServer.Tests/BrotatoServer.Tests.csproj"
COPY . .
WORKDIR "/src/BrotatoServer"
RUN dotnet build "BrotatoServer.csproj" -c Release -o /app/build

FROM build AS test
WORKDIR /src/BrotatoServer.Tests
RUN dotnet test --logger:trx

FROM build AS publish
RUN dotnet publish "BrotatoServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
RUN export LD_LIBRARY_PATH=/app/linux64${LD_LIBRARY_PATH:+:$LD_LIBRARY_PATH}
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrotatoServer.dll"]