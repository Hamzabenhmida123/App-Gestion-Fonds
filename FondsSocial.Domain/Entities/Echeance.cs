using System;
using FondsSocial.Domain.Common;
using FondsSocial.Domain.Enums;

namespace FondsSocial.Domain.Entities
{
    public class Echeance : BaseEntity
    {
        public int ContratId { get; set; }
        public int NumeroEcheance { get; set; }
        public DateTime Date { get; set; }
        public decimal CapitalRestantDu { get; set; }
        public decimal Mensualite { get; set; }
        public decimal CapitalAmorti { get; set; }
        public decimal FraisGestion { get; set; }
        public decimal SoldeRestant { get; set; }
        public StatutEcheance Statut { get; set; }

        public virtual Contrat Contrat { get; set; } = null!;
    }
}
