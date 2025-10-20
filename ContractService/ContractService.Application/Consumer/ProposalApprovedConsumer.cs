using ContractService.Domain.Entities;
using ContractService.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using ProposalService.Application.Events;

namespace ContractService.Application.Consumer;

public class ProposalApprovedConsumer : IConsumer<ProposalApprovedEvent>
{
    private readonly IContractRepository _repository;
    private readonly ILogger<ProposalApprovedConsumer> _logger;

    public ProposalApprovedConsumer(IContractRepository repository, ILogger<ProposalApprovedConsumer> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProposalApprovedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Evento recebido: criando contrato para ProposalId: {ProposalId}", message.ProposalId);

        var existing = _repository.GetByProposalId(message.ProposalId);
        if (existing != null)
        {
            _logger.LogWarning("Contrato já existe para ProposalId: {ProposalId}", message.ProposalId);
            return;
        }

        var contract = new Contract
        {
            Id = Guid.NewGuid(),
            ProposalId = message.ProposalId,
            ContractDate = DateTime.UtcNow
        };

        _repository.Save(contract);
        _logger.LogInformation("Contrato criado com sucesso. ContractId: {ContractId}", contract.Id);
    }
}