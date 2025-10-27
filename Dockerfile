# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY MovizoneApp/MovizoneApp.csproj MovizoneApp/
RUN dotnet restore "MovizoneApp/MovizoneApp.csproj"

# Copy everything else and build
COPY MovizoneApp/ MovizoneApp/
WORKDIR /src/MovizoneApp
RUN dotnet build "MovizoneApp.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "MovizoneApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy published app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "MovizoneApp.dll"]
