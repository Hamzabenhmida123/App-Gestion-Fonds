using System;

namespace FondsSocial.Application.DTOs
{
    public class CreateContratDto
    {
        public int DecisionId { get; set; }
        public DateTime DateSignature { get; set; }
        public decimal MontantPrincipal { get; set; }
        public decimal FraisGestion { get; set; }
        public decimal MontantTotal { get; set; }
        public int DureeMois { get; set; }
        public DateTime DatePremiereEcheance { get; set; }
    }
}
