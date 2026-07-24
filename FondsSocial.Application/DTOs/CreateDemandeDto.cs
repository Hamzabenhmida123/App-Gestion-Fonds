using System;
using FondsSocial.Domain.Enums;

namespace FondsSocial.Application.DTOs
{
    public class CreateDemandeDto
    {
        public string NumeroDossier { get; set; } = null!;
        public int AgentId { get; set; }
        public int TypeDePretId { get; set; }
        public DateTime DateDepot { get; set; }
        public decimal MontantDemande { get; set; }
        public StatutDemande StatutCourant { get; set; }
        public decimal ScorePriorite { get; set; }
    }
}
