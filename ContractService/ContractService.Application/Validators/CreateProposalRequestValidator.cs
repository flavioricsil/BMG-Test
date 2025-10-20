using ContractService.Application.DTOs;
using FluentValidation;

namespace ContractService.Application.Validators;

public class CreateContractRequestValidator : AbstractValidator<CreateContractRequestDTO>
{
    public CreateContractRequestValidator()
    {
        RuleFor(x => x.ProposalId)
            .NotEmpty().WithMessage("Proposal ID is required");
    }
}