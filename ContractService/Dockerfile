# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia todos os arquivos do contexto (a pasta raiz da solução, BMG)
# para o WORKDIR /src.
COPY . .

# Restaura as dependências. O caminho AGORA DEVE incluir a pasta do projeto (ContractService)
RUN dotnet restore ContractService/ContractService.API/ContractService.API.csproj

# Publica a aplicação. O caminho AGORA DEVE incluir a pasta do projeto (ContractService)
RUN dotnet publish ContractService/ContractService.API/ContractService.API.csproj -c Release -o /app/publish --no-self-contained

# Estágio de Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# .NET 8 roda na porta 8080 por padrão, não root.
# EXPOSE 8080 é opcional, mas ajuda a documentar.
EXPOSE 8080 

ENTRYPOINT ["dotnet", "ContractService.API.dll"]
