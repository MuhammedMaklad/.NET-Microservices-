# Stage 1: Build stage (uses SDK image which includes build tools)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
# ^ Base image with .NET 8.0 SDK for building the application
# AS build-env names this stage for later reference

WORKDIR /App
# ^ Sets the working directory inside the container to /App

COPY *.csproj ./
# ^ Copies just the project file(s) first (optimization for layer caching)
# This allows Docker to cache the restore step when only code changes

RUN dotnet restore
# ^ Restores NuGet packages (dependencies) for the project
# Runs only when project files change (thanks to previous COPY optimization)

COPY . ./
# ^ Copies all remaining source code into the container
# This includes all .cs files, config files, etc.

RUN dotnet publish -c Release -o Out
# ^ Builds and publishes the application in Release configuration
# -c Release: Builds in Release mode (optimized)
# -o Out: Outputs published files to /App/Out directory


# Stage 2: Runtime stage (uses smaller runtime-only image)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
# ^ Base image with just .NET 8.0 runtime (smaller than SDK image)
# This makes the final image much smaller (~100MB vs ~1GB for SDK)

WORKDIR /App
# ^ Sets working directory in the runtime container

COPY --from=build-env /App/Out .
# ^ Copies only the published output from the build stage
# --from=build-env takes files from the previously named build stage
# Copies from /App/Out in build container to /App in runtime container

ENTRYPOINT [ "dotnet", "PlatformService.dll" ]
# ^ Specifies the command to run when the container starts
# Runs the published DLL (assuming project name is PlatformService)


# ! Key Features of This Dockerfile:
# ? Multi-stage Build:

# * Stage 1 (build-env): Uses full SDK for building

# * Stage 2: Uses smaller runtime image for execution

# * Results in much smaller final image (better security and performance)

# ? Layer Caching Optimization:

# * Copies project files separately first to optimize dotnet restore caching

# * Restore layer will be cached until project dependencies change

# ? Security Best Practices:

# * Uses official Microsoft .NET images

# * Runtime image has fewer components (reduced attack surface)

# ? Efficiency:

# * Only publishes in Release mode

# * Only necessary files are copied to final image

# ? Conventions:

# * Standard working directory (/App)

# * Clear separation between build and runtime environments