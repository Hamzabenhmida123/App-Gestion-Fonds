using FondsSocial.Domain.Common;
using FondsSocial.Domain.Enums;

namespace FondsSocial.Domain.Entities
{
    public class BudgetFonds : BaseEntity
    {
        public int SocieteId { get; set; }
        public int Exercice { get; set; }
        public CategorieBudget Categorie { get; set; }
        public decimal Ressources { get; set; }
        public decimal Emplois { get; set; }
        public decimal Solde { get; set; }

        public virtual Societe Societe { get; set; } = null!;
    }
}
