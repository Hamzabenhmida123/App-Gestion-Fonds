using FondsSocial.Domain.Common;

namespace FondsSocial.Domain.Entities
{
    public class PieceJustificativeRequise : BaseEntity
    {
        public int TypeDePretId { get; set; }
        public string LibellePiece { get; set; } = null!;
        public bool Obligatoire { get; set; }

        public virtual TypeDePret TypeDePret { get; set; } = null!;
    }
}
