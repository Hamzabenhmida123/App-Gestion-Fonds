using System.Collections.Generic;
using FondsSocial.Domain.Common;

namespace FondsSocial.Domain.Entities
{
    public class Societe : BaseEntity
    {
        public string Code { get; set; } = null!;
        public string RaisonSociale { get; set; } = null!;
        public string? ReferentielReglesGestion { get; set; }

        public virtual ICollection<Agent> Agents { get; set; } = new List<Agent>();
        public virtual ICollection<BudgetFonds> BudgetFonds { get; set; } = new List<BudgetFonds>();
    }
}
