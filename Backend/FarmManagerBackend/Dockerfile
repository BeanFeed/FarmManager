FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
#RUN apt-get update
#RUN apt-get install nodejs -y
#RUN apt-get install npm -y
#RUN npm install -g http-server
WORKDIR /app

EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["Backend/FarmManagerBackend/FarmManagerBackend.csproj", "FarmManagerBackend/"]
RUN dotnet restore "FarmManagerBackend/FarmManagerBackend.csproj"
COPY ["./Backend", "."]
WORKDIR "/src/FarmManagerBackend"
RUN dotnet build "FarmManagerBackend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "FarmManagerBackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final

WORKDIR /app
#RUN mkdir site
RUN mkdir data
COPY --from=publish /app/publish .
#COPY --from=site-build /build/dist/. /app/site
ENTRYPOINT ["dotnet","FarmManagerBackend.dll"]
