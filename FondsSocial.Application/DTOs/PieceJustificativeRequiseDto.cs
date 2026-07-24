namespace FondsSocial.Application.DTOs
{
    public class PieceJustificativeRequiseDto
    {
        public int Id { get; set; }
        public int TypeDePretId { get; set; }
        public string LibellePiece { get; set; } = null!;
        public bool Obligatoire { get; set; }
    }
}
