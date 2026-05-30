# Contact Keeper Dockerfile
# This Dockerfile is used to build and run the Contact Keeper application in a Docker container.
# [1] Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

# [2] Copy all project files (including subdirectories) and restore dependencies
COPY . .
RUN dotnet clean
RUN dotnet restore --disable-parallel --no-cache
RUN dotnet dev-certs https --trust

# [3] Publish only the main project (exclude test projects)
RUN dotnet publish "ContactKeeper.csproj" -c Release -o out --no-restore

# [4] Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

COPY --from=build /app/out .

ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "ContactKeeper.dll"]