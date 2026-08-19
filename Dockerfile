# ==========================================
# ROYAL TREASURY
# RENDER / .NET 10
#
# Versions are deliberately pinned instead
# of using the floating "10.0" tags.
# ==========================================


# ==========================================
# BUILD STAGE
# ==========================================

FROM mcr.microsoft.com/dotnet/sdk:10.0.302-noble AS build


WORKDIR /src


# ==========================================
# COPY PROJECT FILE FIRST
#
# This lets Docker cache NuGet restore when
# only source files change.
# ==========================================

COPY ["Website of Everything.csproj", "./"]


# ==========================================
# RESTORE
# ==========================================

RUN dotnet restore "./Website of Everything.csproj"


# ==========================================
# COPY PROJECT
# ==========================================

COPY . .


# ==========================================
# PUBLISH
# ==========================================

RUN dotnet publish "./Website of Everything.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    --use-current-runtime false


# ==========================================
# RUNTIME STAGE
# ==========================================

FROM mcr.microsoft.com/dotnet/aspnet:10.0.10-noble AS final


WORKDIR /app


# ==========================================
# COPY PUBLISHED APP
# ==========================================

COPY --from=build /app/publish .


# ==========================================
# RENDER
#
# Render's standard web-service port is
# 10000.
# ==========================================

ENV ASPNETCORE_URLS=http://0.0.0.0:10000

ENV ASPNETCORE_ENVIRONMENT=Production


EXPOSE 10000


# ==========================================
# START ROYAL TREASURY
# ==========================================

ENTRYPOINT ["dotnet", "Website_of_Everything.dll"]