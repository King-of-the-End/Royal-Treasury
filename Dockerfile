# ==========================================
# BUILD STAGE
# ==========================================

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src


# Copy the project file first so Docker can
# cache NuGet restore separately.
COPY ["Website of Everything.csproj", "./"]


# Restore dependencies.
RUN dotnet restore "./Website of Everything.csproj"


# Copy the rest of Royal Treasury.
COPY . .


# Publish the application.
RUN dotnet publish "./Website of Everything.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore


# ==========================================
# RUNTIME STAGE
# ==========================================

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app


# Copy the published application.
COPY --from=build /app/publish .


# Render expects public web services to
# listen on 0.0.0.0.
#
# Port 10000 is Render's default web-service
# port.
ENV ASPNETCORE_URLS=http://0.0.0.0:10000


EXPOSE 10000


# Start Royal Treasury.
ENTRYPOINT ["dotnet", "Website_of_Everything.dll"]