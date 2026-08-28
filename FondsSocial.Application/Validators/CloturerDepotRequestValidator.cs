using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class CloturerDepotRequestValidator : AbstractValidator<CloturerDepotRequest>
    {
        public CloturerDepotRequestValidator()
        {
            RuleFor(x => x.Auteur).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Commentaire).MaximumLength(1000);
        }
    }
}
