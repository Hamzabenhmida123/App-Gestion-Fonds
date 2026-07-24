namespace FondsSocial.Application.DTOs
{
    public class UpdatePieceJustificativeRequiseDto
    {
        public int TypeDePretId { get; set; }
        public string LibellePiece { get; set; } = null!;
        public bool Obligatoire { get; set; }
    }
}
