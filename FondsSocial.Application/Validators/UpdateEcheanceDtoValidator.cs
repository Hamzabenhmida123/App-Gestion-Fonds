using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class UpdateEcheanceDtoValidator : AbstractValidator<UpdateEcheanceDto>
    {
        public UpdateEcheanceDtoValidator()
        {
            RuleFor(x => x.ContratId).GreaterThan(0);
            RuleFor(x => x.NumeroEcheance).GreaterThan(0);
            RuleFor(x => x.Date).NotEmpty();
            RuleFor(x => x.Mensualite).GreaterThanOrEqualTo(0);
        }
    }
}
