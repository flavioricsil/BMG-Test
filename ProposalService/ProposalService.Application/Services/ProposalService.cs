using AutoMapper;
using MassTransit;
using Microsoft.Extensions.Logging;
using ProposalService.Application.DTOs;
using ProposalService.Application.Events;
using ProposalService.Application.Interfaces;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Enums;
using ProposalService.Domain.Interfaces;

namespace ProposalService.Application.Services;

public class ProposalService : IProposalService
{
    private readonly IProposalRepository _repository;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<ProposalService> _logger;

    public ProposalService(
        IProposalRepository repository,
        IMapper mapper,
        IPublishEndpoint publishEndpoint,
        ILogger<ProposalService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<Guid> CreateProposalAsync(CreateProposalRequestDto request)
    {
        var proposal = new Proposal(request.CustomerName, request.InsuranceType);
        await _repository.AddAsync(proposal);
        _logger.LogInformation("Proposta criada com ID: {ProposalId}", proposal.Id);
        return proposal.Id;
    }

    public async Task<IEnumerable<ProposalDto>> GetAllAsync()
    {
        var proposals = await _repository.GetAllAsync();
        _logger.LogInformation("Listando todas as propostas. Total: {Count}", proposals.Count());
        return _mapper.Map<IEnumerable<ProposalDto>>(proposals);
    }

    public async Task<ProposalDto?> GetByIdAsync(Guid id)
    {
        var proposal = await _repository.GetByIdAsync(id);
        if (proposal is null)
        {
            _logger.LogWarning("Proposta com ID {ProposalId} não encontrada.", id);
            return null;
        }

        _logger.LogInformation("Proposta com ID {ProposalId} recuperada.", id);
        return _mapper.Map<ProposalDto>(proposal);
    }

    public async Task<bool> ChangeStatusAsync(Guid id, string status)
    {
        var proposal = await _repository.GetByIdAsync(id);
        if (proposal == null)
        {
            _logger.LogWarning("Tentativa de alterar status de proposta inexistente: {ProposalId}", id);
            return false;
        }

        if (!Enum.TryParse<ProposalStatus>(status, true, out var newStatus))
        {
            _logger.LogWarning("Status inválido informado: {Status}", status);
            return false;
        }

        proposal.ChangeStatus(newStatus);
        await _repository.UpdateAsync(proposal);
        _logger.LogInformation("Status da proposta {ProposalId} alterado para {Status}", proposal.Id, newStatus);

        if (newStatus == ProposalStatus.Approved)
        {
            var eventMessage = new ProposalApprovedEvent
            {
                ProposalId = proposal.Id,
                CustomerName = proposal.CustomerName,
                InsuranceType = proposal.InsuranceType,
                CreatedAt = proposal.CreatedAt
            };

            await _publishEndpoint.Publish(eventMessage);
            _logger.LogInformation("Evento ProposalApprovedEvent publicado para ProposalId: {ProposalId}", proposal.Id);
        }

        return true;
    }
}