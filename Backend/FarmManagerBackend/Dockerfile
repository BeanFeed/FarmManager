FROM node:20.15 AS site-build
WORKDIR /build
COPY ./Frontend/package*.json ./
RUN npm install
COPY ./Frontend/. ./
RUN npm run build

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
RUN apt-get update
RUN apt-get install nodejs -y
RUN apt-get install npm -y
RUN npm install -g http-server
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Backend/FarmManagerBackend/FarmManagerBackend.csproj", "FarmManagerBackend/"]
RUN dotnet restore "FarmManagerBackend/FarmManagerBackend.csproj"
COPY ["./Backend", "."]
WORKDIR "/src/FarmManagerBackend"
RUN dotnet build "FarmManagerBackend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FarmManagerBackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
RUN mkdir site
COPY --from=publish /app/publish .
COPY --from=site-build /build/dist/. /app/site
ENTRYPOINT ["dotnet","FarmManagerBackend.dll"]
