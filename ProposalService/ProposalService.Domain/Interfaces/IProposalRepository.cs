using ProposalService.Domain.Entities;

namespace ProposalService.Domain.Interfaces;

public interface IProposalRepository
{
    Task<Proposal?> GetByIdAsync(Guid id);
    Task<IEnumerable<Proposal>> GetAllAsync();
    Task AddAsync(Proposal proposal);
    Task UpdateAsync(Proposal proposal);
}