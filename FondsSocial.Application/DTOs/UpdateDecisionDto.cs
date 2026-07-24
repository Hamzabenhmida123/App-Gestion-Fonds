using System;
using FondsSocial.Domain.Enums;

namespace FondsSocial.Application.DTOs
{
    public class UpdateDecisionDto
    {
        public int DemandeId { get; set; }
        public int SeanceComiteId { get; set; }
        public SensDecision SensDecision { get; set; }
        public decimal? MontantAccorde { get; set; }
        public DateTime? DateNotification { get; set; }
        public DateTime? DatePeremption { get; set; }
    }
}
