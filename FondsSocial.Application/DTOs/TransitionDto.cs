using FondsSocial.Domain.Enums;

namespace FondsSocial.Application.DTOs
{
    public class TransitionDto
    {
        public StatutDemande NewStatut { get; set; }
        public string Auteur { get; set; } = null!;
        public string Commentaire { get; set; } = null!;
    }
}
