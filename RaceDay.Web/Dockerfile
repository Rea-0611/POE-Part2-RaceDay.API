# 1. Base image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Create the keys folder for Data Protection
RUN mkdir -p /app/keys && chown -R app:app /app/keys

# 2. Build image (SDK is used to compile the code)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["RaceDay.Web/RaceDay.Web.csproj", "RaceDay.Web/"]
RUN dotnet restore "./RaceDay.Web/RaceDay.Web.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/RaceDay.Web"
RUN dotnet build "./RaceDay.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 3. Publish the app
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RaceDay.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 4. Final image (lightweight)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RaceDay.Web.dll"]