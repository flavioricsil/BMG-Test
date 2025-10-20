using ProposalService.Domain.Enums;

namespace ProposalService.Domain.Entities;

public class Proposal
{
    public Guid Id { get; private set; }
    public string CustomerName { get; private set; }
    public string InsuranceType { get; private set; }
    public ProposalStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Proposal(string customerName, string insuranceType)
    {
        Id = Guid.NewGuid();
        CustomerName = customerName;
        InsuranceType = insuranceType;
        Status = ProposalStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void ChangeStatus(ProposalStatus newStatus)
    {
        Status = newStatus;
    }
}