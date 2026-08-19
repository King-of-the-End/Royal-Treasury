# ==========================================
# ROYAL TREASURY
# RENDER DIAGNOSTIC DOCKERFILE
#
# This version is intentionally diagnostic.
# It prints .NET runtime information before
# starting Royal Treasury so that we can see
# exactly where the status 139 crash occurs.
# ==========================================


# ==========================================
# BUILD STAGE
# ==========================================

FROM mcr.microsoft.com/dotnet/sdk:10.0.301 AS build


WORKDIR /src


# ==========================================
# COPY PROJECT FILE
#
# Copying the .csproj first allows Docker to
# cache the restore step independently from
# changes to the rest of the source code.
# ==========================================

COPY ["Website of Everything.csproj", "./"]


# ==========================================
# RESTORE
# ==========================================

RUN dotnet restore "./Website of Everything.csproj"


# ==========================================
# COPY THE REST OF ROYAL TREASURY
# ==========================================

COPY . .


# ==========================================
# PUBLISH
# ==========================================

RUN dotnet publish "./Website of Everything.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore


# ==========================================
# RUNTIME STAGE
#
# SDK 10.0.301 corresponds to the
# .NET / ASP.NET Core 10.0.9 runtime.
# ==========================================

FROM mcr.microsoft.com/dotnet/aspnet:10.0.9 AS final


WORKDIR /app


# ==========================================
# COPY PUBLISHED APPLICATION
# ==========================================

COPY --from=build /app/publish .


# ==========================================
# RENDER NETWORKING
#
# Render routes traffic to the application
# through port 10000 by default.
#
# ASP.NET Core must listen on all network
# interfaces rather than localhost.
# ==========================================

ENV ASPNETCORE_URLS=http://0.0.0.0:10000


EXPOSE 10000


# ==========================================
# DIAGNOSTIC STARTUP
#
# IMPORTANT:
#
# Keep this ENTRYPOINT on ONE LINE.
#
# It will:
#
# 1. print a marker
# 2. run dotnet --info
# 3. print another marker
# 4. start Royal Treasury normally
#
# This allows us to determine whether the
# .NET runtime itself is crashing or whether
# the crash happens when the application
# begins running.
# ==========================================

ENTRYPOINT ["/bin/sh", "-c", "echo '===== DOTNET INFO =====' && dotnet --info && echo '===== STARTING ROYAL TREASURY =====' && exec dotnet Website_of_Everything.dll"]