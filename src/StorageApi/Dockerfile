FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5277

ENV ASPNETCORE_URLS=http://+:5277

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/StorageApi/StorageApi.csproj", "src/StorageApi/"]
RUN dotnet restore "src/StorageApi/StorageApi.csproj"
COPY . .
WORKDIR "/src/src/StorageApi"
RUN dotnet build "StorageApi.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "StorageApi.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StorageApi.dll"]
