# ==========================================
# ROYAL TREASURY - RENDER
# ==========================================

# ----------------------------
# BUILD
# ----------------------------

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy project file first so Docker can cache restore
COPY ["Website of Everything.csproj", "./"]

# Restore dependencies
RUN dotnet restore "./Website of Everything.csproj" -m:1

# Copy everything else
COPY . .

# Publish
RUN dotnet publish "./Website of Everything.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    -m:1


# ----------------------------
# RUNTIME
# ----------------------------

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

# Render supplies PORT.
# Fall back to 10000 when running somewhere
# that does not supply it.
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT:-10000}

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_NOLOGO=1

EXPOSE 10000

ENTRYPOINT ["dotnet", "Website_of_Everything.dll"]