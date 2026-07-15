using System;
using FondsSocial.Domain.Common;
using FondsSocial.Domain.Enums;

namespace FondsSocial.Domain.Entities
{
    public class PieceJustificative : BaseEntity
    {
        public int DemandeId { get; set; }
        public string TypePiece { get; set; } = null!;
        public string CheminFichier { get; set; } = null!;
        public DateTime DateDepot { get; set; }
        public StatutVerification StatutVerification { get; set; }

        public virtual Demande Demande { get; set; } = null!;
    }
}
