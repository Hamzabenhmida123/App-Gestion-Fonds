using System;

namespace FondsSocial.Application.DTOs
{
    public class CreateDemandeDto
    {
        public int AgentId { get; set; }
        public int TypeDePretId { get; set; }
        public DateTime DateDepot { get; set; }
        public decimal MontantDemande { get; set; }
    }
}
