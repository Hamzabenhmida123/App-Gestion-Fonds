using System;
using FondsSocial.Domain.Common;
using FondsSocial.Domain.Enums;

namespace FondsSocial.Domain.Entities
{
    public class HistoriqueStatutDemande : BaseEntity
    {
        public int DemandeId { get; set; }
        public StatutDemande Statut { get; set; }
        public DateTime DateChangement { get; set; }
        public string Auteur { get; set; } = null!;
        public string? Commentaire { get; set; }

        public virtual Demande Demande { get; set; } = null!;
    }
}
