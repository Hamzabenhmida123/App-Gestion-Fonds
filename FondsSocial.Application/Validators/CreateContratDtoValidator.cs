using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class CreateContratDtoValidator : AbstractValidator<CreateContratDto>
    {
        public CreateContratDtoValidator()
        {
            RuleFor(x => x.DecisionId).GreaterThan(0);
            RuleFor(x => x.DateSignature).NotEmpty();
            RuleFor(x => x.MontantPrincipal).GreaterThan(0);
            RuleFor(x => x.FraisGestion).GreaterThanOrEqualTo(0);
        }
    }
}
