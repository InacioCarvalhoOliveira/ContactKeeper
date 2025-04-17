# # Primeira etapa: Build da aplicação
# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# # Define o diretório de trabalho no container
# WORKDIR /app

# # Copia os arquivos .csproj para o container
# COPY *.csproj ./

# # Restaura os pacotes NuGet
# RUN dotnet restore --disable-parallel

# # Copia o restante do código da aplicação
# COPY . .

# # Remove the obj folder to avoid conflicts
# #RUN rm -rf obj

# # Remove the problematic AssemblyAttributes.cs file
# #RUN rm -f obj/Release/net8.0/.NETCoreApp,Version=v8.0.AssemblyAttributes.cs

# RUN dotnet clean ContactKeeper.csproj 
# # Compila a aplicação
# RUN dotnet build ContactKeeper.csproj -c Release

# # Publica a aplicação
# RUN dotnet publish -c Release -o out 

# # Segunda etapa: Criação da imagem final
# FROM mcr.microsoft.com/dotnet/aspnet:8.0

# # Define o diretório de trabalho no container
# WORKDIR /app

# # Copia a aplicação publicada da camada de build
# COPY --from=build /app/out .

# # Configura as variáveis de ambiente para o runtime
# ENV ASPNETCORE_ENVIRONMENT=Development

# # Exposição da porta
# EXPOSE 5000

# # Configura a URL da porta
# ENV ASPNETCORE_URLS=http://+:5000

# # Define o ponto de entrada para a aplicação
# ENTRYPOINT ["dotnet", "ContactKeeper.dll"]


# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy project files and restore dependencies
COPY *.csproj ./
RUN dotnet restore --disable-parallel --no-cache
RUN dotnet dev-certs https --trust

# Install dotnet-ef tool globally
RUN dotnet tool install --global dotnet-ef

# Copy remaining source code and publish
COPY . .

RUN dotnet publish "ContactKeeper.csproj" -c Release -o out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/out .

ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "ContactKeeper.dll"]

