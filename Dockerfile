# ============================
# Etapa 1: Build ContractService
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-contract
WORKDIR /app

COPY ContractService/ContractService.Domain/ContractService.Domain.csproj ContractService.Domain/
COPY ContractService/ContractService.Application/ContractService.Application.csproj ContractService.Application/
COPY ContractService/ContractService.Infrastructure/ContractService.Infrastructure.csproj ContractService.Infrastructure/
COPY ContractService/ContractService.API/ContractService.API.csproj ContractService.API/

RUN dotnet restore ContractService.API/ContractService.API.csproj

COPY ContractService/ ./

RUN dotnet publish ContractService.API/ContractService.API.csproj -c Release -o /app-contract --no-self-contained

# ============================
# Etapa 2: Build ProposalService
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-proposal
WORKDIR /app

COPY ProposalService/ProposalService.Domain/ProposalService.Domain.csproj ProposalService.Domain/
COPY ProposalService/ProposalService.Application/ProposalService.Application.csproj ProposalService.Application/
COPY ProposalService/ProposalService.Infrastructure/ProposalService.Infrastructure.csproj ProposalService.Infrastructure/
COPY ProposalService/ProposalService.API/ProposalService.API.csproj ProposalService.API/

RUN dotnet restore ProposalService.API/ProposalService.API.csproj

COPY ProposalService/ ./

RUN dotnet publish ProposalService.API/ProposalService.API.csproj -c Release -o /app-proposal --no-self-contained

# ============================
# Etapa 3: Runtime base (usado por ambos)
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app