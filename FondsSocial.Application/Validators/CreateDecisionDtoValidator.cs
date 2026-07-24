using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class CreateDecisionDtoValidator : AbstractValidator<CreateDecisionDto>
    {
        public CreateDecisionDtoValidator()
        {
            RuleFor(x => x.DemandeId).GreaterThan(0);
            RuleFor(x => x.SeanceComiteId).GreaterThan(0);
            RuleFor(x => x.MontantAccorde).GreaterThanOrEqualTo(0).When(x => x.MontantAccorde.HasValue);
        }
    }
}
