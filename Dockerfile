# ==========================================
# ROYAL TREASURY
# RENDER DEPLOYMENT
#
# .NET SDK 10.0.301 is deliberately pinned.
#
# SDK 10.0.302 currently has a container
# regression that can cause dotnet commands
# to fail during Docker builds.
# ==========================================


# ==========================================
# BUILD STAGE
# ==========================================

FROM mcr.microsoft.com/dotnet/sdk:10.0.301 AS build


WORKDIR /src


# ==========================================
# COPY PROJECT FILE FIRST
# ==========================================

COPY ["Website of Everything.csproj", "./"]


# ==========================================
# RESTORE
# ==========================================

RUN dotnet restore "./Website of Everything.csproj"


# ==========================================
# COPY THE REST OF THE PROJECT
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
# SDK 10.0.301 ships with the .NET 10.0.9
# runtime, so use the matching runtime here.
# ==========================================

FROM mcr.microsoft.com/dotnet/aspnet:10.0.9 AS final


WORKDIR /app


# ==========================================
# COPY PUBLISHED APPLICATION
# ==========================================

COPY --from=build /app/publish .


# ==========================================
# RENDER NETWORK CONFIGURATION
#
# Render expects the application to listen
# on all interfaces.
# ==========================================

ENV ASPNETCORE_URLS=http://0.0.0.0:10000


EXPOSE 10000


# ==========================================
# START ROYAL TREASURY
# ==========================================

ENTRYPOINT ["dotnet", "Website_of_Everything.dll"]