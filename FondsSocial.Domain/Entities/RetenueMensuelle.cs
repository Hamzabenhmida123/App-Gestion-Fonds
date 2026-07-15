using System;
using FondsSocial.Domain.Common;
using FondsSocial.Domain.Enums;

namespace FondsSocial.Domain.Entities
{
    public class RetenueMensuelle : BaseEntity
    {
        public DateTime Mois { get; set; }
        public int AgentId { get; set; }
        public int ContratId { get; set; }
        public decimal MontantARetenir { get; set; }
        public decimal? MontantEffectivementRetenu { get; set; }
        public StatutRetenue Statut { get; set; }
        public string? ReferencePaie { get; set; }

        public virtual Agent Agent { get; set; } = null!;
        public virtual Contrat Contrat { get; set; } = null!;
    }
}
