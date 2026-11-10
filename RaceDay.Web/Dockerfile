
# STAGE 1: BASE (Runtime Environment)
# This creates the environment that the final app will run in.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Expose the ports the container will listen on
EXPOSE 8080
EXPOSE 8081

# Create a folder specifically to store ASP.NET Core session/security keys
# This prevents Docker from throwing DataProtection warnings and session crashes
RUN mkdir -p /app/keys


# STAGE 2: BUILD (Compilation)
# This stage uses the full SDK (Software Development Kit) to compile the code.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy ONLY the project file first. This allows Docker to cache the NuGet 
# dependencies so subsequent builds are much faster.
COPY ["RaceDay.Web/RaceDay.Web.csproj", "RaceDay.Web/"]
RUN dotnet restore "./RaceDay.Web/RaceDay.Web.csproj"

# Copy the rest of the source code and build the project
COPY . .
WORKDIR "/src/RaceDay.Web"
RUN dotnet build "./RaceDay.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build


# STAGE 3: PUBLISH (Optimisation)
# This stage takes the compiled code and publishes it, stripping out 
# unnecessary files to make the final image as small as possible.

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RaceDay.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# STAGE 4: FINAL (Production)
# This is the actual image that gets deployed. It copies the published files 
# from Stage 3 and runs the application.

FROM base AS final
WORKDIR /app
# Copy the published files from the previous stage into the final environment
COPY --from=publish /app/publish .
# Start the application
ENTRYPOINT ["dotnet", "RaceDay.Web.dll"]