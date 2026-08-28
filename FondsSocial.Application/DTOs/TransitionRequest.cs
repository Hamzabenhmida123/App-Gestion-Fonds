using FondsSocial.Domain.Enums;

namespace FondsSocial.Application.DTOs
{
    public class TransitionRequest
    {
        public StatutDemande NewStatut { get; set; }
        public string Auteur { get; set; } = null!;
        public string? Commentaire { get; set; }
    }
}
