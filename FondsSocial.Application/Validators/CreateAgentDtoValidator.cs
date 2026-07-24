using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class CreateAgentDtoValidator : AbstractValidator<CreateAgentDto>
    {
        public CreateAgentDtoValidator()
        {
            RuleFor(x => x.Matricule).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Nom).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Prenom).NotEmpty().MaximumLength(150);
            RuleFor(x => x.CIN).NotEmpty().MaximumLength(20);
            RuleFor(x => x.IdentifiantUnique).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Adresse).MaximumLength(500);
            RuleFor(x => x.Telephone).MaximumLength(20);
        }
    }
}
