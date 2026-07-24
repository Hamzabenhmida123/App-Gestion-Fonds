using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class UpdateParticipationSeanceDtoValidator : AbstractValidator<UpdateParticipationSeanceDto>
    {
        public UpdateParticipationSeanceDtoValidator()
        {
            RuleFor(x => x.SeanceComiteId).GreaterThan(0);
            RuleFor(x => x.MembreComiteId).GreaterThan(0);
        }
    }
}
