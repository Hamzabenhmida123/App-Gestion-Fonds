using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class UpdateDemandeDtoValidator : AbstractValidator<UpdateDemandeDto>
    {
        public UpdateDemandeDtoValidator()
        {
            RuleFor(x => x.NumeroDossier).NotEmpty().MaximumLength(100);
            RuleFor(x => x.AgentId).GreaterThan(0);
            RuleFor(x => x.TypeDePretId).GreaterThan(0);
            RuleFor(x => x.DateDepot).NotEmpty();
            RuleFor(x => x.MontantDemande).GreaterThan(0);
        }
    }
}
