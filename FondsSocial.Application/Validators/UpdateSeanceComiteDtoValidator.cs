using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class UpdateSeanceComiteDtoValidator : AbstractValidator<UpdateSeanceComiteDto>
    {
        public UpdateSeanceComiteDtoValidator()
        {
            RuleFor(x => x.Date).NotEmpty();
            RuleFor(x => x.ProcesVerbal).MaximumLength(2000);
        }
    }
}
