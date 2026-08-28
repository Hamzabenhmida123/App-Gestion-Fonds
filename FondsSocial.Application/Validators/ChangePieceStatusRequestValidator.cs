using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class ChangePieceStatusRequestValidator : AbstractValidator<ChangePieceStatusRequest>
    {
        public ChangePieceStatusRequestValidator()
        {
            RuleFor(x => x.Auteur).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Commentaire).MaximumLength(1000);
        }
    }
}
