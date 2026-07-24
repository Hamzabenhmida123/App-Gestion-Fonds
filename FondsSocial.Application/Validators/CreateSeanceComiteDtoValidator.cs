using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    public class CreateSeanceComiteDtoValidator : AbstractValidator<CreateSeanceComiteDto>
    {
        public CreateSeanceComiteDtoValidator()
        {
            RuleFor(x => x.Date).NotEmpty();
            RuleFor(x => x.ProcesVerbal).MaximumLength(2000);
        }
    }
}
