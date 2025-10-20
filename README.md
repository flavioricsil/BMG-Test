# API de Seguros

Um sistema baseado em microsserviços para gerenciar propostas e contratos de seguros, desenvolvido em .NET 8 (C#).

---

## Sumário

1. Tecnologias
2. Arquitetura e Padrões
3. Funcionalidade
4. Regras de Negócio
5. Execução com Docker
6. Endpoints da API
7. Testes
8. Roadmap
9. Licença

---

## Tecnologias

| Categoria         | Tecnologia           | Descrição                                      |
|------------------|----------------------|------------------------------------------------|
| Backend           | .NET 8 (C#)          | Framework principal da API                     |
| Banco de Dados    | PostgreSQL           | Persistência de dados                          |
| ORM               | Entity Framework Core| Mapeamento Objeto-Relacional                   |
| Mensageria        | RabbitMQ             | Comunicação entre microsserviços               |
| Containerização   | Docker               | Execução isolada e padronizada                 |
| Testes            | xUnit                | Testes unitários e de integração               |
| Documentação      | Swagger/OpenAPI      | Interface interativa dos endpoints             |
| Padrões           | MediatR              | Implementação do padrão Mediator               |

---

## Arquitetura e Padrões

- Arquitetura Hexagonal (Ports & Adapters)
- Separação em microsserviços: ProposalService e ContractService
- Comunicação entre microsserviços via HTTP REST e mensageria (RabbitMQ)
- Aplicação dos princípios SOLID
- Padrões utilizados:
  - Repository e Unit of Work
  - Mediator (via MediatR) para comandos e consultas desacoplados

---

## Funcionalidade

**ProposalService**:
- Criar proposta de seguro
- Listar propostas
- Atualizar status da proposta (Em Análise, Aprovada, Rejeitada)
- Expor API REST

**ContractService**:
- Contratar uma proposta (somente se Aprovada)
- Armazenar informações da contratação (ID da proposta, data de contratação)
- Comunicar-se com o ProposalService para verificar status da proposta
- Expor API REST

Quando uma proposta é aprovada no ProposalService, uma mensagem é publicada em uma fila do RabbitMQ. O ContractService escuta essa fila e automaticamente cria um novo contrato para a proposta aprovada.

---

## Regras de Negócio

1. Propostas com status "Rejeitada" não podem ser contratadas
2. Propostas com status "Em Análise" não podem ser contratadas
3. Apenas propostas com status "Aprovada" podem ser contratadas

---

## Execução com Docker

Pré-requisitos:
- Docker e Git instalados

Passos:

1. Clonar o repositório:

```bash
git clone https://github.com/flavioricsil/BMG-Test.git
cd api-seguros
```

2. Executar com Docker Compose:

```bash
docker network create microservices-net
docker-compose up --build -d
```

3. Acessar:
- ProposalService: http://localhost:5001/swagger
- ContractService: http://localhost:5002/swagger

---

## Endpoints da API

### ProposalService

| Método  | Caminho             | Descrição                  |
|---------|---------------------|----------------------------|
| GET     | /proposals          | Lista todas as propostas    |
| POST    | /proposals          | Cria uma nova proposta      |
| PUT     | /proposals/{id}     | Atualiza status da proposta |

### ContractService

| Método  | Caminho                     | Descrição                          |
|---------|-----------------------------|------------------------------------|
| POST    | /contracts                  | Contrata uma proposta aprovada     |
| GET     | /contracts/{proposalId}     | Consulta informações de contrato   |

---

## Testes

- Cobertura de testes acima de 80%
- Foco nas regras de negócio
- Execução local:

```bash
cd .\ProposalService\
dotnet test
cd .\ContractService\
dotnet test
```

---

## Roadmap

### Arquitetura

- Implementar CQRS
- Adicionar autenticação e autorização
- Implementar observabilidade (logs, métricas, rastreamento)

### DevOps

- CI/CD com GitHub Actions ou Azure DevOps
- Deploy em Azure (App Service ou AKS)

### Funcionalidade

- Integração com sistema de apólices
- Histórico de alterações nas propostas
- Notificações em tempo real

---

## Licença

Este projeto é destinado exclusivamente para avaliação técnica e demonstração de habilidades em desenvolvimento de software.
