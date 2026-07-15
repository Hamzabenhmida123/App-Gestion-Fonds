using System;
using FondsSocial.Domain.Common;
using FondsSocial.Domain.Enums;

namespace FondsSocial.Domain.Entities
{
    public class Decision : BaseEntity
    {
        public int DemandeId { get; set; }
        public int SeanceComiteId { get; set; }
        public SensDecision SensDecision { get; set; }
        public decimal? MontantAccorde { get; set; }
        public DateTime? DateNotification { get; set; }
        public DateTime? DatePeremption { get; set; }

        public virtual Demande Demande { get; set; } = null!;
        public virtual SeanceComite SeanceComite { get; set; } = null!;
        public virtual Contrat? Contrat { get; set; }
    }
}
