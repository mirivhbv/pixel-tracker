FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app


USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/PixelApi/PixelApi.csproj", "src/PixelApi/"]
RUN dotnet restore "src/PixelApi/PixelApi.csproj"
COPY . .
WORKDIR "/src/src/PixelApi"
RUN dotnet build "PixelApi.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "PixelApi.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PixelApi.dll"]
