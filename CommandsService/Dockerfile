# Stage 1: Build stage (uses SDK image which includes build tools)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
# ^ Base image with .NET 8.0 SDK for building the application
# AS build-env names this stage for later reference

WORKDIR /App
# ^ Sets the working directory inside the container to /App


COPY CommandsService/CommandsService.csproj ./
# ^ Copies just the project file(s) first (optimization for layer caching)
# This allows Docker to cache the restore step when only code changes

RUN dotnet restore
# ^ Restores NuGet packages (dependencies) for the project
# Runs only when project files change (thanks to previous COPY optimization)

COPY CommandsService/ ./
# ^ Copies all remaining source code into the container
# This includes all .cs files, config files, etc.

RUN dotnet publish CommandsService.csproj -c Release -o Out
# ^ Builds and publishes the application in Release configuration
# -c Release: Builds in Release mode (optimized)
# -o Out: Outputs published files to /App/Out directory


# Stage 2: Runtime stage (uses smaller runtime-only image)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
# ^ Base image with just .NET 8.0 runtime (smaller than SDK image)
# This makes the final image much smaller (~100MB vs ~1GB for SDK)

WORKDIR /App
# ^ Sets working directory in the runtime container

COPY --from=build-env /App/Out ./
# ^ Copies only the published output from the build stage
# --from=build-env takes files from the previously named build stage
# Copies from /App/Out in build container to /App in runtime container
EXPOSE 5237

ENTRYPOINT [ "dotnet", "CommandsService.dll" ]
# ^ Specifies the command to run when the container starts
# Runs the published DLL (assuming project name is PlatformService)
