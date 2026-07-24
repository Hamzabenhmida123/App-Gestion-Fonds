using System;
using FondsSocial.Domain.Enums;

namespace FondsSocial.Application.DTOs
{
    public class RetenueMensuelleDto
    {
        public int Id { get; set; }
        public DateTime Mois { get; set; }
        public int AgentId { get; set; }
        public int ContratId { get; set; }
        public decimal MontantARetenir { get; set; }
        public decimal? MontantEffectivementRetenu { get; set; }
        public StatutRetenue Statut { get; set; }
        public string? ReferencePaie { get; set; }
    }
}
