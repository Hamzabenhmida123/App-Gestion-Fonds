using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class UpdateDecisionDtoValidator : AbstractValidator<UpdateDecisionDto>
    {
        public UpdateDecisionDtoValidator()
        {
            RuleFor(x => x.DemandeId).GreaterThan(0);
            RuleFor(x => x.SeanceComiteId).GreaterThan(0);
            RuleFor(x => x.MontantAccorde).GreaterThanOrEqualTo(0).When(x => x.MontantAccorde.HasValue);
        }
    }
}
