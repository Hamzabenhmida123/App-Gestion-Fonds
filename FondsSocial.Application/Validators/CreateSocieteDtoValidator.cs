using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class CreateSocieteDtoValidator : AbstractValidator<CreateSocieteDto>
    {
        public CreateSocieteDtoValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.RaisonSociale).NotEmpty().MaximumLength(250);
            RuleFor(x => x.ReferentielReglesGestion).MaximumLength(1000);
        }
    }
}
