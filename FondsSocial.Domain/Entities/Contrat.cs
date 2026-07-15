using System.Collections.Generic;
using System;
using FondsSocial.Domain.Common;

namespace FondsSocial.Domain.Entities
{
    public class Contrat : BaseEntity
    {
        public int DecisionId { get; set; }
        public DateTime DateSignature { get; set; }
        public decimal MontantPrincipal { get; set; }
        public decimal FraisGestion { get; set; }
        public decimal MontantTotal { get; set; }
        public int DureeMois { get; set; }
        public DateTime DatePremiereEcheance { get; set; }

        public virtual Decision Decision { get; set; } = null!;
        public virtual Garantie? Garantie { get; set; }
        public virtual ICollection<Echeance> Echeances { get; set; } = new List<Echeance>();
        public virtual ICollection<RetenueMensuelle> RetenuesMensuelles { get; set; } = new List<RetenueMensuelle>();
    }
}
