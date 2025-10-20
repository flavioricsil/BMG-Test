using ProposalService.Application.DTOs;

namespace ProposalService.Application.Interfaces;

public interface IProposalService
{
    Task<Guid> CreateProposalAsync(CreateProposalRequestDto request);
    Task<IEnumerable<ProposalDto>> GetAllAsync();
    Task<ProposalDto?> GetByIdAsync(Guid id);
    Task<bool> ChangeStatusAsync(Guid id, string status);
}