using System;

namespace ContractService.Application.Interfaces;

public interface IProposalStatusChecker
{
    string GetStatus(Guid proposalId);
}