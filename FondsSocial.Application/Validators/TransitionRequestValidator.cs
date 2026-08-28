using FluentValidation;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Validators
{
    /// <summary>
    /// Le cahier des charges (Module M1, §4) exige que chaque transition de statut
    /// soit tracée avec un auteur ET un commentaire obligatoires.
    /// </summary>
    public class TransitionRequestValidator : AbstractValidator<TransitionRequest>
    {
        public TransitionRequestValidator()
        {
            RuleFor(x => x.Auteur).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Commentaire).NotEmpty().MaximumLength(1000);
        }
    }
}
