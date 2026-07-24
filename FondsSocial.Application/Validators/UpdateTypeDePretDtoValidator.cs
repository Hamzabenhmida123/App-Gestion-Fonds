using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class UpdateTypeDePretDtoValidator : AbstractValidator<UpdateTypeDePretDto>
    {
        public UpdateTypeDePretDtoValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Libelle).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Plafond).GreaterThan(0);
            RuleFor(x => x.TauxOuMontantFraisGestion).GreaterThanOrEqualTo(0);
        }
    }
}
