namespace ContractService.Application.DTOs;

public class ContractDTO
{
    public Guid Id { get; set; }
    public Guid ProposalId { get; set; }
    public DateTime ContractDate { get; set; }
}