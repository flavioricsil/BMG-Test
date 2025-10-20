using Microsoft.EntityFrameworkCore;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Interfaces;
using ProposalService.Infrastructure.Data;

namespace ProposalService.Infrastructure.Repositories;

public class ProposalRepository : IProposalRepository
{
    private readonly ProposalDbContext _context;

    public ProposalRepository(ProposalDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Proposal proposal)
    {
        await _context.Proposals.AddAsync(proposal);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Proposal>> GetAllAsync()
    {
        return await _context.Proposals.ToListAsync();
    }

    public async Task<Proposal?> GetByIdAsync(Guid id)
    {
        return await _context.Proposals.FindAsync(id);
    }

    public async Task UpdateAsync(Proposal proposal)
    {
        _context.Proposals.Update(proposal);
        await _context.SaveChangesAsync();
    }
}