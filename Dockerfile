# ==========================================
# ROYAL TREASURY
# RENDER DIAGNOSTIC / DEPLOYMENT DOCKERFILE
# ==========================================


# ==========================================
# BUILD STAGE
#
# Render requires Linux AMD64 images.
#
# .NET SDK 10.0.302 contains:
# .NET Runtime 10.0.10
# ASP.NET Core 10.0.10
# ==========================================

FROM --platform=linux/amd64 \
    mcr.microsoft.com/dotnet/sdk:10.0.302-noble \
    AS build


WORKDIR /src


# ==========================================
# COPY PROJECT FILE
# ==========================================

COPY ["Website of Everything.csproj", "./"]


# ==========================================
# RESTORE
#
# -m:1 keeps MSBuild single-process.
# This also avoids unnecessary parallelism
# while diagnosing the Render environment.
# ==========================================

RUN dotnet restore \
    "./Website of Everything.csproj" \
    -m:1


# ==========================================
# COPY PROJECT
# ==========================================

COPY . .


# ==========================================
# PUBLISH
# ==========================================

RUN dotnet publish \
    "./Website of Everything.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    -m:1


# ==========================================
# RUNTIME STAGE
# ==========================================

FROM --platform=linux/amd64 \
    mcr.microsoft.com/dotnet/aspnet:10.0.10-noble \
    AS final


WORKDIR /app


# ==========================================
# COPY PUBLISHED APP
# ==========================================

COPY --from=build /app/publish .


# ==========================================
# RENDER NETWORKING
#
# Render recommends binding to 0.0.0.0 and
# the PORT environment variable.
#
# PORT defaults to 10000 on Render.
# ==========================================

ENV ASPNETCORE_URLS=http://0.0.0.0:10000


# ==========================================
# TEMPORARY CORECLR DIAGNOSTIC
#
# We are disabling tiered compilation while
# diagnosing the immediate SIGSEGV.
#
# If the application becomes stable, we can
# later test turning this back on.
# ==========================================

ENV DOTNET_TieredCompilation=0

ENV DOTNET_TieredPGO=0


# ==========================================
# NORMAL PRODUCTION ENVIRONMENT
# ==========================================

ENV ASPNETCORE_ENVIRONMENT=Production


EXPOSE 10000


# ==========================================
# STARTUP DIAGNOSTICS
#
# This deliberately prints dotnet --info
# before starting Royal Treasury.
#
# That tells us whether:
#
# 1. The .NET runtime itself crashes
#
# OR
#
# 2. .NET starts successfully and the crash
#    occurs while Royal Treasury launches.
# ==========================================

ENTRYPOINT [
    "/bin/sh",
    "-c",
    "echo '===== DOTNET INFO =====' && dotnet --info && echo '===== STARTING ROYAL TREASURY =====' && exec dotnet Website_of_Everything.dll"
]