# Primeira etapa: Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory in the container
WORKDIR /app

# Copy the .csproj files
COPY *.csproj ./

# Restore NuGet packages
RUN dotnet restore

# Copy the rest of the application code
COPY . .

# Clean build artifacts
RUN dotnet clean ContactKeeper.csproj

# Build the application
RUN dotnet build ContactKeeper.csproj -c Release

# Publish the application
RUN dotnet publish -c Release -o out

# Use the official .NET runtime image as a base for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory in the container
WORKDIR /app

# Copy the published application from the build layer
COPY --from=build /app/out .

# Set environment variables for runtime
ENV ASPNETCORE_ENVIRONMENT=Development

# Set port url
ENV ASPNETCORE_URLS=http://*:80

# Set the entry point for the application
ENTRYPOINT ["dotnet", "ContactKeeper.dll"]
