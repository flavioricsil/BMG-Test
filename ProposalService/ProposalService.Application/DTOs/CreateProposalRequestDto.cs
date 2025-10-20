namespace ProposalService.Application.DTOs;

public class CreateProposalRequestDto
{
    public string CustomerName { get; set; } = string.Empty;
    public string InsuranceType { get; set; } = string.Empty;
}