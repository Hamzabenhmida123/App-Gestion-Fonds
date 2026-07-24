using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class CreateRetenueMensuelleDtoValidator : AbstractValidator<CreateRetenueMensuelleDto>
    {
        public CreateRetenueMensuelleDtoValidator()
        {
            RuleFor(x => x.Mois).NotEmpty();
            RuleFor(x => x.AgentId).GreaterThan(0);
            RuleFor(x => x.ContratId).GreaterThan(0);
            RuleFor(x => x.MontantARetenir).GreaterThanOrEqualTo(0);
        }
    }
}
