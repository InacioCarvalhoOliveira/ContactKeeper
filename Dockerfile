# Contact Keeper Dockerfile
# This Dockerfile is used to build and run the Contact Keeper application in a Docker container.
# [1] Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# [2] Copy project files and restore dependencies
COPY *.csproj ./
RUN dotnet restore --disable-parallel --no-cache
RUN dotnet dev-certs https --trust

# [3] Copy remaining source code and publish
COPY . .

RUN dotnet publish "ContactKeeper.csproj" -c Release -o out

# [4] Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/out .

ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "ContactKeeper.dll"]

