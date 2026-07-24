using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class UpdatePieceJustificativeRequiseDtoValidator : AbstractValidator<UpdatePieceJustificativeRequiseDto>
    {
        public UpdatePieceJustificativeRequiseDtoValidator()
        {
            RuleFor(x => x.TypeDePretId).GreaterThan(0);
            RuleFor(x => x.LibellePiece).NotEmpty().MaximumLength(250);
            RuleFor(x => x.Obligatoire).NotNull();
        }
    }
}
