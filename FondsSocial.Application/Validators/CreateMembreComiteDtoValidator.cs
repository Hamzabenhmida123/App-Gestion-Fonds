using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class CreateMembreComiteDtoValidator : AbstractValidator<CreateMembreComiteDto>
    {
        public CreateMembreComiteDtoValidator()
        {
            RuleFor(x => x.Nom).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Prenom).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Fonction).MaximumLength(200);
        }
    }
}
