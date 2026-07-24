using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class CreateGarantieDtoValidator : AbstractValidator<CreateGarantieDto>
    {
        public CreateGarantieDtoValidator()
        {
            RuleFor(x => x.ContratId).GreaterThan(0);
        }
    }
}
