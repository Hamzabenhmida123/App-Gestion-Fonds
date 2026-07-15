using System;
using System.Collections.Generic;
using FondsSocial.Domain.Common;
using FondsSocial.Domain.Enums;

namespace FondsSocial.Domain.Entities
{
    public class Demande : BaseEntity
    {
        public string NumeroDossier { get; set; } = null!;
        public int AgentId { get; set; }
        public int TypeDePretId { get; set; }
        public DateTime DateDepot { get; set; }
        public decimal MontantDemande { get; set; }
        public StatutDemande StatutCourant { get; set; }
        public decimal ScorePriorite { get; set; }

        public virtual Agent Agent { get; set; } = null!;
        public virtual TypeDePret TypeDePret { get; set; } = null!;

        public virtual ICollection<HistoriqueStatutDemande> HistoriqueStatuts { get; set; } = new List<HistoriqueStatutDemande>();
        public virtual ICollection<PieceJustificative> PieceJustificatives { get; set; } = new List<PieceJustificative>();
        public virtual ICollection<Decision> Decisions { get; set; } = new List<Decision>();
    }
}
