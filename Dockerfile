FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MovieSearchBot.csproj", "./"]
RUN dotnet restore "MovieSearchBot.csproj"
RUN dotnet tool install --global Microsoft.OpenApi.Kiota
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN kiota generate -d https://api.kinopoisk.dev/documentation-yaml -c KinoPoiskDev -l csharp
COPY . .
WORKDIR "/src/"
RUN dotnet build "MovieSearchBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MovieSearchBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MovieSearchBot.dll"]
