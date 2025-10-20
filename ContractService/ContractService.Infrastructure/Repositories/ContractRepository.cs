using ContractService.Domain.Entities;
using ContractService.Domain.Interfaces;
using ContractService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContractService.Infrastructure.Repositories;

public class ContractRepository : IContractRepository
{
    private readonly ContractDbContext _context;

    public ContractRepository(ContractDbContext context)
    {
        _context = context;
    }

    public void Save(Contract contract)
    {
        _context.Contracts.Add(contract);
        _context.SaveChanges();
    }

    public Contract? GetByProposalId(Guid proposalId)
    {
        return _context.Contracts.FirstOrDefault(c => c.ProposalId == proposalId);
    }

    public async Task<Contract?> GetByIdAsync(Guid id)
    {
        return await _context.Contracts.FindAsync(id);
    }

    public async Task<IEnumerable<Contract>> GetAllAsync()
    {
        return await _context.Contracts.ToListAsync();
    }
}
