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
RUN dotnet publish -c Release -o out --no-restore

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

# Configura o fuso horário para America/Sao_Paulo
ENV TZ=America/Sao_Paulo
RUN apt-get update && apt-get install -y tzdata && ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# Define o ponto de entrada para a aplicação
ENTRYPOINT ["dotnet", "ContactKeeper.dll"]
