using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class UpdateGarantieDtoValidator : AbstractValidator<UpdateGarantieDto>
    {
        public UpdateGarantieDtoValidator()
        {
            RuleFor(x => x.ContratId).GreaterThan(0);
        }
    }
}
