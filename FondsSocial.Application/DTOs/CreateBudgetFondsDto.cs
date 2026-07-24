using FondsSocial.Domain.Enums;

namespace FondsSocial.Application.DTOs
{
    public class CreateBudgetFondsDto
    {
        public int SocieteId { get; set; }
        public int Exercice { get; set; }
        public CategorieBudget Categorie { get; set; }
        public decimal Ressources { get; set; }
        public decimal Emplois { get; set; }
        public decimal Solde { get; set; }
    }
}
