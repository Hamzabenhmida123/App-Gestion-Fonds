using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class UpdateHistoriqueStatutDemandeDtoValidator : AbstractValidator<UpdateHistoriqueStatutDemandeDto>
    {
        public UpdateHistoriqueStatutDemandeDtoValidator()
        {
            RuleFor(x => x.DemandeId).GreaterThan(0);
            RuleFor(x => x.Statut).NotNull();
            RuleFor(x => x.DateChangement).NotEmpty();
            RuleFor(x => x.Auteur).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Commentaire).MaximumLength(1000);
        }
    }
}
