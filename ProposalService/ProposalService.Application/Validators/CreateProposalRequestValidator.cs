using FluentValidation;
using ProposalService.Application.DTOs;

namespace ProposalService.Application.Validators;

public class CreateProposalRequestDtoValidator : AbstractValidator<CreateProposalRequestDto>
{
    public CreateProposalRequestDtoValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Customer name is required");

        RuleFor(x => x.InsuranceType)
            .NotEmpty().WithMessage("Insurance type is required");
    }
}