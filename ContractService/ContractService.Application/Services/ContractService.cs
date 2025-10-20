using ContractService.Application.DTOs;
using ContractService.Application.Interfaces;
using ContractService.Domain.Entities;
using ContractService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ContractService.Application.Services;

public class ContractService : IContractService
{
    private readonly IContractRepository _repository;
    private readonly IProposalStatusChecker _statusChecker;
    private readonly ILogger<ContractService> _logger;

    public ContractService(
        IContractRepository repository,
        IProposalStatusChecker statusChecker,
        ILogger<ContractService> logger)
    {
        _repository = repository;
        _statusChecker = statusChecker;
        _logger = logger;
    }

    public ContractDTO CreateContract(CreateContractRequestDTO request)
    {
        _logger.LogInformation("Iniciando criação de contrato para ProposalId: {ProposalId}", request.ProposalId);

        var status = _statusChecker.GetStatus(request.ProposalId);
        _logger.LogInformation("Status da proposta {ProposalId}: {Status}", request.ProposalId, status);

        if (status != "Approved")
        {
            _logger.LogWarning("Proposta {ProposalId} não está aprovada. Contrato não será criado.", request.ProposalId);
            throw new InvalidOperationException("Proposta não está aprovada.");
        }

        var existing = _repository.GetByProposalId(request.ProposalId);
        if (existing != null)
        {
            _logger.LogWarning("Proposta {ProposalId} já possui contrato. Operação cancelada.", request.ProposalId);
            throw new InvalidOperationException("Proposta já contratada.");
        }

        var contract = new Contract
        {
            Id = Guid.NewGuid(),
            ProposalId = request.ProposalId,
            ContractDate = DateTime.UtcNow
        };

        _repository.Save(contract);
        _logger.LogInformation("Contrato criado com sucesso. ContractId: {ContractId}", contract.Id);

        return new ContractDTO
        {
            Id = contract.Id,
            ProposalId = contract.ProposalId,
            ContractDate = contract.ContractDate
        };
    }

    public async Task<ContractDTO?> GetByIdAsync(Guid id)
    {
        var contract = await _repository.GetByIdAsync(id);
        return contract != null ? new ContractDTO
        {
            Id = contract.Id,
            ProposalId = contract.ProposalId,
            ContractDate = contract.ContractDate
        } : null;
    }

    public async Task<IEnumerable<ContractDTO>> GetAllAsync()
    {
        var contracts = await _repository.GetAllAsync();
        return contracts.Select(c => new ContractDTO
        {
            Id = c.Id,
            ProposalId = c.ProposalId,
            ContractDate = c.ContractDate
        });
    }
}