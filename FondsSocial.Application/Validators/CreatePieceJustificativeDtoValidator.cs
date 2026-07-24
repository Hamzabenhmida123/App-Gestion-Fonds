using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class CreatePieceJustificativeDtoValidator : AbstractValidator<CreatePieceJustificativeDto>
    {
        public CreatePieceJustificativeDtoValidator()
        {
            RuleFor(x => x.DemandeId).GreaterThan(0);
            RuleFor(x => x.TypePiece).NotEmpty().MaximumLength(200);
            RuleFor(x => x.CheminFichier).NotEmpty().MaximumLength(1000);
            RuleFor(x => x.DateDepot).NotEmpty();
        }
    }
}
