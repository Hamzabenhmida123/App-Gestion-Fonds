using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class UpdateBudgetFondsDtoValidator : AbstractValidator<UpdateBudgetFondsDto>
    {
        public UpdateBudgetFondsDtoValidator()
        {
            RuleFor(x => x.SocieteId).GreaterThan(0);
            RuleFor(x => x.Exercice).GreaterThan(2000);
            RuleFor(x => x.Ressources).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Emplois).GreaterThanOrEqualTo(0);
        }
    }
}
