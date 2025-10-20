using ContractService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContractService.Domain.Interfaces;

public interface IContractRepository
{
    void Save(Contract contract);
    Contract? GetByProposalId(Guid proposalId);
    Task<Contract?> GetByIdAsync(Guid id);
    Task<IEnumerable<Contract>> GetAllAsync();
}
