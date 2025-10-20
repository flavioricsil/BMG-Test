namespace ContractService.Domain.Entities;

public class Contract
{
    public Guid Id { get; set; }
    public Guid ProposalId { get; set; }
    public DateTime ContractDate { get; set; }
}