using System;
using FondsSocial.Domain.Enums;

namespace FondsSocial.Application.DTOs
{
    public class EcheanceDto
    {
        public int Id { get; set; }
        public int ContratId { get; set; }
        public int NumeroEcheance { get; set; }
        public DateTime Date { get; set; }
        public decimal CapitalRestantDu { get; set; }
        public decimal Mensualite { get; set; }
        public decimal CapitalAmorti { get; set; }
        public decimal FraisGestion { get; set; }
        public decimal SoldeRestant { get; set; }
        public StatutEcheance Statut { get; set; }
    }
}
