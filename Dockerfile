# Primeira etapa: Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Define o diretório de trabalho no container
WORKDIR /app

# Copia os arquivos .csproj para o container
COPY *.csproj ./

# Restaura os pacotes NuGet
RUN dotnet restore --disable-parallel

# Copia o restante do código da aplicação
COPY . .

# Limpa os artefatos de build
RUN dotnet clean ContactKeeper.csproj

# Compila a aplicação
RUN dotnet build ContactKeeper.csproj -c Release

# Publica a aplicação
RUN dotnet publish -c Release -o out 

# Segunda etapa: Criação da imagem final
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Define o diretório de trabalho no container
WORKDIR /app

# Copia a aplicação publicada da camada de build
COPY --from=build /app/out .

# Configura as variáveis de ambiente para o runtime
ENV ASPNETCORE_ENVIRONMENT=Development

# Exposição da porta
EXPOSE 5000

# Configura a URL da porta
ENV ASPNETCORE_URLS=http://+:5000

# Define o ponto de entrada para a aplicação
ENTRYPOINT ["dotnet", "ContactKeeper.dll"]
