using System;
using System.ComponentModel.DataAnnotations.Schema;
using FondsSocial.Domain.Common;

namespace FondsSocial.Domain.Entities
{
    public class Garantie : BaseEntity
    {
        public int ContratId { get; set; }

        public bool AssuranceVieSouscrite { get; set; }
        public DateTime? DateAssuranceVie { get; set; }
        public string? ReferencePoliceAssurance { get; set; }

        public bool TraiteSigneeLegalisee { get; set; }
        public DateTime? DateTraite { get; set; }

        public bool EngagementRemboursementSigne { get; set; }
        public DateTime? DateEngagement { get; set; }

        public bool AutorisationRetenueSalaireSignee { get; set; }
        public DateTime? DateAutorisationRetenue { get; set; }

        [NotMapped]
        public bool ToutesGarantiesReunies => AssuranceVieSouscrite && TraiteSigneeLegalisee && EngagementRemboursementSigne && AutorisationRetenueSalaireSignee;

        public virtual Contrat Contrat { get; set; } = null!;
    }
}
