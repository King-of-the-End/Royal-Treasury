# ==========================================
# ROYAL TREASURY
# RENDER DOCKERFILE
#
# Uses the current .NET 10 servicing release
# and forces single-node MSBuild during restore/
# publish to avoid the .NET 10.0.9 Linux
# multi-process MSBuild/CoreCLR crash path.
# ==========================================


# ==========================================
# BUILD STAGE
# ==========================================

FROM mcr.microsoft.com/dotnet/sdk:10.0.302 AS build

WORKDIR /src


# ==========================================
# COPY PROJECT FILE
# ==========================================

COPY ["Website of Everything.csproj", "./"]


# ==========================================
# RESTORE
#
# -m:1 keeps MSBuild in a single node.
# ==========================================

RUN dotnet restore "./Website of Everything.csproj" -m:1


# ==========================================
# COPY PROJECT
# ==========================================

COPY . .


# ==========================================
# PUBLISH
#
# Again use one MSBuild node so the Linux
# build does not enter the multiprocess node
# IPC path implicated in the 10.0.9 crash.
# ==========================================

RUN dotnet publish "./Website of Everything.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    -m:1


# ==========================================
# RUNTIME STAGE
# ==========================================

FROM mcr.microsoft.com/dotnet/aspnet:10.0.10 AS final

WORKDIR /app

COPY --from=build /app/publish .


# ==========================================
# RENDER NETWORKING
# ==========================================

ENV ASPNETCORE_URLS=http://0.0.0.0:10000
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_NOLOGO=1

EXPOSE 10000


# ==========================================
# STARTUP DIAGNOSTICS
#
# These markers make it obvious in Render's
# logs whether the runtime itself starts and
# whether the application process is reached.
# ==========================================

ENTRYPOINT ["/bin/sh", "-c", "echo '===== DOTNET INFO =====' && dotnet --info && echo '===== STARTING ROYAL TREASURY =====' && exec dotnet Website_of_Everything.dll"]
