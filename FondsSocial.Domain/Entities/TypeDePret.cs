using System.Collections.Generic;
using FondsSocial.Domain.Common;
using FondsSocial.Domain.Enums;

namespace FondsSocial.Domain.Entities
{
    public class TypeDePret : BaseEntity
    {
        public string Code { get; set; } = null!;
        public string Libelle { get; set; } = null!;
        public CategorieBudget Categorie { get; set; }
        public decimal Plafond { get; set; }
        public int DureeMaxMois { get; set; }
        public int FranchiseMois { get; set; }
        public ModeCalculFraisGestion ModeCalculFraisGestion { get; set; }
        public decimal TauxOuMontantFraisGestion { get; set; }
        public bool Actif { get; set; }

        public virtual ICollection<PieceJustificativeRequise> PiecesJustificativesRequises { get; set; } = new List<PieceJustificativeRequise>();
        public virtual ICollection<Demande> Demandes { get; set; } = new List<Demande>();
    }
}
