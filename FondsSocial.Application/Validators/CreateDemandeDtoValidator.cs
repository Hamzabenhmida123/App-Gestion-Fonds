using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class CreateDemandeDtoValidator : AbstractValidator<CreateDemandeDto>
    {
        public CreateDemandeDtoValidator()
        {
            RuleFor(x => x.AgentId).GreaterThan(0);
            RuleFor(x => x.TypeDePretId).GreaterThan(0);
            RuleFor(x => x.DateDepot).NotEmpty();
            RuleFor(x => x.MontantDemande).GreaterThan(0);
        }
    }
}
