using System;
using FondsSocial.Domain.Enums;

namespace FondsSocial.Application.DTOs
{
    public class CreateHistoriqueStatutDemandeDto
    {
        public int DemandeId { get; set; }
        public StatutDemande Statut { get; set; }
        public DateTime DateChangement { get; set; }
        public string Auteur { get; set; } = null!;
        public string? Commentaire { get; set; }
    }
}
