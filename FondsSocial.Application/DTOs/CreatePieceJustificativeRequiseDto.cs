namespace FondsSocial.Application.DTOs
{
    public class CreatePieceJustificativeRequiseDto
    {
        public int TypeDePretId { get; set; }
        public string LibellePiece { get; set; } = null!;
        public bool Obligatoire { get; set; }
    }
}
